using Bud;
using Nightcap.Core.Diagnostics.CodeContract;
using Nightcap.Core.Diagnostics.CodeContract.Exceptions;
using Nightcap.NET.Core.IO;
using System.Text;

namespace Test.Unit.Nightcap.Core.IO
{
    [TestFixture]
    internal sealed class FileUtilities_tester
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
        public void OpenText_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.OpenText(null));
        }

        [Test]
        public void OpenText_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.OpenText(@""));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.OpenText(" "));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.OpenText("\t\n\r"));
        }

        [Test]
        public void OpenText_throws_exception_when_file_not_found()
        {
            using (var dir = new TmpDir())
            {
                var destination = Path.Combine(dir.Path, "newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<FileNotFoundException>(() => FileUtilities.OpenText(destination));
            }
        }


        [Test]
        public void OpenText_throws_exception_when_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<DirectoryNotFoundException>(() => FileUtilities.OpenText(destination));
            }
        }

        [Test]
        public void OpenText_throws_exception_when_path_is_too_long()
        {
            var filePath = new String(
                  'f'
                , 32767
            );
            Assert.Throws<PathTooLongException>(() => FileUtilities.OpenText(filePath));
        }

        [Test]
        public void OpenText_opens_empty_text_file()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateEmptyFile("a.txt");

                using (var s = FileUtilities.OpenText(fileA))
                {
                    Assert.AreEqual(s.ReadLine(), null);
                }
            }
        }

        [Test]
        public void OpenText_opens_and_reads_simple_text_file()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateFile("hello", "a.txt");

                using (var s = FileUtilities.OpenText(fileA))
                {
                    Assert.AreEqual(s.ReadLine(), "hello");
                    Assert.AreEqual(s.ReadLine(), null);
                }
            }
        }

        [Test]
        public void OpenText_supports_nested_opens()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateFile(";oaijwef;oijm32p90;jaq230RI1]=A;WFEIJ0P9", "a.txt");

                using (var s = FileUtilities.OpenText(fileA))
                {
                    Assert.AreEqual(s.ReadLine(), ";oaijwef;oijm32p90;jaq230RI1]=A;WFEIJ0P9");
                    Assert.AreEqual(s.ReadLine(), null);

                    using (var s2 = FileUtilities.OpenText(fileA))
                    {
                        Assert.AreEqual(s2.ReadLine(), ";oaijwef;oijm32p90;jaq230RI1]=A;WFEIJ0P9");
                        Assert.AreEqual(s2.ReadLine(), null);


                        using (var s3 = FileUtilities.OpenText(fileA))
                        {
                            Assert.AreEqual(s3.ReadLine(), ";oaijwef;oijm32p90;jaq230RI1]=A;WFEIJ0P9");
                            Assert.AreEqual(s3.ReadLine(), null);
                        }
                    }
                }
            }
        }

        [Test]
        public void Move_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move(null, null));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("somefile.txt", null));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move(null, "somefile.txt"));
        }

        [Test]
        public void Move_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("", "somefile.txt"));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move(" ", "somefile.txt"));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("\t\n\r", "somefile.txt"));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("somefile.txt", ""));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("somefile.txt", " "));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Move("somefile.txt", "\t\n\r"));
        }

        [Test]
        public void Move_throws_exception_when_file_not_found()
        {
            Assert.Throws<FileNotFoundException>(() => FileUtilities.Move("somefile.txt", "somefile2.txt"));
        }

        [Test]
        public void Move_throws_exception_when_source_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));

                // move should throw a DirectoryNotFoundException in this case,
                // which would be consistent with the file open methods, however
                // instead it throws a FileNotFoundException
                Assert.Throws<FileNotFoundException>(
                      () => FileUtilities.Move(
                            destination
                          , "somefile.txt"
                      )
                );
            }
        }

        [Test]
        public void Move_throws_exception_when_destination_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateFile("hello", "a.txt");
                Assert.Throws<DirectoryNotFoundException>(
                      () => FileUtilities.Move(
                            fileA
                          , ".\\hello\\somefile.txt"
                      )
                );
            }
        }

        [Test]
        public void Move_throws_exception_when_path_is_too_long()
        {
            var longFilePath = new String(
                  'f'
                , 32767
            );

            // source path too long
            Assert.Throws<PathTooLongException>(
                  () => FileUtilities.Move(
                        longFilePath
                      , "somefile.txt"
                  )
            );

            // destination path too long
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateFile("hello", "a.txt");
                Assert.Throws<PathTooLongException>(
                      () => FileUtilities.Move(
                            fileA
                          , longFilePath
                      )
                );
            }
        }

        [Test]
        public void Move_throws_exception_when_destination_file_already_exists()
        {
            using (var dir = new TmpDir())
            {
                var source = dir.CreateFile("hello", "source.txt");
                var destination = dir.CreateFile("hello", "destination.txt");
                Assert.Throws<ApplicationException>(
                      () => FileUtilities.Move(
                            source
                          , destination
                      )
                );
            }
        }

        [Test]
        public void Move_throws_exception_when_source_file_is_locked()
        {
            using (var dir = new TmpDir())
            {
                var source = dir.CreateFile("hello", "source.txt");
                var destination = Path.Combine(
                      Path.GetDirectoryName(source)
                    , "destination.txt"
                );

                // lock the source file
                using (var f = File.Open(
                             source
                           , FileMode.Open
                           , FileAccess.ReadWrite
                           , FileShare.None
                       )
                )
                {
                    Assert.Throws<SharingViolationException>(
                          () => FileUtilities.Move(
                                source
                              , destination
                          )
                    );
                }
            }
        }

        [Test]
        public void Move_correctly_moves_a_file()
        {
            using (var dir = new TmpDir())
            {
                var source = dir.CreateFile("hello", "source.txt");
                var destination = Path.Combine(
                      Path.GetDirectoryName(source)
                    , "destination.txt"
                );

                Assert.IsTrue(File.Exists(source));
                Assert.IsFalse(File.Exists(destination));

                FileUtilities.Move(source, destination);

                Assert.IsFalse(File.Exists(source));
                Assert.IsTrue(File.Exists(destination));
                Assert.IsTrue(File.ReadAllText(destination) == "hello");
            }
        }

        [Test]
        public void ReadAllBytes_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.ReadAllBytes(null));
        }

        [Test]
        public void ReadAllBytes_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.ReadAllBytes(@""));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.ReadAllBytes(" "));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.ReadAllBytes("\t\n\r"));
        }

        [Test]
        public void ReadAllBytes_throws_exception_when_file_not_found()
        {
            Assert.Throws<FileNotFoundException>(() => FileUtilities.ReadAllBytes(@"s"));
            Assert.Throws<FileNotFoundException>(() => FileUtilities.ReadAllBytes(@"somefile.txt"));
        }

        [Test]
        public void ReadAllBytes_throws_exception_when_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<DirectoryNotFoundException>(
                      () => FileUtilities.ReadAllBytes(
                            destination
                      )
                );
            }
        }

        [Test]
        public void ReadAllBytes_throws_exception_when_path_is_too_long()
        {
            var filePath = new String(
                  'f'
                , 32767
            );
            Assert.Throws<PathTooLongException>(() => FileUtilities.ReadAllBytes(filePath));
        }

        [Test]
        public void ReadAllBytes_reads_an_empty_file()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateEmptyFile("a.txt");
                var bytes = FileUtilities.ReadAllBytes(fileA);
                Assert.That(bytes.Length == 0);
            }
        }

        [Test]
        public void ReadAllBytes_reads_a_non_empty_file()
        {
            using (var dir = new TmpDir())
            {
                var fileA = dir.CreateFile("o;acijmp;0oc9j[a'90k]SKI]", "a.txt");
                var bytes = FileUtilities.ReadAllBytes(fileA);
                Assert.AreEqual(Encoding.ASCII.GetString(bytes), "o;acijmp;0oc9j[a'90k]SKI]");
            }
        }

        [Test]
        public void WriteAllBytes_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.WriteAllBytes(null, new byte[1]));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.WriteAllBytes("file", null));
        }

        [Test]
        public void WriteAllBytes_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.WriteAllBytes(@"", new byte[1]));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.WriteAllBytes(" ", new byte[1]));
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.WriteAllBytes("\t\n\r", new byte[1]));

        }

        [Test]
        public void WriteAllBytes_throws_exception_when_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<DirectoryNotFoundException>(
                      () => FileUtilities.WriteAllBytes(
                            destination
                          , new byte[1]
                      )
                );
            }
        }

        [Test]
        public void WriteAllBytes_throws_exception_when_path_is_too_long()
        {
            var filePath = new String(
                  'f'
                , 32767
            );
            Assert.Throws<PathTooLongException>(() => FileUtilities.WriteAllBytes(filePath, new byte[1]));
        }

        [Test]
        public void WriteAllBytes_writes_an_empty_file()
        {
            using (var dir = new TmpDir())
            {
                var filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                var bytes = new byte[0];
                FileUtilities.WriteAllBytes(filePath, bytes);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(File.ReadAllText(filePath) == "");
            }
        }

        [Test]
        public void WriteAllBytes_writes_a_non_empty_file()
        {
            using (var dir = new TmpDir())
            {
                var filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                var bytes = Encoding.ASCII.GetBytes(";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k");
                FileUtilities.WriteAllBytes(filePath, bytes);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(File.ReadAllText(filePath) == ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k");
            }
        }

        [Test]
        public void Delete_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Delete(null));
        }

        [Test]
        public void Delete_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(() => FileUtilities.Delete(""));
        }

        [Test]
        public void Delete_does_not_throw_an_exception_when_file_not_found()
        {
            using (var dir = new TmpDir())
            {
                var filePath = Path.Combine(dir.Path, @"\newfile.txt");
                Assert.IsFalse(File.Exists(filePath));

                FileUtilities.Delete(filePath);

                Assert.IsFalse(File.Exists(filePath));
            }
        }

        [Test]
        public void Delete_throws_exception_when_directory_not_found()
        {
            using (var dir = new TmpDir())
            {
                var filePath = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(filePath));

                Assert.Throws<DirectoryNotFoundException>(
                      () => FileUtilities.Delete(
                            filePath
                      )
                );
            }
        }

        [Test]
        public void Delete_throws_exception_when_path_is_too_long()
        {
            var longFilePath = new String(
                  'f'
                , 32767
            );

            Assert.Throws<PathTooLongException>(
                  () => FileUtilities.Delete(
                        longFilePath
                  )
            );
        }

        [Test]
        public void Delete_throws_exception_when_file_is_locked()
        {
            using (var dir = new TmpDir())
            {
                var filePath = dir.CreateFile("hello", "source.txt");

                // lock the source file
                using (var f = File.Open(
                             filePath
                           , FileMode.Open
                           , FileAccess.ReadWrite
                           , FileShare.None
                       )
                )
                {
                    Assert.Throws<SharingViolationException>(
                          () => FileUtilities.Delete(
                                filePath
                          )
                    );
                }
            }
        }

        [Test]
        public void Delete_deletes_an_existing_file()
        {
            using (var dir = new TmpDir())
            {
                var filePath = dir.CreateFile("hello", "source.txt");

                Assert.IsTrue(File.Exists(filePath));

                FileUtilities.Delete(
                      filePath
                );

                Assert.IsFalse(File.Exists(filePath));
            }
        }

        [Test]
        public void FileUtilities_CreateReadableStream_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.CreateReadableStream(null)
            );
        }

        [Test]
        public void FileUtilities_CreateWritableStream_throws_exception_for_null_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.CreateWritableStream(null)
            );
        }

        [Test]
        public void FileUtilities_CreateWritableStream_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.CreateWritableStream(@"")
            );
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.CreateWritableStream(" ")
            );
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.CreateWritableStream("\t\n\r")
            );

        }


        [Test]
        public void FileUtilities_CreateWritableStream_throws_exception_when_directory_not_found()
        {
            using (TmpDir dir = new TmpDir())
            {
                string destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<DirectoryNotFoundException>(
                    () => FileUtilities.CreateWritableStream(destination)
                );
            }
        }

        [Test]
        public void FileUtilities_CreateWritableStream_throws_exception_when_path_is_too_long()
        {
            string filePath = new string(
                  'f'
                , 32767
            );
            Assert.Throws<PathTooLongException>(
                () => FileUtilities.CreateWritableStream(filePath)
            );
        }

        [Test]
        public void FileUtilities_CreateWritableStream_throws_exception_when_file_is_locked()
        {
            using (TmpDir dir = new TmpDir())
            {
                string filePath = dir.CreateEmptyFile("newfile.txt");

                Assert.True(File.Exists(filePath));

                // lock the source file
                using (var f = File.Open(
                             filePath
                           , FileMode.Open
                           , FileAccess.ReadWrite
                           , FileShare.None
                       )
                )
                {
                    Assert.Throws<SharingViolationException>(
                          () => FileUtilities.CreateWritableStream(filePath)
                    );
                }
            }
        }

        [Test]
        public void FileUtilities_CreateWritableStream_writes_an_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                using (Stream s = FileUtilities.CreateWritableStream(filePath))
                { }

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(string.Empty == File.ReadAllText(filePath));
            }
        }

        [Test]
        public void FileUtilities_CreateWritableStream_writes_a_non_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string content = ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k";
                string filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                using (Stream s = FileUtilities.CreateWritableStream(filePath))
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(content);
                }

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(content == File.ReadAllText(filePath));
            }
        }

        [Test]
        public void FileUtilities_CreateWritableStream_overwrites_a_non_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string content = ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k";
                string filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                using (Stream s = FileUtilities.CreateWritableStream(filePath))
                using (StreamWriter w = new StreamWriter(s))
                {
                    w.Write(content);
                }

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(content == File.ReadAllText(filePath));

                using (Stream s = FileUtilities.CreateWritableStream(filePath))
                { }

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(string.Empty == File.ReadAllText(filePath));
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_throws_exception_for_null_path()
        {
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.WriteAllText(null, "abc")
            );
        }

        [Test]
        public void FileUtilities_WriteAllText_throws_exception_for_whitespace_input()
        {
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.WriteAllText(@"", "abc")
            );
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.WriteAllText(" ", "abc")
            );
            Assert.Throws<CodeContractViolationException>(
                () => FileUtilities.WriteAllText("\t\n\r", "abc")
            );
        }

        [Test]
        public void FileUtilities_WriteAllText_throws_exception_when_directory_not_found()
        {
            using (TmpDir dir = new TmpDir())
            {
                string destination = Path.Combine(dir.Path, @"\nonexistentdir\newfile.txt");
                Assert.IsFalse(File.Exists(destination));
                Assert.Throws<DirectoryNotFoundException>(
                    () => FileUtilities.WriteAllText(destination, "abc")
                );
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_throws_exception_when_path_is_too_long()
        {
            string filePath = new string(
                  'f'
                , 32767
            );
            Assert.Throws<PathTooLongException>(
                () => FileUtilities.WriteAllText(filePath, "abc")
            );
        }

        [Test]
        public void FileUtilities_WriteAllText_throws_exception_when_file_is_locked()
        {
            using (TmpDir dir = new TmpDir())
            {
                string filePath = dir.CreateEmptyFile("newfile.txt");

                Assert.True(File.Exists(filePath));

                // lock the source file
                using (var f = File.Open(
                             filePath
                           , FileMode.Open
                           , FileAccess.ReadWrite
                           , FileShare.None
                       )
                )
                {
                    Assert.Throws<SharingViolationException>(
                          () => FileUtilities.WriteAllText(filePath, "abc")
                    );
                }
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_writes_an_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string filePathEmptyString = Path.Combine(dir.Path, "emptyString.txt");
                string filePathNull = Path.Combine(dir.Path, "null.txt");

                Assert.IsFalse(File.Exists(filePathEmptyString));
                Assert.IsFalse(File.Exists(filePathNull));

                FileUtilities.WriteAllText(filePathEmptyString, string.Empty);
                FileUtilities.WriteAllText(filePathNull, null);

                Assert.IsTrue(File.Exists(filePathEmptyString));
                Assert.IsTrue(File.Exists(filePathNull));

                Assert.IsTrue(string.Empty == File.ReadAllText(filePathEmptyString));
                Assert.IsTrue(string.Empty == File.ReadAllText(filePathNull));
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_writes_a_non_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string content = ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k";
                string filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                FileUtilities.WriteAllText(filePath, content);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(content == File.ReadAllText(filePath));
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_overwrites_a_non_empty_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string content = ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k";
                string filePath = Path.Combine(dir.Path, "newfile.txt");

                Assert.IsFalse(File.Exists(filePath));

                FileUtilities.WriteAllText(filePath, content);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(content == File.ReadAllText(filePath));

                FileUtilities.WriteAllText(filePath, string.Empty);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(string.Empty == File.ReadAllText(filePath));
            }
        }

        [Test]
        public void FileUtilities_WriteAllText_writes_a_multi_line_file()
        {
            using (TmpDir dir = new TmpDir())
            {
                string filePath = Path.Combine(dir.Path, "newfile.txt");
                int repetitions = 10;
                string line = ";aoijcfep;o09jacp;[ j;[oz0pkj 9['0k";
                string content = string.Join(
                    "\r\n"
                    , Enumerable.Repeat(line, repetitions)
                );

                Assert.IsFalse(File.Exists(filePath));

                FileUtilities.WriteAllText(filePath, content);

                Assert.IsTrue(File.Exists(filePath));
                Assert.IsTrue(content == File.ReadAllText(filePath));
            }
        }
        #endregion
    }
}