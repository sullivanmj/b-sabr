// Copyright © 2021 Matt Sullivan

using System.Linq;

namespace Bsabr
{
    /// <summary>
    /// Extracts string arguments for B-SABR into a more easy to use object.
    /// </summary>
    class ArgParser
    {
        private readonly string[] _args;

        /// <summary>
        /// Initializes a new instance of the <see cref="ArgParser"/> class.
        /// </summary>
        /// <param name="args">The arguments to extract.</param>
        private ArgParser(string[] args)
        {
            _args = args;
        }

        /// <summary>
        /// Gets a new instance of the <see cref="ArgParser"/> class.
        /// </summary>
        /// <param name="args">The arguments that the ArgParser should process.</param>
        /// <returns>An <see cref="ArgParser"/> for the specified <paramref name="args"/>.</returns>
        public static ArgParser InitializeNew(string[] args)
        {
            return new ArgParser(args);
        }

        /// <summary>
        /// Gets the options that this <see cref="ArgParser"/>'s arguments specified.
        /// </summary>
        /// <returns>The options that were represented by the arguments.</returns>
        internal Options GetOptions()
        {
            // help will have been requested if the first of the arguments match a normal help string
            var helpRequested = _args.FirstOrDefault() == "/?" || _args.FirstOrDefault() == "/help";

            // studio file name will be the first argument (as long as that wasn't the help arg)
            var studioFileName = helpRequested ? null : _args.FirstOrDefault();

            // whether or not the caller is requesting build output to be saved to a file
            var outFileSpecified = _args.Any(a => a == "/out");

            // output filename will be the first argument after the "/out" filename
            var outFileName = outFileSpecified ? _args.SkipWhile(a => a != "/out").Skip(1).FirstOrDefault() : null;

            // studio arguments will be all of the arguments after the first argument
            var studioArgs = helpRequested ? null : string.Join(" ", _args.Skip(1));

            return new Options(studioFileName, outFileName, outFileSpecified, studioArgs, helpRequested);
        }
    }
}
