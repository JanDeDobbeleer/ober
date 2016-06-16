using System.Threading.Tasks;
using Ober.Tool.Commands;
using Ober.Tool.Interfaces;
using Ober.Tool.Options;

namespace Ober.Test.Mock
{
    internal class MockCommandBase: CommandBase
    {
        public MockCommandBase(IStoreClient client, ILogger logger, IStringProvider stringProvider) : base(client, logger, stringProvider)
        {
        }

        protected override Task<int> CommandLogic()
        {
            return Task.Factory.StartNew(() => 1);
        }

        public async Task<int> DoCommand(OptionsBase options)
        {
            return await HandleCommand(options.ConfigFile, options.Verbose);
        }
    }
}
