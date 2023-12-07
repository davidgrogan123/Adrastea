using global::Bud;
using global::System.Xml;
using global::System.Xml.Xsl;
using Nightcap.Adrastea.Core;
using Nightcap.Adrastea.Core.Interfaces;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;

namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class TransformationFactory_tester
    {
        #region private support classes

        class MatchingContextFactory : IMatchingContextFactory
        {
            IMatchingContext IMatchingContextFactory.CreateMatchingContext(
                    object content
                  , ContentType contentType
            )
            {
                Assert.AreEqual(ContentType.XmlReader, contentType, "ContentType was not 'XmlReader'");
                Assert.IsInstanceOf<XmlReader>(content as XmlReader, "Content could not be cast to XmlReader");

                XmlReader reader = content as XmlReader;

                List<object> interfaces = new List<object>();

                var context = new MatchingContext(
                      interfaces.ToArray()
                );

                return context;
            }
        }

        class FakeMatcherFactory : IMatcherFactory
        {
            public IMatcher CreateMatcher(XmlReader reader)
            {
                return new FakeMatcher();
            }
        }

        class FakeMatcher : IMatcher
        {
            public bool IsMatch(IMatchingContext context)
            {
                return false;
            }
        }
        #endregion

        #region tests

        [Test]
        public void TransformationFactory_constructor_throws_when_input_is_null()
        {
            string path                     = "some/path";
            XsltArgumentList xsltArgs       = new XsltArgumentList();
            IMatchingContextFactory mcf     = new MatchingContextFactory();
            IMatcherFactory mf              = new MatcherFactory();

            Assert.Throws<CodeContractViolationException>(() =>
                new TransformationFactory(null, mf)
                , "null matchingContextFactory did not throw exception"
            );

            Assert.Throws<CodeContractViolationException>(() =>
                new TransformationFactory(mcf, null)
                , "null matcherFactory did not throw exception"
            );
        }

        [Test]
        public void CreateTransformation_throws_an_exception_when_type_not_found()
        {
            var config = @"<transformation type=""xsl"">standard_rates_table_with_prefix.xsl</transformation>";
            using (var dir = new TmpDir())
            {
                var factory = new TransformationFactory(
                      new MatchingContextFactory()
                    , new MatcherFactory()
                );

                Assert.Throws<ApplicationException>(
                      () => factory.CreateTransformation(
                            XmlReader.Create(
                                  new StringReader(
                                        config
                                  )
                            )
                      )
                );
            }
        }

        [Test]
        public void CreateTransformation_creates_a_CsvToXmlTransformation()
        {
            var config = @"<transformation type=""CsvToXmlTransformation""/>";
            using (var dir = new TmpDir())
            {
                var factory = new TransformationFactory(
                      new MatchingContextFactory()
                    , new MatcherFactory()
                );

                var matcher = factory.CreateTransformation(
                      XmlReader.Create(
                            new StringReader(
                                  config
                            )
                      )
                );
                Assert.IsNotNull(matcher);
                Assert.IsNotNull(
                      matcher.GetType() == typeof(CsvToXmlTransformation)
                );
            }
        }
        #endregion
    }
}
