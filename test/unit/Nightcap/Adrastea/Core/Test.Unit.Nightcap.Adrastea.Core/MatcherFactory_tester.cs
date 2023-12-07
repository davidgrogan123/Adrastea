using Nightcap.Adrastea.Core;
using System.Xml;


namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class MatcherFactory_tester
    {
        #region tests

        [Test]
        public void CreateMatcher_throws_an_exception_when_type_not_found()
        {
            var config = @"<matcher type=""RegexFileEntryMatchers"">liuahwfliuhlawg89ou1[2';9u</matcher>";
            var factory = new MatcherFactory();
            Assert.Throws<ApplicationException>(
                  () => factory.CreateMatcher(
                        XmlReader.Create(
                              new StringReader(
                                    config
                              )
                        )
                  )
            );
        }

        [Test]
        public void CreateMatcher_creates_a_RegexFileEntryMatcher()
        {
            var config = @"<matcher type=""RegexFileEntryMatcher"">(\w+)\s+(car)</matcher>";
            var factory = new MatcherFactory();
            var m = factory.CreateMatcher(
                  XmlReader.Create(
                        new StringReader(
                              config
                        )
                  )
            );
            Assert.IsNotNull(m);
            Assert.IsNotNull(m.GetType() == typeof(RegexFileEntryMatcher));
            Assert.IsFalse(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext(@"la;oiuhfj89o4hjcloi89jc""';C{AI9;'p30'1[2));")})));
            Assert.IsTrue(m.IsMatch(new MatchingContext(new object[] {new FileEntryMatchingContext("One car red car blue car")})));
        }
        #endregion
    }
}
