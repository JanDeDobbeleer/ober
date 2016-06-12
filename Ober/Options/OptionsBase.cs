using CommandLine;

namespace Ober.Tool.Options
{
    internal class OptionsBase
    {
        [Option('c', "config", Required = false, HelpText = "The path to the config file containing your credentials")]
        public string ConfigFile { get; set; }

        [Option('v', "verbose", Default = false, HelpText = "Prints all messages to standard output.")]
        public bool Verbose { get; set; }
    }
}
