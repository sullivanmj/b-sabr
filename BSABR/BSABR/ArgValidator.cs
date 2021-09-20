// Copyright © 2021 Matt Sullivan

using System;
using System.IO;
using System.Text;

namespace Bsabr
{
    /// <summary>
    /// Validates options for the process instance.
    /// </summary>
    class ArgValidator
    {
        private readonly Options _options;
        private readonly string _errorMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgValidator"/> class.
        /// </summary>
        /// <param name="options">The <see cref="Options"/> to validate.</param>
        public ArgValidator(Options options)
        {
            _options = options;

            if (!_options.HelpRequested)
            {
                _errorMessage = GetErrorMessage();
            }
        }

        /// <summary>
        /// Summary of what is wrong with the options.
        /// </summary>
        public string ErrorMessage => _errorMessage;

        /// <summary>
        /// Indicates whether or not the options for this <see cref="ArgValidator"/> are valid.
        /// </summary>
        public bool OptionsAreValid => _errorMessage == null;

        /// <summary>
        /// Determines whether an <see cref="Options"/> is suitable for the program to execute.
        /// </summary>
        /// <param name="options">The configuration to validate.</param>
        /// <returns>An error message, if an error occurred. <c>null</c> otherwise.</returns>
        private string GetErrorMessage()
        {
            var errorBuilder = new StringBuilder();

            var studioExecutableErrorMessage = GetStudioExecutableErrorString();

            if (studioExecutableErrorMessage != null)
            {
                errorBuilder.AppendLine($"\t- {studioExecutableErrorMessage}");
            }

            var outFileNameErrorMessage = GetOutFileNameErrorString();

            if (outFileNameErrorMessage != null)
            {
                errorBuilder.AppendLine($"\t- {outFileNameErrorMessage}");
            }

            if (errorBuilder.Length > 0)
            {
                return $"The following error(s) occurred: \n{errorBuilder}";
            }

            return null;
        }

        /// <summary>
        /// Gets an error string based on the specified output filename.
        /// </summary>
        /// <returns>An error message if the argument is invalid, <c>null</c> otherwise.</returns>
        private string GetOutFileNameErrorString()
        {
            if (_options.OutFileName == null)
            {
                if (_options.OutFileSpecified)
                {
                    return "The \"/out\" argument should be followed by a valid file path.";
                }

                return null;
            }

            try
            {
                var attributes = File.GetAttributes(_options.OutFileName);
            }
            catch (ArgumentException)
            {
                return "Invalid output file path. Path is empty, contains only white spaces, or contains invalid characters.";
            }
            catch (PathTooLongException)
            {
                return "Output file path is too long. File paths should be less than 260 characters.";
            }
            catch (NotSupportedException)
            {
                return "Output file path is in an invalid format.";
            }
            catch (FileNotFoundException)
            {
                // Not a problem in this scenario as we will be creating the file
            }
            catch (DirectoryNotFoundException)
            {
                return "Output file path represents a directory and is invalid, such as being on an unmapped drive, or the directory cannot be found.";
            }
            catch (IOException)
            {
                return "Output file path is to a file that is in use by another process.";
            }
            catch (UnauthorizedAccessException)
            {
                return "You do not have permission to access the file specified as the output file path argument.";
            }

            return null;
        }

        /// <summary>
        /// Gets an error string based on the specified path to AtmelStudio.exe.
        /// </summary>
        /// <returns>An error message if the argument is invalid, <c>null</c> otherwise.</returns>
        private string GetStudioExecutableErrorString()
        {
            bool fileExists = File.Exists(_options.StudioExecutable);

            if (!fileExists)
            {
                // see if it exists on the path somewhere
                foreach (var evt in Enum.GetValues(typeof(EnvironmentVariableTarget)))
                {
                    var paths = Environment.GetEnvironmentVariable("PATH", (EnvironmentVariableTarget)evt).Split(';');

                    foreach (var path in paths)
                    {
                        if (File.Exists(_options.StudioExecutable))
                        {
                            fileExists = true;
                            break;
                        }
                    }
                }

                return "The specified Atmel Studio/Microchip Studio executable does not exist. Make sure you have entered the path correctly and that you have permission to access the executable.";
            }

            return null;
        }
    }
}
