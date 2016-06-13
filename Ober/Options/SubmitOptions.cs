using CommandLine;

namespace Ober.Tool.Options
{
    [Verb("submit", HelpText = "Create a new submission")]
    public class SubmitOptions: OptionsBase
    {
        [Option('a', "app", Required = true, HelpText = "The ID of the application you want to create a new submission for")]
        public string Application { get; set; }

        [Option('f', "flight", Required = false, HelpText = "The flight ID if you wish to submit a new flight")]
        public string Flight { get; set; }

        [Option('p', "packages", Required = true, HelpText = "The location of the zip file containing the packages to be submitted")]
        public string PackagesFile { get; set; }
    }
}