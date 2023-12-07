using Nightcap.Adrastea.Core;

namespace Test.Unit.Nightcap.Adrastea.Core
{

    [TestFixture]
    internal sealed class FileTransformEndEventArgs_tester
    {
        #region tests

        [Test]
        public void FileTransformEndEventArgs_has_correct_properties()
        {
            Assert.NotNull(
                  typeof(FileTransformEndEventArgs).GetProperty("SuccessfullyProcessed")
                , "FileTransformEndEventArgs does not have property 'SuccessfullyProcessed'"
            );
        }

        [Test]
        public void FileTransformEndEventArgs_creates_object_correctly()
        {
            bool success = true;

            FileTransformEndEventArgs eventArgs = new FileTransformEndEventArgs(success);
            Assert.NotNull(eventArgs, "Constructor did not create object");
            Assert.AreEqual(success, eventArgs.SuccessfullyProcessed, "'SuccessfullyProcessed' not correctly set true");

            success = false;
            eventArgs = new FileTransformEndEventArgs(success);
            Assert.NotNull(eventArgs, "Constructor did not create object");
            Assert.AreEqual(success, eventArgs.SuccessfullyProcessed, "'SuccessfullyProcessed' not correctly set false");
        }
        #endregion
    }
}
