using System;
using System.Threading.Tasks;
using Ober.Tool.Interfaces;
using Ober.Tool.Localization;
using Ober.Tool.Options;

namespace Ober.Tool.Commands
{
    internal class ShowCommand: CommandBase, IShowCommand
    {
        private ShowOptions _showOptions;

        public ShowCommand(IStoreClient client, ILogger logger, IStringProvider stringProvider) : base(client, logger, stringProvider) { }

        public async Task<int> ShowSubmission(ShowOptions options)
        {
            _showOptions = options;
            return await HandleCommand(options.ConfigFile, options.Verbose);
        }

        protected override Task<int> CommandLogic()
        {
            return Task<int>.Factory.StartNew(() =>
            {
                Logger.Info(StringProvider.GetString(Strings.CommandNotYetImplemented));
                return -1;
            });
        }
    }
}
