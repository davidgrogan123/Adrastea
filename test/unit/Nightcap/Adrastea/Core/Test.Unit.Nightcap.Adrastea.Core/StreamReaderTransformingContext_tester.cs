using global::System.Text;
using Nightcap.Adrastea.Core;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class StreanReaderTransformingContext_tester
    {
        #region tests
        
        [Test]
        public void StreamTransformingContext_throws_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => new StreamReaderTransformingContext(null)
            );
        }
        
        [Test]
        public void StreamReaderTransformingContext_does_not_throw_for_correct_input()
        {
            using (Stream stream = new MemoryStream())
            {
                Assert.DoesNotThrow(
                    () => new StreamReaderTransformingContext(stream)
                );
            }
        }
        
        [Test]
        public void StreamReaderTransformingContext_disposes()
        {
            using (Stream stream = new MemoryStream())
            {
                var tc = new StreamReaderTransformingContext(stream);

                Assert.DoesNotThrow(
                      () => tc.CreateStreamReader().Dispose()
                    , "Creating reader threw an exception"
                );

                Assert.True(stream.CanRead, "Stream should be readable");
                
                Assert.DoesNotThrow(
                      () => tc.Dispose()
                    , "Disposing context threw an exception"
                );

                Assert.Throws<ObjectDisposedException>(
                      () => tc.CreateStreamReader()
                    , "Disposed context did not throw an exception"
                );

                Assert.False(
                      stream.CanRead
                    , "Context should dispose underlying reader and so should not be readable"
                );
            }
        }
        
        [Test]
        public void StreamReaderTransformingContext_stream_has_correct_content()
        {
            string expected = "abc";

            using (Stream stream = new MemoryStream())
            {
                using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8, 1024, true))
                {
                    sw.Write(expected);
                }

                stream.Seek(0, SeekOrigin.Begin);

                using (var tc = new StreamReaderTransformingContext(stream))
                using (StreamReader sr = tc.CreateStreamReader())
                {
                    Assert.AreEqual(expected, sr.ReadToEnd());
                }
            }
        }
        #endregion
    }
}