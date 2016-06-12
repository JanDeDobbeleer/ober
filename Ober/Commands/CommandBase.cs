using System;
using System.IO;
using System.Threading.Tasks;
using Ober.Tool.Interfaces;
using YamlDotNet.Serialization;

namespace Ober.Tool.Commands
{
    internal abstract class CommandBase
    {
        protected readonly IStoreClient Client;
        protected readonly ILogger Logger;

        protected CommandBase(IStoreClient client, ILogger logger)
        {
            Client = client;
            Logger = logger;
        }

        protected abstract Task<int> CommandLogic();

        protected async Task<int> HandleCommand(string configFile, bool verbose)
        {
            Logger.Verbose = verbose;
            Config.Config config;
            if (!TryLoadConfig(configFile, out config))
                return -1;
            Logger.InfoWithProgress("Logging in");
            var didLogin = await Client.Login(config.Credentials.ClientId, config.Credentials.Key, config.Credentials.TenantId);
            Logger.StopProgress();
            if (didLogin)
            {
                Logger.Debug("Login successful!");
                return await CommandLogic();
            }
            Console.WriteLine("Login unsuccessful, please provide the correct credentials.");
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
                Logger.Error($"Ì'm unable to load the config file at {configFile}, would you mind to verify if the file exists and is correctly configured?");
                Logger.Info(_configTemplate);
                return false;
            }
        }

        private bool VerifyAndLoadConfig(string configFile, out Config.Config config)
        {
            config = null;
            if (File.Exists(configFile))
            {
                Logger.Debug($"Config file found at {configFile}");
                var configText = File.OpenText(configFile);
                var deserializer = new Deserializer();
                config = deserializer.Deserialize<Config.Config>(configText);
                return ValidateConfig(config, configFile);
            }
            Logger.Error($"the config file cannot be found at {configFile}, would you mind adding one to proceed?");
            Logger.Info(_configTemplate);
            return false;
        }

        private bool ValidateConfig(Config.Config config, string configFile)
        {
            if (string.IsNullOrWhiteSpace(config?.Credentials?.ClientId)
                    || string.IsNullOrWhiteSpace(config.Credentials?.Key)
                    || string.IsNullOrWhiteSpace(config.Credentials?.TenantId))
            {
                Logger.Error($"it seems the configfile at {configFile} is incomplete or malformed.");
                Logger.Info(_configTemplate);
                return false;
            }
            Logger.Debug("Successfully loaded config file.");
            return true;
        }

        private string _configTemplate = "\nThe config file menu is:\n\nCredentials:\n\tClientId: <clientid>\n\tKey: <key>\n\tTenantId: <tenantid>";
    }
}
