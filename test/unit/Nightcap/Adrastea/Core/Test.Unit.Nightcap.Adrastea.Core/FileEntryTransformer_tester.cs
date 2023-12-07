using Bud;
using global::Nightcap.Adrastea.Core;
using global::Nightcap.Adrastea.Core.Interfaces;
using global::System.Text;
using global::System.Xml;
using global::System.Xml.Linq;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;
using Nightcap.NET.Core.IO;

namespace Test.Unit.Nightcap.Adrastea.Core
{
    [TestFixture]
    internal sealed class FileEntryTransformer_tester
    {
        #region support classes
        
        class TestTransformation : ITransformation
        {
            private readonly string m_name;

            public TestTransformation(string name)
            {
                m_name = name;
            }

            public ITransformingContext Transform(ITransformingContext context, IEnumerable<Delegate> eventHandlers)
            {
                IStreamReaderTransformingContext streamContext;
                if (context.TryGetInterface(out streamContext))
                {
                    using (StreamReader reader = streamContext.CreateStreamReader())
                    {
                        string input = reader.ReadToEnd();
                        string output = input + "Transform" + m_name;

                        Stream stream = new MemoryStream();

                        using(StreamWriter writer = new StreamWriter(
                                  stream
                                , Encoding.UTF8
                                , 1024
                                , true
                            )
                        )
                        {
                            writer.Write(output);
                        }

                        stream.Seek(0, 0);

                        return TransformingContextUtil.CreateStreamReaderTransformingContext(stream);
                    }
                }
                else
                {
                    throw new ApplicationException("Does not support stream context");
                }
            }
        }

        class UnrelatedEventArgs : EventArgs 
        { }

        class TransformationFails : ITransformation
        {
            public ITransformingContext Transform(ITransformingContext context, IEnumerable<Delegate> eventHandlers)
            {
                throw new ApplicationException();
            }
        }

        class XmlTransformation : ITransformation
        {
            public ITransformingContext Transform(ITransformingContext context, IEnumerable<Delegate> eventHandlers)
            {
                XmlWriterSettings writerSettings = new XmlWriterSettings
                {
                    Indent = false
                };

                XmlReaderSettings readerSettings = new XmlReaderSettings
                {
                      CloseInput = true
                    , IgnoreWhitespace = true
                    , IgnoreProcessingInstructions = true
                };

                IStreamReaderTransformingContext tc;
                if (context.TryGetInterface(out tc))
                {
                    using (XmlReader sr = XmlReader.Create(tc.CreateStreamReader(), readerSettings))
                    {
                        Stream ms = new MemoryStream();
                        using (XmlWriter w = XmlWriter.Create(ms, writerSettings))
                        {
                            w.WriteStartDocument();
                            
                            while (sr.Read())
                            {
                                if (XmlNodeType.XmlDeclaration != sr.NodeType)
                                {
                                    w.WriteNode(sr, true);
                                }
                            }
                        }

                        ms.Seek(0, 0);

                        return TransformingContextUtil.CreateXmlReaderTransformingContext(XmlReader.Create(ms, readerSettings));
                    }
                }

                throw new Exception("Did not provide StreamReader input");
            }
        }

        class TransformExposesEventHandlers : ITransformation
        {
            public IEnumerable<Delegate> EventHandlers { get; set; }

            public ITransformingContext Transform(
                  ITransformingContext context
                , IEnumerable<Delegate> eventHandlers
            )
            {
                EventHandlers = eventHandlers;
                return TransformingContextUtil.CreateXmlReaderTransformingContext(new XDocument());
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
        public void Test_Constructor_throws_exception_for_null_input()
        {
            var transformations = new List<ITransformation>{ new TestTransformation("A") };
            
            Assert.Throws<CodeContractViolationException>(
                  () => new FileEntryTransformer(
                        null
                      , "somefile.txt"
                      , "./somefolder"
                      , "myprefix"
                      , new List<Delegate>()
                  )
            );
            Assert.Throws<CodeContractViolationException>(
                  () => new FileEntryTransformer(
                        transformations
                      , null
                      , "./somefolder"
                      , "myprefix"
                      , new List<Delegate>()
                  )
            );
            Assert.Throws<CodeContractViolationException>(
                  () => new FileEntryTransformer(
                        transformations
                      , "somefile.txt"
                      , null
                      , "myprefix"
                      , new List<Delegate>()
                  )
            );
            Assert.Throws<CodeContractViolationException>(
                  () => new FileEntryTransformer(
                        transformations
                      , "somefile.txt"
                      , "./somefolder"
                      , null
                      , new List<Delegate>()
                  )
            );
        }

        [Test]
        public void Test_Constructor_throws_when_transformations_empty()
        {
            Assert.Throws<CodeContractViolationException>(() =>
                  new FileEntryTransformer(
                        new List<ITransformation>()
                      , "somefile.txt"
                      , "./somefolder"
                      , "myprefix"
                      , null
                  )
            );
        }

        [Test]
        public void Test_Constructor_returns_non_null_object()
        {
            Assert.NotNull(
                  new FileEntryTransformer(
                        new List<ITransformation>{ new TestTransformation("A") }
                      , "somefile.txt"
                      , "./somefolder"
                      , "myprefix"
                      , new List<Delegate>()
                  )
            );
        }

        [Test]
        public void Test_Transform_processes_a_single_successful_transformation()
        {
            var transformations = new List<ITransformation> { new TestTransformation("A") };
            
            using (var transformInput = new TmpDir())
            {
                using (var loadInput = new TmpDir())
                {
                    var inputFile = transformInput.CreateFile(
                          "hello"
                        , "a.txt"
                    );
                    var outputFile = Path.Combine(
                          loadInput.Path
                        , "myprefix" + Path.GetFileName(inputFile)
                    );

                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));

                    var t = new FileEntryTransformer(
                          transformations
                        , inputFile
                        , loadInput.Path
                        , "myprefix"
                        , new List<Delegate>()
                    );
                    t.Transform();

                    Assert.IsTrue(File.Exists(outputFile));
                    Assert.AreEqual(
                          "helloTransformA"
                        , File.ReadAllText(outputFile)
                    );
                }
            }
        }

