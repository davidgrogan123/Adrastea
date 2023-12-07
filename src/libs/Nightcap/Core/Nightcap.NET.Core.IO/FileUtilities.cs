using Nightcap.Core.Diagnostics.CodeContract;

namespace Nightcap.NET.Core.IO
{
    /// <summary>
    ///  Helper methods for file input/output.
    /// </summary>
    /// <remarks>
    ///  TODO: This should be hidden behind an interface to allow mocking.
    /// </remarks>
    public static class FileUtilities
    {
        #region constants

        private static class Constants
        {
            // replacement error messages for common situations where the
            // traditional io file exceptions don't provide enough info
            internal const string FailureMessage_FilePathInvalidCharacters_2 =
                "File path '{0}' contains one or more invalid characters. {1}";
            internal const string FailureMessage_FileSharingViolation_1 =
                "The process cannot access the file because it is being used by "
                + "another process. {0}";
            internal const string FailureMessage_MoveFilePathInvalidCharacters_3 =
                "The file '{0}' could not be moved to '{1}' as one or more invalid "
                + "characters were encountered in the path. {2}";
            internal const string FailureMessage_DestinationFileAlreadyExists_2 =
                "The destination file '{0}' already exists. {1}";

            // Windows system HRESULT values; for listing of all HRESULT values
            // see the following website:
            // https://msdn.microsoft.com/en-us/library/cc704587.aspx
            // 
            // NB: these are usually stored as signed integers rather than
            // unsigned integers within the .NET framework
            internal enum HResult
            {
                // The process cannot access the file because it is being used
                // by another process.
                ErrorSharingViolation = -2147024864

                // Cannot create a file when that file already exists.    
                , ErrorFileExists = -2147024713
            }
        }
        #endregion

        #region methods

        private static bool IsExceptionHResultEqualTo(
              Exception x
            , Constants.HResult hresult
        )
        {
            // returns true if the internal HResult property for the given
            // exception is equal to hresult; false otherwise

            return Convert.ToInt32(hresult) == x.HResult;
        }

