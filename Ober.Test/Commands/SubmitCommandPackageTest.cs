using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Ober.Test.Extensions;
using Ober.Test.Mock;
using Ober.Tool.Commands;
using Ober.Tool.Interfaces;
using Ober.Tool.Localization;
using Ober.Tool.Options;
using static System.String;

namespace Ober.Test.Commands
{
    [TestFixture]
    public class SubmitCommandPackageTest
    {
        [Test]
        public async Task TestSubmitNoZipfile()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new SubmitCommand(client, logger, stringprovider);
            var packageFileLocation = $"{this.GetExecutingPath()}\\Files\\mypackages.txt";
            var options = new SubmitOptions
            {
                PackagesFile = packageFileLocation,
                Application = "myapp",
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.CreateSubmission(options);
            result.Should().BeNegative("the file format is not a zip file");
            logger.Message.Last().Should().Be(Format(stringprovider.GetString(Strings.ValidatePackageNoZip), packageFileLocation), "The provided file is not a zip file");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
        }

        [Test]
        public async Task TestSubmitZipfileNonExistant()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new SubmitCommand(client, logger, stringprovider);
            var packageFileLocation = $"{this.GetExecutingPath()}\\Files\\mypackages.zip";
            var options = new SubmitOptions
            {
                PackagesFile = packageFileLocation,
                Application = "myapp",
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.CreateSubmission(options);
            result.Should().BeNegative("the file cannot be found");
            logger.Message.Last().Should().Be(Format(stringprovider.GetString(Strings.ValidatePackageNonExistant), packageFileLocation), "The provided file is not a zip file");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
        }

        [Test]
        public async Task TestSubmitZipfileNoPackages()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new SubmitCommand(client, logger, stringprovider);
            var packageFileLocation = $"{this.GetExecutingPath()}\\Files\\nopackage.zip";
            var options = new SubmitOptions
            {
                PackagesFile = packageFileLocation,
                Application = "myapp",
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.CreateSubmission(options);
            result.Should().BeNegative("the zip file does not contain app packages");
            logger.Message.Last().Should().Be(Format(stringprovider.GetString(Strings.ValidatePackageNoPackages), packageFileLocation), "The provided zip file does not contain any packages");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
        }
    }
}
