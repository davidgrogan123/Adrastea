using Bud;
using global::System.Xml;
using global::System.Xml.Linq;
using Nightcap.Adrastea.Core;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class TransformingContextUtil_tester
    {
        #region tests
        [Test]
        public void TransformingContextUtil_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(
                  () => TransformingContextUtil.CreateFileEntryTransformingContext(null)
                , "Null FileEntry context creation did not throw exception"
            );

            Assert.Throws<CodeContractViolationException>(
                  () => TransformingContextUtil.CreateXmlReaderTransformingContext((XmlReader) null)
                , "XmlReader context from null XmlReader did not throw exception"
            );

            Assert.Throws<CodeContractViolationException>(
                  () => TransformingContextUtil.CreateXmlReaderTransformingContext((XDocument) null)
                , "XmlReader context from null XDocument did not throw exception"
            );

            Assert.Throws<CodeContractViolationException>(
                  () => TransformingContextUtil.CreateStreamReaderTransformingContext(null)
                , "Null StreamReader context creation did not throw exception"
            );
        }
        
        [Test]
        public void TransformingContextUtil_CreateFileEntryTransformingContext_creates_correct_context()
        {
            using (TmpDir dir = new TmpDir())
            {
                string fileEntry = dir.CreateEmptyFile();

                using (ITransformingContext tc 
                            = TransformingContextUtil
                                .CreateFileEntryTransformingContext(fileEntry)
                )
                {
                    IFileEntryTransformingContext    tcFile;
                    IXmlReaderTransformingContext    tcXml;
                    IStreamReaderTransformingContext tcStream;

                    Assert.True(tc.TryGetInterface(out tcFile));
                    Assert.False(tc.TryGetInterface(out tcXml));
                    Assert.False(tc.TryGetInterface(out tcStream));
                }
            }
        }
        
        [Test]
        public void TransformingContextUtil_CreateXmlReaderTransformingContext_creates_correct_context_with_reader()
        {
            using (TmpDir dir = new TmpDir())
            {
                string inputFile = dir.CreateFile("<root/>");

                using (XmlReader readerIn = XmlReader.Create(inputFile))
                using (ITransformingContext tc 
                            = TransformingContextUtil
                                .CreateXmlReaderTransformingContext(readerIn)
                )
                {
                    IFileEntryTransformingContext    tcFile;
                    IXmlReaderTransformingContext    tcXml;
                    IStreamReaderTransformingContext tcStream;

                    Assert.True(tc.TryGetInterface(out tcXml));
                    Assert.False(tc.TryGetInterface(out tcFile));
                    Assert.False(tc.TryGetInterface(out tcStream));
                }
            }
        }
        
        [Test]
        public void TransformingContextUtil_CreateXmlReaderTransformingContext_creates_correct_context_with_XDoc()
        {
            XDocument doc = new XDocument();

            using (ITransformingContext tc 
                        = TransformingContextUtil
                            .CreateXmlReaderTransformingContext(doc)
            )
            {
                IFileEntryTransformingContext    tcFile;
                IXmlReaderTransformingContext    tcXml;
                IStreamReaderTransformingContext tcStream;

                Assert.True(tc.TryGetInterface(out tcXml));
                Assert.False(tc.TryGetInterface(out tcFile));
                Assert.False(tc.TryGetInterface(out tcStream));
            }
        }
        
        [Test]
        public void TransformingContextUtil_CreateStreamReaderTransformingContext_creates_correct_context()
        {
            using (Stream stream = new MemoryStream())
            using (ITransformingContext tc 
                        = TransformingContextUtil
                            .CreateStreamReaderTransformingContext(stream)
            )
            {
                IFileEntryTransformingContext    tcFile;
                IXmlReaderTransformingContext    tcXml;
                IStreamReaderTransformingContext tcStream;

                Assert.True(tc.TryGetInterface(out tcStream));
                Assert.False(tc.TryGetInterface(out tcFile));
                Assert.False(tc.TryGetInterface(out tcXml));
            }
        }
        #endregion
    }
}

