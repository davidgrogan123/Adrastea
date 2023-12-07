using global::Nightcap.Adrastea.Core;
using global::Nightcap.Adrastea.Core.Interfaces;
using global::System.Xml;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;


namespace Test.Unit.Nightcap.Adrastea.Core
{

    [TestFixture]
    internal sealed class FileEntryTransformerFactory_tester
    {
        #region support classes

        class FileEntryMatcher : IMatcher
        {
            public bool IsMatch(IMatchingContext context)
            {
                return true;
            }
        }

        class FileEntryMatcherFactory : IMatcherFactory
        {
            public IMatcher CreateMatcher(XmlReader config)
            {
                return new FileEntryMatcher();
            }
        }

        class Transformation : ITransformation
        {
            public ITransformingContext Transform(ITransformingContext context, IEnumerable<Delegate> eventHandlers)
            {
                return null;
            }
        }

        class TransformationFactory : ITransformationFactory
        {
            public ITransformation CreateTransformation(XmlReader config)
            {
                return new Transformation();
            }
        }

        class MatchingContextFactory : IMatchingContextFactory
        {
            IMatchingContext IMatchingContextFactory.CreateMatchingContext(
                    object content
                  , ContentType contentType
            )
            {
                Assert.AreEqual(ContentType.FilePath, contentType, "Content type was not 'FilePath'");
                Assert.IsInstanceOf<string>(content as string, "Content was not a string");

                string fileEntry = content as string;

                var interfaces = new List<object> {
                    new FileEntryMatchingContext(
                          fileEntry
                    )
                };

                var context = new MatchingContext(
                      interfaces.ToArray()
                );

                return context;
            }
        }
        
        #endregion
        
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
        public void FileEntryTransformerFactory_constructor_enforces_code_contract()
        {
            var config = @"
  <transformers>
    <transformer>
      <matcher type=""regex"">rz(Curves|Curves_ZCIS|OIS).csv</matcher>
      <transformations>
        <transformation type=""xsl"">standard_rates_table_with_prefix.xsl</transformation>
      </transformations>
    </transformer>
  </transformers>";

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          null
                        , new FileEntryMatcherFactory()
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] {}
                        , "./somefolder"
                        , "someprefix"
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , null
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , "./somefolder"
                        , "someprefix"
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , new FileEntryMatcherFactory()
                        , null
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , "./somefolder"
                        , "someprefix"
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , new FileEntryMatcherFactory()
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , null
                        , "someprefix"
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , new FileEntryMatcherFactory()
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , " "
                        , "someprefix"
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , new FileEntryMatcherFactory()
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , "./somefolder"
                        , null
                    )
            );

            Assert.Throws<CodeContractViolationException>(
                () => new FileEntryTransformerFactory(
                          XmlReader.Create(new StringReader(config))
                        , new FileEntryMatcherFactory()
                        , new TransformationFactory()
                        , new MatchingContextFactory()
                        , new Delegate[] { }
                        , "./somefolder"
                        , " "
                    )
            );
        }

        [Test]
        public void CreateTransformers_correctly_returns_a_single_transformer()
        {
            var config = @"
  <transformers>
    <transformer>
      <matcher type=""regex"">rz(Curves|Curves_ZCIS|OIS).csv</matcher>
      <transformations>
        <transformation type=""xsl"">standard_rates_table_with_prefix.xsl</transformation>
      </transformations>
    </transformer>
  </transformers>";

            var factory = new FileEntryTransformerFactory(
                  XmlReader.Create(new StringReader(config))
                , new FileEntryMatcherFactory()
                , new TransformationFactory()
                , new MatchingContextFactory()
                , new List<Delegate>()
                , "./somefolder"
                , "someprefix"
            );

            var transformers = factory.CreateTransformers("somefile.xml");
            Assert.IsNotNull(transformers);
            Assert.AreEqual(1, transformers.Count);
        }

        [Test]
        public void CreateTransformers_correctly_returns_multiple_transformers()
        {
            var config = @"
  <transformers>
    <transformer>
      <matcher type=""regex"">rz(Curves|Curves_ZCIS|OIS).csv</matcher>
      <transformations>
        <transformation type=""xsl"">standard_rates_table_with_prefix.xsl</transformation>
      </transformations>
    </transformer>
    <transformer>
      <matcher>a;wfeijql;a82ghfp;q;[0gaj.csv</matcher>
      <transformations>
        <transformation>98ahfw98hwf39hf</transformation>
      </transformations>
    </transformer>
  </transformers>";

            var factory = new FileEntryTransformerFactory(
                XmlReader.Create(new StringReader(config))
                , new FileEntryMatcherFactory()
                , new TransformationFactory()
                , new MatchingContextFactory()
                , new List<Delegate>()
                , "./somefolder"
                , "someprefix"
            );

            var transformers = factory.CreateTransformers("somefile.xml");
            Assert.IsNotNull(transformers);
            Assert.AreEqual(2, transformers.Count);
        }

        [Test]
        public void FileEntryTransformerFactory_throws_when_transformer_has_no_transformations()
        {
            var config = @"
  <transformers>
    <transformer>
      <matcher type=""regex"">rz(Curves|Curves_ZCIS|OIS).csv</matcher>
      <transformations>
      </transformations>
    </transformer>
  </transformers>";

            Assert.Throws<CodeContractViolationException>(() =>
                new FileEntryTransformerFactory(
                    XmlReader.Create(new StringReader(config))
                    , new FileEntryMatcherFactory()
                    , new TransformationFactory()
                    , new MatchingContextFactory()
                    , new List<Delegate>()
                    , "./somefolder"
                    , "someprefix"
                )
            );
        }
        #endregion
    }
}
