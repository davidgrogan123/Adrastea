using CsvHelper;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract;
using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace Nightcap.Adrastea.Core
{

    /// <summary>
    ///  Transforms CSV into XML.
    /// </summary>
    /// <remarks>
    ///  The first row of the input CSV file should contain column names.
    ///  These will become the element names in the output XML. Each
    ///  subsequent data field in the CSV will become an element value. Double
    ///  quotes can be used around fields in the CSV that contain reserved
    ///  characters (such as commas, or newlines) to preserve the field contents.
    ///
    ///  Any empty data rows in the CSV data will be ignored when it is
    ///  converted to XML. A row is considered 'empty' if all fields are empty
    ///  or contain only whitespace. The first row in the CSV must not contain
    ///  any empty fields, as these will define the element names in the
    ///  converted XML. Each column name must also be unique.
    ///
    ///  Any invalid XML characters will be encoded using the The World Wide
    ///  Web Consortium (W3C) Extensible Markup Language (XML) 1.0 (Second
    ///  Edition) standard format. This means the format _xHHHH_, where HHHH
    ///  stands for the four-digit hexadecimal representation of the invalid
    ///  character. For example, the column name 'Column Name 1' is encoded as
    ///  'Column_x0020_Name_x0020_1', when it is represented within an XML
    ///  attribute/element name.
    ///
    ///  For example, the following CSV:
    ///
    ///  ColName1,Col Name2
    ///  Data1,Data 2
    ///  Data3,"Data,4"
    ///
    ///  will be converted to the following XML:
    /// 
    ///  &lt;DocumentElement&gt;
    ///    &lt;csv&gt;
    ///      &lt;ColName1&gt;Data1&lt;/ColName1&gt;
    ///      &lt;Col_x0020_Name2&gt;Data 2&lt;/Col_x0020_Name2&gt;
    ///    &lt;/csv&gt;
    ///    &lt;csv&gt;
    ///      &lt;ColName1&gt;Data3&lt;/ColName1&gt;
    ///      &lt;Col_x0020_Name2&gt;Data,4&lt;/Col_x0020_Name2&gt;
    ///    &lt;/csv&gt;
    ///  &lt;/DocumentElement&gt;
    /// </remarks>
    public sealed class CsvToXmlTransformation : ITransformation
    {
        #region constants

        private static class Constants
        {
            internal static class FailureMessages
            {
                internal const string DuplicateColumnName_1 =
                    "Could not add column with duplicate name. {0}";
                internal const string EmptyColumnName =
                    "Could not add column with empty or whitespace name.";
                internal const string InvalidColumnName_1 =
                    "Could not add column with invalid name. {0}";
                internal const string NoColumnHeaderRow_1 =
                    "Could not read column header row. {0}";
                internal const string NoSupportedTransformingContext =
                    "Transforming Context does not contain supported interface";
            }

            // the name of the parent XML element that will group together all
            // XML elements in a row. Please refer to the XML example in the
            // class comments, and note use of the <csv> element
            internal const string CsvRowElementName = "csv";
        }
        #endregion

        #region methods

        /// <inheritdoc/>
        public ITransformingContext Transform(
              ITransformingContext context
            , IEnumerable<Delegate> eventHandlers
        )
        {
            Requires.ThatArgumentIsNotNull(context, "context");
            Requires.ThatArgumentIsNotNull(eventHandlers, "eventHandlers");

            IStreamReaderTransformingContext contextIn;
            if (!context.TryGetInterface(out contextIn))
            {
                // TODO: Determine correct exception type.
                throw new ApplicationException(
                    Constants.FailureMessages.NoSupportedTransformingContext
                );
            }

            using (var dt = new DataTable())
            {
                using (TextReader reader = contextIn.CreateStreamReader())
                using (CsvReader csvReader = new CsvReader(reader))
                {
                    // read CSV column header information
                    try
                    {
                        csvReader.ReadHeader();
                    }
                    catch (CsvReaderException x)
                    {
                        // TODO: determine correct exception type
                        throw new ApplicationException(
                              String.Format(
                                    Constants.FailureMessages.NoColumnHeaderRow_1
                                  , x.Message
                              )
                            , x
                        );
                    }

                    // add columns to the data table
                    foreach (var columnName in csvReader.FieldHeaders)
                    {
                        if (String.IsNullOrWhiteSpace(columnName))
                        {
                            // TODO: determine correct exception type
                            throw new ApplicationException(
                                  Constants.FailureMessages.EmptyColumnName
                            );
                        }

                        try
                        {
                            dt.Columns.Add(columnName);
                        }
                        catch (DuplicateNameException x)
                        {
                            // TODO: determine correct exception type
                            throw new ApplicationException(
                                  String.Format(
                                        Constants.FailureMessages.DuplicateColumnName_1
                                      , x.Message
                                  )
                                , x
                            );
                        }
                        catch (InvalidExpressionException x)
                        {
                            // TODO: determine correct exception type
                            throw new ApplicationException(
                                  String.Format(
                                        Constants.FailureMessages.InvalidColumnName_1
                                      , x.Message
                                  )
                                , x
                            );
                        }
                    }

                    // add rows to the data table
                    while (csvReader.Read())
                    {
                        // determine whether the current row is empty, and
                        // ignore it if so
                        bool isCsvRowEmpty =
                            csvReader.CurrentRecord.All(
                                  String.IsNullOrWhiteSpace
                            );

                        if (!isCsvRowEmpty)
                        {
                            var row = dt.NewRow();

                            foreach (DataColumn column in dt.Columns)
                            {
                                row[column.ColumnName] =
                                    csvReader.GetField(column.ColumnName);
                            }

                            dt.Rows.Add(row);
                        }
                    }
                }

                // write the XML

                // the following works because WriteXml will group row data
                // using the table name
                dt.TableName = Constants.CsvRowElementName;

                XDocument doc = new XDocument();

                using (XmlWriter writer = doc.CreateWriter())
                {
                    dt.WriteXml(writer);
                }

                return TransformingContextUtil
                    .CreateXmlReaderTransformingContext(doc);
            }
        }
        #endregion
    }
}