        /// <summary>
        ///  Opens a file as a stream for reading.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for reading. Must not be <c>null</c>,
        ///  empty, or whitespace.
        /// </param>
        /// <returns>A StreamReader on the specified path.</returns>
        /// <exception>TBD</exception>
        public static StreamReader OpenText(
              string filePath
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            try
            {
                return File.OpenText(filePath);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Opens a file, reads the contents of the file into a byte array, and
        ///  then closes the file.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for reading. Must not be <c>null</c>,
        ///  empty, or whitespace.
        /// </param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <exception>TBD</exception>
        public static byte[] ReadAllBytes(
              string filePath
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            try
            {
                return File.ReadAllBytes(filePath);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Opens a file, reads each line of the file into a string array, and
        ///  then closes the file.
        /// </summary>
        /// <param name="fileEntry">
        ///  The file to be opened for reading. Must not be <c>null</c>.
        /// </param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <exception cref="System.IO.PathTooLongException">
        ///  Thrown when the path or file name is longer than the system-defined
        ///  maximum length.
        /// </exception>
        /// <exception cref="Nightcap.Core.IO.SharingViolationException">
        ///  Thrown when a process cannot access a file because it is being used
        ///  by another process.
        /// </exception>
        public static string[] ReadAllLines(
              string filePath
        )
        {
            Requires.ThatArgumentIsNotNull(filePath, "fileEntry");

            try
            {
                return File.ReadAllLines(filePath);
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Creates a new file, writes the specified byte array to the file,
        ///  and then closes the file. If the target file already exists, it is
        ///  overwritten.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for writing. Must not be <c>null</c>,
        ///  empty, or whitespace.
        /// </param>
        /// <param name="bytes">
        ///  The bytes to write to the file. Must not be <c>null</c>.
        /// </param>
        /// <returns>A byte array containing the contents of the file.</returns>
        /// <exception>TBD</exception>
        public static void WriteAllBytes(
              string filePath
            , byte[] bytes
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");
            Requires.ThatArgumentIsNotNull(bytes, "bytes");

            try
            {
                File.WriteAllBytes(filePath, bytes);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Moves a specified file to a new location, providing the option to
        ///  specify a new file name.
        /// </summary>
        /// <param name="source">
        ///  The file to move. Must not be <c>null</c>,
        ///  empty, or whitespace.
        /// </param>
        /// <param name="destination">
        ///  The new path and name for the file. Must not be <c>null</c>,
        ///  empty, or whitespace.
        /// </param>
        /// <exception>TBD</exception>
        public static void Move(
              string source
            , string destination
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(source, "source");
            Requires.ThatArgumentIsNotNullOrWhiteSpace(destination, "destination");

            try
            {
                File.Move(source, destination);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_MoveFilePathInvalidCharacters_3
                          , source
                          , destination
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorFileExists
                    ))
                {
                    // TODO: determine correct exception type
                    throw new ApplicationException(
                          String.Format(
                                Constants.FailureMessage_DestinationFileAlreadyExists_2
                              , destination
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Deletes a file.
        /// </summary>
        /// <remarks>
        ///  If the file to be deleted does not exist, no exception is thrown.
        /// </remarks>
        /// <param name="filePath">
        ///  The file to delete. Must not be <c>null</c>, empty, or whitespace.
        /// </param>
        /// <exception>TBD</exception>
        public static void Delete(
              string filePath
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            try
            {
                File.Delete(filePath);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Opens a file as a readonly stream.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for reading. May not be <c>null</c>.
        /// </param>
        /// <returns>
        ///  A readonly Stream on the specified path.
        /// </returns>
        /// <exception>TBD</exception>
        public static Stream CreateReadableStream(
            string filePath
        )
        {
            Requires.ThatArgumentIsNotNull(filePath, "fileEntry");

            try
            {
                return File.OpenRead(filePath);
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Creates and opens a file as a stream with write access. If the 
        ///  target file already exists, it is overwritten.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for writing. May not be <c>null</c> or only 
        ///  whitespace.
        /// </param>
        /// <returns>
        ///  A Stream on the specified path.
        /// </returns>
        /// <exception>TBD</exception>
        public static Stream CreateWritableStream(
            string filePath
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            try
            {
                return new FileStream(
                      filePath
                    , FileMode.Create
                    , FileAccess.Write
                    , FileShare.None
                );
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }

        /// <summary>
        ///  Creates a new file, writes the specified text to the file, and 
        ///  then closes the file. If the target file already exists, it is
        ///  overwritten.
        /// </summary>
        /// <param name="filePath">
        ///  The file to be opened for writing. May not be <c>null</c> or 
        ///  only whitespace.
        /// </param>
        /// <param name="text">
        ///  The text to write to the file.
        /// </param>
        /// <exception>TBD</exception>
        public static void WriteAllText(
              string filePath
            , string text
        )
        {
            Requires.ThatArgumentIsNotNullOrWhiteSpace(filePath, "filePath");

            try
            {
                File.WriteAllText(filePath, text);
            }
            catch (ArgumentException x)
            {
                // TODO: determine correct exception type
                throw new ApplicationException(
                      String.Format(
                            Constants.FailureMessage_FilePathInvalidCharacters_2
                          , filePath
                          , x.Message
                      )
                    , x
                );
            }
            catch (IOException x)
            {
                if (IsExceptionHResultEqualTo(
                          x
                        , Constants.HResult.ErrorSharingViolation
                    ))
                {
                    // TODO: determine correct exception type
                    throw new SharingViolationException(
                          String.Format(
                                Constants.FailureMessage_FileSharingViolation_1
                              , x.Message
                          )
                        , x
                    );
                }

                throw;
            }
        }
        #endregion
    }
}
