using global::Bud; // for TmpDir
using global::System.Xml;
using global::System.Xml.Linq;
using Nightcap.Adrastea.Core;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{


    [TestFixture]
    internal sealed class TransformingContext_tester
    {
        #region tests
        [Test]
        public void TransformingContext_throws_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => new TransformingContext(null)
            );
        }

        [Test]
        public void TransformingContext_TryGetInterface_exposes_interfaces()
        {
            using (TmpDir dir = new TmpDir())
            {
                string      fileEntry   = dir.CreateEmptyFile();
                XDocument   doc         = new XDocument(new XElement("root"));

                using (Stream ms = new MemoryStream())
                {
                    var tcFileEntryIn = new FileEntryTransformingContext(fileEntry);
                    var tcXmlIn = new XmlReaderTransformingContext(doc);
                    var tcStreamIn = new StreamReaderTransformingContext(ms);

                    object[] contexts = new object[]
                        {
                              tcFileEntryIn
                            , tcXmlIn
                            , tcStreamIn
                        };

                    var tc = new TransformingContext(contexts);

                    IFileEntryTransformingContext       tcFileEntryOut;
                    IXmlReaderTransformingContext       tcXmlOut;
                    IStreamReaderTransformingContext    tcStreamOut;

                    Assert.True(
                        tc.TryGetInterface(out tcFileEntryOut)
                        , "Did not contain a FileEntryTransformingContext"
                    );

                    Assert.True(
                        tc.TryGetInterface(out tcXmlOut)
                        , "Did not contain a XmlReaderTransformingContext"
                    );

                    Assert.True(
                        tc.TryGetInterface(out tcStreamOut)
                        , "Did not contain a StreamReaderTransformingContext"
                    );

                    Assert.AreSame(
                          tcFileEntryIn
                        , tcFileEntryOut
                        , "FileEntry transforming context was not the same"
                    );

                    Assert.AreSame(
                          tcXmlIn
                        , tcXmlIn
                        , "XmlReader transforming context was not the same"
                    );

                    Assert.AreSame(
                          tcStreamIn
                        , tcStreamIn
                        , "StreamReader transforming context was not the same"
                    );
                }
            }
        }

        [Test]
        public void TransformingContext_disposes_contained_contexts()
        {
            using (TmpDir dir = new TmpDir())
            {
                string      elementName = "root";
                string      fileEntry   = dir.CreateEmptyFile();
                XDocument   doc         = new XDocument(new XElement(elementName));

                using (Stream ms = new MemoryStream())
                {
                    List<object> contexts = new List<object>();

                    contexts.Add(new FileEntryTransformingContext(fileEntry));
                    contexts.Add(new XmlReaderTransformingContext(doc));
                    contexts.Add(new StreamReaderTransformingContext(ms));
                    using (XmlReader r = doc.CreateReader())
                    {
                        contexts.Add(new XmlReaderTransformingContext(r));
                    }

                    var tc = new TransformingContext(contexts.ToArray());

                    IFileEntryTransformingContext       tcFileEntry;
                    IXmlReaderTransformingContext       tcXml;
                    IStreamReaderTransformingContext    tcStream;

                    Assert.True(
                        tc.TryGetInterface(out tcFileEntry)
                        , "Did not contain a FileEntryTransformingContext"
                    );

                    Assert.True(
                        tc.TryGetInterface(out tcXml)
                        , "Did not contain a XmlReaderTransformingContext"
                    );

                    Assert.True(
                        tc.TryGetInterface(out tcStream)
                        , "Did not contain a StreamReaderTransformingContext"
                    );

                    Assert.DoesNotThrow(
                        () => tcStream.CreateStreamReader().Dispose()
                        , "Should be able to create a stream"
                    );

                    Assert.True(ms.CanRead, "Underlying stream should be readable");

                    Assert.DoesNotThrow(
                        () => tc.Dispose()
                        , "Disposing context threw an exception"
                    );

                    Assert.False(ms.CanRead, "Underlying stream should no longer be readable");

                    Assert.Throws<ObjectDisposedException>(
                        () => tcStream.CreateStreamReader()
                        , "Should not be able to create stream after disposal"
                    );

                    Assert.Throws<ObjectDisposedException>(
                        () => tc.TryGetInterface(out tcFileEntry)
                        , "Should not be able to get FileEntry context after disposal"
                    );

                    Assert.Throws<ObjectDisposedException>(
                        () => tc.TryGetInterface(out tcXml)
                        , "Should not be able to get XmlReader context after disposal"
                    );

                    Assert.Throws<ObjectDisposedException>(
                        () => tc.TryGetInterface(out tcStream)
                        , "Should not be able to get StreamReader context after disposal"
                    );
                }
            }
        }
        #endregion
    }
}
