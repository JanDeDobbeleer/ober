using System;
using System.Threading.Tasks;
using Ober.Tool.Interfaces;
using Ober.Tool.Options;

namespace Ober.Tool.Commands
{
    internal class ListCommand: CommandBase, IListCommand
    {
        private ListOptions _listOptions;

        public ListCommand(IStoreClient client, ILogger logger) : base(client, logger) { }

        public async Task<int> ListSubmissions(ListOptions options)
        {
            _listOptions = options;
            return await HandleCommand(_listOptions.ConfigFile, options.Verbose);
        } 
        
        protected override Task<int> CommandLogic()
        {
            return Task<int>.Factory.StartNew(() =>
            {
                Console.WriteLine("The list command has not yet been implemented");
                return -1;
            });
        }
    }
}