        [Test]
        public void Test_Transform_processes_multiple_successful_transformations()
        {
            var transformations = new List<ITransformation> 
            {
                  new TestTransformation("A")
                , new TestTransformation("B")
            };
            
            using (var transformInput = new TmpDir())
            {
                using (var loadInput = new TmpDir())
                {
                    var inputFile = transformInput.CreateFile("hello", "a.txt");
                    var outputFile = Path.Combine(
                          loadInput.Path
                        , "myprefix" + Path.GetFileName(inputFile)
                    );

                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));

                    var t = new FileEntryTransformer(
                          transformations
                        , inputFile
                        , loadInput.Path
                        , "myprefix"
                        , new List<Delegate>()
                    );

                    t.Transform();

                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsTrue(File.Exists(outputFile));
                    Assert.AreEqual(
                          "helloTransformATransformB"
                        , File.ReadAllText(outputFile)
                    );
                }
            }
        }

        [Test]
        public void Test_Transform_does_not_process_file_when_a_sharing_violation_exception_is_thrown()
        {
            var transformations = new List<ITransformation> 
            {
                  new TestTransformation("A")
                , new TestTransformation("B")
            };
            
            using (var transformInput = new TmpDir())
            {
                using (var loadInput = new TmpDir())
                {
                    var inputFile = transformInput.CreateFile(
                          "hello"
                        , "a.txt"
                    );
                    var outputFile = Path.Combine(
                          loadInput.Path
                        , "myprefix" + Path.GetFileName(inputFile)
                    );

                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));

                    var t = new FileEntryTransformer(
                          transformations
                        , inputFile
                        , loadInput.Path
                        , "myprefix"
                        , new List<Delegate>()
                    );

                    // lock the input file in exclusive mode
                    using(var lockedInputFile = File.Open(
                                inputFile
                              , FileMode.Open
                              , FileAccess.ReadWrite
                              , FileShare.None
                          )
                    )
                    {
                        Assert.Throws<SharingViolationException>(
                              () => t.Transform()
                        );
                    }

                    // Make sure that the input file wasn't moved
                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));
                    Assert.AreEqual(
                          "hello"
                        , File.ReadAllText(inputFile)
                    );
                }
            }
        }

        [Test]
        public void Test_Transform_throws_exception_when_transform_fails()
        {
            var transformations = new List<ITransformation> 
            {
                  new TestTransformation("A")
                , new TransformationFails()
            };
            
            using (var transformInput = new TmpDir())
            {
                using (var loadInput = new TmpDir())
                {
                    var inputFile = transformInput.CreateFile(
                          "hello"
                        , "a.txt"
                    );
                    var outputFile = Path.Combine(
                          loadInput.Path
                        , "myprefix" + Path.GetFileName(inputFile)
                    );

                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));

                    var t = new FileEntryTransformer(
                          transformations
                        , inputFile
                        , loadInput.Path
                        , "myprefix"
                        , new List<Delegate>()
                    );

                    Assert.Throws<ApplicationException>(
                          () => t.Transform()
                    );

                    // Make sure that the input file wasn't moved
                    Assert.IsTrue(File.Exists(inputFile));
                    Assert.IsFalse(File.Exists(outputFile));
                    Assert.AreEqual(
                          "hello"
                        , File.ReadAllText(inputFile)
                    );
                }
            }
        }

        [Test]
        public void Test_Transform_correctly_formats_xml_output()
        {
            string input = 
@"<?xml version=""1.0"" encoding=""utf-8""?><root><SomeElement>
<SomeOtherElement>Some Value</SomeOtherElement>
</SomeElement></root>";

            string expected = 
@"<?xml version=""1.0"" encoding=""utf-8""?>
<root>
  <SomeElement>
    <SomeOtherElement>Some Value</SomeOtherElement>
  </SomeElement>
</root>";

            var transformations = new List<ITransformation> { new XmlTransformation() };

            using (TmpDir transformInput = new TmpDir())
            using (TmpDir loadInput = new TmpDir())
            {
                var inputFile = transformInput.CreateFile(
                      input
                    , "a.txt"
                );

                var outputFile = Path.Combine(
                      loadInput.Path
                    , "myprefix" + Path.GetFileName(inputFile)
                );

                Assert.IsTrue(File.Exists(inputFile));
                Assert.IsFalse(File.Exists(outputFile));

                var t = new FileEntryTransformer(
                      transformations
                    , inputFile
                    , loadInput.Path
                    , "myprefix"
                    , new Delegate[] {}
                );

                t.Transform();

                Assert.IsTrue(File.Exists(inputFile));
                Assert.IsTrue(File.Exists(outputFile));
                Assert.AreEqual(
                      expected
                    , File.ReadAllText(outputFile)
                );
            }
        }
        #endregion
    }
}

