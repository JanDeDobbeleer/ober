using Ninject.Modules;
using Ober.Tool.Api;
using Ober.Tool.Commands;
using Ober.Tool.Interfaces;
using Ober.Tool.Logger;

namespace Ober.Tool
{
    public class Bindings: NinjectModule
    {
        public override void Load()
        {
            Bind<ISpinner>().To<Spinner>();
            Bind<ILogger>().To<ConsoleLogger>();
            Bind<IStoreClient>().To<StoreClient>();
            Bind<IShowCommand>().To<ShowCommand>();
            Bind<ISubmitCommand>().To<SubmitCommand>();
            Bind<IListCommand>().To<ListCommand>();
        }
    }
}
