using global::Nightcap.Core.Diagnostics.CodeContract;
using global::System.Text;
using global::System.Xml;
using Nightcap.Adrastea.Core;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;


namespace Test.Unit.Nightcap.Adrastea.Core
{

    [TestFixture]
    internal sealed class CsvToXmlTransformation_tester
    {
        #region setup

        [OneTimeSetUp]
        public void Test_Setup()
        {
            EnforcementCore.Enforcement = true;
            EnforcementCore.ConfigureReporterType(EnforcementCore.ViolationReportAction.Exception);
        }
        #endregion

        #region tests

        [Test]
        public void Test_Transform_throws_exception_when_context_is_null()
        {
            Assert.Throws<CodeContractViolationException>(
                  () => new CsvToXmlTransformation().Transform(null, new Delegate[] {})
            );
        }

        [Test]
        public void Test_Transform_throws_exception_for_empty_column_row()
        {
            var inputs = new string[] { @"", @"
", @"
 " };

            foreach(var input in inputs)
            {
                using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
                using (ITransformingContext tc = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
                {
                    Assert.Throws<ApplicationException>(
                          () => new CsvToXmlTransformation().Transform(tc, new List<Delegate>()));
                }
            }
        }

        [Test]
        public void Test_Transform_throws_exception_for_empty_column_name()
        {
            var inputs = new string[] { @"col 1,, col 3", @"col 1, , col 3" };


            foreach(var input in inputs)
            {
                using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
                using (ITransformingContext tc = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
                {
                    Assert.Throws<ApplicationException>(
                          () => new CsvToXmlTransformation().Transform(tc, new List<Delegate>()));
                }
            }
        }

        [Test]
        public void Test_Transform_supports_no_data_rows()
        {
            var input = @"col1,col2";
            var expected = @"<DocumentElement />";


            using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (ITransformingContext tcIn = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
            {
                using (ITransformingContext tcOut = new CsvToXmlTransformation().Transform(tcIn, new List<Delegate>()))
                {
                    IXmlReaderTransformingContext tcXml;
                    Assert.True(tcOut.TryGetInterface(out tcXml));

                    using (XmlReader r = tcXml.CreateXmlReader())
                    {
                        r.Read();
                        Assert.AreEqual(
                              expected
                            , r.ReadOuterXml()
                        );
                    }
                }
            }
        }

        [Test]
        public void Test_Transform_supports_missing_data_values()
        {
            var input = @"col1,col2
d
,s";

            var expected = 
@"<DocumentElement><csv><col1>d</col1></csv><csv><col1></col1><col2>s</col2></csv></DocumentElement>";


            using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (ITransformingContext tcIn = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
            {
                using (ITransformingContext tcOut = new CsvToXmlTransformation().Transform(tcIn, new List<Delegate>()))
                {
                    IXmlReaderTransformingContext tcXml;
                    Assert.True(tcOut.TryGetInterface(out tcXml));

                    using (XmlReader r = tcXml.CreateXmlReader())
                    {
                        r.Read();
                        Assert.AreEqual(
                              expected
                            , r.ReadOuterXml()
                        );
                    }
                }
            }
        }

        [Test]
        public void Test_Transform_ignores_empty_rows1()
        {
            var input = @"col1,col2,col3
,,
, , 
";
            var expected = @"<DocumentElement />";

            using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (ITransformingContext tcIn = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
            {
                using (ITransformingContext tcOut = new CsvToXmlTransformation().Transform(tcIn, new List<Delegate>()))
                {
                    IXmlReaderTransformingContext tcXml;
                    Assert.True(tcOut.TryGetInterface(out tcXml));

                    using (XmlReader r = tcXml.CreateXmlReader())
                    {
                        r.Read();
                        Assert.AreEqual(
                              expected
                            , r.ReadOuterXml()
                        );
                    }
                }
            }
        }

        [Test]
        public void Test_Transform_ignores_empty_rows2()
        {
            var input = @"col1,col2,col3
,,
a,b,c
, , 
";
            var expected = 
@"<DocumentElement><csv><col1>a</col1><col2>b</col2><col3>c</col3></csv></DocumentElement>";

            using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (ITransformingContext tcIn = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
            {
                using (ITransformingContext tcOut = new CsvToXmlTransformation().Transform(tcIn, new List<Delegate>()))
                {
                    IXmlReaderTransformingContext tcXml;
                    Assert.True(tcOut.TryGetInterface(out tcXml));

                    using (XmlReader r = tcXml.CreateXmlReader())
                    {
                        r.Read();
                        Assert.AreEqual(
                              expected
                            , r.ReadOuterXml()
                        );
                    }
                }
            }
        }

        [Test]
        public void Test_Transform_parses_quotes_and_invalid_xml_characters()
        {
            var input = @"col 1,"" col 2,d"",45col,-2
1,      2     ,3,""4 , slijasdlfij""
3,j""ilksf8hj9823fhhhhhso8hj`19vcmlz'\/m.;j[[],5,l;ap98jpf9aj;oaijf9823ja;o98fwj6
""x
y""";

            var expected = 
@"<DocumentElement><csv><col_x0020_1>1</col_x0020_1>"
+ @"<_x0020_col_x0020_2_x002C_d>      2     </_x0020_col_x0020_2_x002C_d>"
+ @"<_x0034_5col>3</_x0034_5col><_x002D_2>4 , slijasdlfij</_x002D_2></csv>"
+ @"<csv><col_x0020_1>3</col_x0020_1>"
+ @"<_x0020_col_x0020_2_x002C_d>j""ilksf8hj9823fhhhhhso8hj`19vcmlz'\/m.;j[[]</_x0020_col_x0020_2_x002C_d>"
+ @"<_x0034_5col>5</_x0034_5col><_x002D_2>l;ap98jpf9aj;oaijf9823ja;o98fwj6</_x002D_2>"
+ @"</csv><csv><col_x0020_1>x
y</col_x0020_1></csv></DocumentElement>";

            using (Stream ms = new MemoryStream(Encoding.UTF8.GetBytes(input)))
            using (ITransformingContext tcIn = TransformingContextUtil.CreateStreamReaderTransformingContext(ms))
            {
                using (ITransformingContext tcOut = new CsvToXmlTransformation().Transform(tcIn, new List<Delegate>()))
                {
                    IXmlReaderTransformingContext tcXml;
                    Assert.True(tcOut.TryGetInterface(out tcXml));

                    using (XmlReader r = tcXml.CreateXmlReader())
                    {
                        r.Read();
                        Assert.AreEqual(
                              expected
                            , r.ReadOuterXml()
                        );
                    }
                }
            }
        }

        #endregion
    }
}
