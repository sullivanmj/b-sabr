// Copyright © 2021 Matt Sullivan

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Bsabr
{
    class Program
    {
        // regex for the file finished string is a match when there are any number of characters,
        // and the final line matches with equals signs, Build succeeded/up-to-date, failed, an
        // skipped count all have one or more decimal, and there are closing equal signs.
        // This is the end of the string.
        private static readonly Regex _buildFinishedRegex = new Regex(@".*========== Build: \d+ succeeded or up-to-date, \d+ failed, \d+ skipped ==========");

        static void Main(string[] args)
        {
            var (shouldContinue, options) = ParseArguments(args);

            if (!shouldContinue)
            {
                return;
            }

            // we need to specify an out file if the user didn't specify one
            var outFileName = options.OutFileName ?? Path.GetTempFileName();

            // start with an empty file
            if (File.Exists(outFileName))
            {
                File.Delete(outFileName);
            }

            // we need an output file - that's how this whole thing works! if the user didn't specify one, we will 
            // add an argument for it to the arguments that we pass to AtmelStudio.exe
            var studioArgs = options.OutFileSpecified ? options.StudioArgs : $"{options.StudioArgs} /out {outFileName}";

            RunBuild(options.StudioExecutable, studioArgs, outFileName);

            // clean up if the user didn't specify that they wanted an output file of the build
            if (!options.OutFileSpecified)
            {
                DeleteOutFile(outFileName);
            }
        }

        /// <summary>
        /// Parses the command line arguments, validating them, indicates the options that the caller
        /// selected and whether or not to continue execution of the program.
        /// </summary>
        /// <param name="args">Command line arguments passed to the program.</param>
        /// <returns>A <see cref="Tuple{bool, Options}"/> indicating whether execution of the program should continue,
        /// and if so, which execution options the <paramref name="args"/> specify.</returns>
        private static (bool shouldContinue, Options options) ParseArguments(string[] args)
        {
            var argParser = ArgParser.InitializeNew(args);
            var options = argParser.GetOptions();
            var validator = new ArgValidator(options);

            var shouldShowHelp = options.HelpRequested;

            if (!validator.OptionsAreValid)
            {
                Console.WriteLine(validator.ErrorMessage);
                shouldShowHelp = true;
            }

            if (shouldShowHelp)
            {
                Console.WriteLine("Usage: Bsabr.exe studio_path studio_arguments\n"
                                + "\tstudio_path: the path to AtmelStudio.exe"
                                + "\tstudio_arguments: the build arguments to pass to AtmelStudio.exe"
                                + "If you are unsure of which arguments you should use for Atmel Studio"
                                + "/Microchip Studio, launch it from the command line with the \"/?\""
                                + "argument to see its usage");

                return (false, options);
            }

            return (true, options);
        }

        /// <summary>
        /// Runs the build, forwarding all build output to the console and effectively blocking until the build is finished.
        /// </summary>
        /// <param name="studioExecutablePath">The path to AtmelStudio.exe.</param>
        /// <param name="studioArgs">Arguments to be passed to AtmelStudio.exe.</param>
        /// <param name="outFilePath">The path to the file where build output will be saved.</param>
        private static void RunBuild(string studioExecutablePath, string studioArgs, string outFilePath)
        {
            // this is where the magic happens. we simply monitor the build output
            // as it is written to a file and forward that to the console.
            // we know when the build is done because studio always prints the
            // same format of build summary at the end

            using (var logFileStream = new FileStream(outFilePath, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite))
            using (var logFileStreamReader = new StreamReader(logFileStream))
            {
                var studioProcess = Process.Start(new ProcessStartInfo
                {
                    FileName = studioExecutablePath,
                    Arguments = studioArgs,
                });

                var buildOutputContent = new StringBuilder();

                while (!_buildFinishedRegex.IsMatch(buildOutputContent.ToString()))
                {
                    if (!logFileStreamReader.EndOfStream)
                    {
                        var currentString = logFileStreamReader.ReadToEnd();

                        if (!string.IsNullOrEmpty(currentString))
                        {
                            buildOutputContent.Append(currentString);
                            Console.Write(currentString);
                        }
                    }

                    if (studioProcess.HasExited && studioProcess.ExitCode != 0)
                    {
                        // during a normal build, studio will have an exit code of 0.
                        // but if an error occurred, let's stop waiting for a build to finish
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the out file.
        /// </summary>
        /// <param name="outFilePath"></param>
        private static void DeleteOutFile(string outFilePath)
        {
            var fileDeleted = false;
            var errorDeletingFile = false;
            var tryDeleteStartTime = DateTime.UtcNow;
            var timeoutDuration = TimeSpan.FromSeconds(20);

            while (!fileDeleted && timeoutDuration < DateTime.UtcNow - tryDeleteStartTime)
            {
                // this is in a loop since sometimes the file is still locked by the compiler
                try
                {
                    File.Delete(outFilePath);

                    fileDeleted = true;
                }
                catch (IOException)
                {
                    // file is still locked by the compiler - loop back and try again
                }
                catch (Exception)
                {
                    errorDeletingFile = true;
                }
            }

            if (errorDeletingFile)
            {
                Console.WriteLine($"A temporary file was created but could not be deleted. Location:\n\t{outFilePath}.");
            }
        }
    }
}
