using System.Linq;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using NUnit.Framework;
using Ober.Test.Extensions;
using Ober.Test.Mock;
using Ober.Tool.Interfaces;
using Ober.Tool.Localization;
using Ober.Tool.Options;

namespace Ober.Test.Commands
{
    [TestFixture]
    public class CommandBaseTest
    {
        [Test]
        public async Task TestLoginFailure()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => false));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new MockCommandBase(client, logger, stringprovider);
            var options = new OptionsBase
            {
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.DoCommand(options);
            result.Should().BeNegative("client credentials do not match");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
        }

        [Test]
        public async Task TestLoginSuccess()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new MockCommandBase(client, logger, stringprovider);
            var options = new OptionsBase
            {
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.DoCommand(options);
            result.Should().BeGreaterOrEqualTo(0, "client credentials do not match");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
        }

        [Test]
        public async Task TestConfigFileEmpty()
        {
            await TestConfigFailure(".invalid_config_empty", Strings.ConfigMalformed);
        }

        [Test]
        public async Task TestConfigFileGarbled()
        {
            await TestConfigFailure(".invalid_config_garbled", Strings.ConfigError);
        }

        [Test]
        public async Task TestConfigFileNoClientId()
        {
            await TestConfigFailure(".invalid_config_noclientid", Strings.ConfigMalformed);
        }

        [Test]
        public async Task TestConfigFileNoKey()
        {
            await TestConfigFailure(".invalid_config_nokey", Strings.ConfigMalformed);
        }

        [Test]
        public async Task TestConfigFileNoTentantId()
        {
            await TestConfigFailure(".invalid_config_notenant", Strings.ConfigMalformed);
        }

        [Test]
        public async Task TestConfigFileNothing()
        {
            await TestConfigFailure(".invalid_config_nothing", Strings.ConfigMalformed);
        }

        [Test]
        public async Task TestConfigFileNonExistant()
        {
            await TestConfigFailure(".invalid_config_notreally here", Strings.ConfigNotFound);
        }

        public async Task TestConfigFailure(string fileName, Strings expectedMessage)
        {
            var client = A.Fake<IStoreClient>();
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new MockCommandBase(client, logger, stringprovider);
            var options = new OptionsBase
            {
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\{fileName}"
            };
            var result = await command.DoCommand(options);
            logger.Message.ElementAt(logger.Message.Count - 2).Should().Be(string.Format(stringprovider.GetString(expectedMessage), options.ConfigFile), "because there is something wrong with the config file");
            result.Should().BeNegative("the config file is invalid");
        }
    }
}
