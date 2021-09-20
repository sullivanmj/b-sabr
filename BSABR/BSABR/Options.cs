// Copyright © 2021 Matt Sullivan

namespace Bsabr
{
    /// <summary>
    /// Represents the command line options selected when the process was started.
    /// </summary>
    class Options
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Options"/> class.
        /// </summary>
        /// <param name="studioExecutable">Path to the Atmel Studio/Microchip Studio executable.</param>
        /// <param name="outFileName">The name of the output file requested in the process arguments. <c>null</c> if no /out argument was supplied.</param>
        /// <param name="studioArgs">The arguments that should be passed to Atmel Studio/Microchip Studio.</param>
        /// <param name="helpRequested">Indicates whether or not a help argument was passed in at process start.</param>
        public Options(string studioExecutable, string outFileName, bool outFileSpecified, string studioArgs, bool helpRequested)
        {
            StudioExecutable = studioExecutable;
            OutFileName = outFileName;
            OutFileSpecified = outFileSpecified;
            StudioArgs = studioArgs;
            HelpRequested = helpRequested;
        }

        /// <summary>
        /// The name of the output file requested in the process arguments. Is <c>null</c> when no /out argument was supplied.
        /// </summary>
        public string OutFileName { get; }

        /// <summary>
        /// Whether or not the user specified the "/out" argument for AtmelStudio.exe.
        /// </summary>
        public bool OutFileSpecified { get; }

        /// <summary>
        /// Path to the Atmel Studio/Microchip Studio executable.
        /// </summary>
        public string StudioExecutable { get; }

        /// <summary>
        /// The arguments that should be passed to Atmel Studio/Microchip Studio.
        /// </summary>
        public string StudioArgs { get; }

        /// <summary>
        /// Indicates whether or not a help argument was passed in at process start.
        /// </summary>
        public bool HelpRequested { get; }
    }
}
