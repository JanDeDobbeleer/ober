using System;
using System.IO;
using System.Threading.Tasks;
using Ober.Tool.Interfaces;
using Ober.Tool.Localization;
using YamlDotNet.Serialization;

namespace Ober.Tool.Commands
{
    public abstract class CommandBase
    {
        protected readonly IStoreClient Client;
        protected readonly ILogger Logger;
        protected readonly IStringProvider StringProvider;

        protected CommandBase(IStoreClient client, ILogger logger, IStringProvider stringProvider)
        {
            Client = client;
            Logger = logger;
            StringProvider = stringProvider;
        }

        protected abstract Task<int> CommandLogic();

        protected async Task<int> HandleCommand(string configFile, bool verbose)
        {
            Logger.Verbose = verbose;
            Config.Config config;
            if (!TryLoadConfig(configFile, out config))
                return -1;
            Logger.InfoWithProgress(StringProvider.GetString(Strings.LoginProgress));
            var didLogin = await Client.Login(config.Credentials.ClientId, config.Credentials.Key, config.Credentials.TenantId);
            Logger.StopProgress();
            if (didLogin)
            {
                Logger.Debug(StringProvider.GetString(Strings.LoginSuccess));
                return await CommandLogic();
            }
            Logger.Error(StringProvider.GetString(Strings.LoginError));
            return -1;
        }

        private bool TryLoadConfig(string configFile, out Config.Config config)
        {
            config = null;
            if (string.IsNullOrWhiteSpace(configFile))
                configFile = $"{Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)}\\.oberconfig";
            try
            {
                return VerifyAndLoadConfig(configFile, out config);
            }
            catch (Exception)
            {
                Logger.Error(string.Format(StringProvider.GetString(Strings.ConfigError), configFile));
                Logger.Info(StringProvider.GetString(Strings.ConfigTemplate));
                return false;
            }
        }

        private bool VerifyAndLoadConfig(string configFile, out Config.Config config)
        {
            config = null;
            if (File.Exists(configFile))
            {
                Logger.Debug(string.Format(StringProvider.GetString(Strings.ConfigFound), configFile));
                var configText = File.OpenText(configFile);
                var deserializer = new Deserializer();
                config = deserializer.Deserialize<Config.Config>(configText);
                return ValidateConfig(config, configFile);
            }
            Logger.Error(string.Format(StringProvider.GetString(Strings.ConfigNotFound), configFile));
            Logger.Info(StringProvider.GetString(Strings.ConfigTemplate));
            return false;
        }

        private bool ValidateConfig(Config.Config config, string configFile)
        {
            if (string.IsNullOrWhiteSpace(config?.Credentials?.ClientId)
                    || string.IsNullOrWhiteSpace(config.Credentials?.Key)
                    || string.IsNullOrWhiteSpace(config.Credentials?.TenantId))
            {
                Logger.Error(string.Format(StringProvider.GetString(Strings.ConfigMalformed), configFile));
                Logger.Info(StringProvider.GetString(Strings.ConfigTemplate));
                return false;
            }
            Logger.Debug(StringProvider.GetString(Strings.ConfigSuccess));
            return true;
        }
    }
}
