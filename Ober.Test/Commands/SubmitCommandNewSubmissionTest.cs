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

namespace Ober.Test.Commands
{
    [TestFixture]
    public class SubmitCommandNewSubmissionTest
    {
        [Test]
        public async Task TestSubmitNewSubmissionFailure()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var json = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\submission.json");
            A.CallTo(() => client.CreateSubmission("myapp")).Returns(Task.Factory.StartNew(() => new Tuple<JObject, HttpStatusCode>(JObject.Parse(json), HttpStatusCode.NotFound)));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new SubmitCommand(client, logger, stringprovider);
            var packageFileLocation = $"{this.GetExecutingPath()}\\Files\\package.zip";
            var options = new SubmitOptions
            {
                PackagesFile = packageFileLocation,
                Application = "myapp",
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config"
            };
            var result = await command.CreateSubmission(options);
            result.Should().BeNegative("the creation of submission failed");
            logger.Message.Last().Should().Be(stringprovider.GetString(Strings.VerifyParameters), "there's something wrong with the parameters");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
            A.CallTo(() => client.CreateSubmission("myapp")).MustHaveHappened();
        }

        [Test]
        public async Task TestSubmitNewFlightSubmissionFailure()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var json = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\submission.json");
            A.CallTo(() => client.CreateSubmission("myapp", "myflight")).Returns(Task.Factory.StartNew(() => new Tuple<JObject, HttpStatusCode>(JObject.Parse(json), HttpStatusCode.NotFound)));
            var logger = new MockLogger();
            var stringprovider = new StringProvider();
            var command = new SubmitCommand(client, logger, stringprovider);
            var packageFileLocation = $"{this.GetExecutingPath()}\\Files\\package.zip";
            var options = new SubmitOptions
            {
                PackagesFile = packageFileLocation,
                Application = "myapp",
                ConfigFile = $"{this.GetExecutingPath()}\\Files\\.valid_config",
                Flight = "myflight"
            };
            var result = await command.CreateSubmission(options);
            result.Should().BeNegative("the creation of submission failed");
            logger.Message.Last().Should().Be(stringprovider.GetString(Strings.VerifyParameters), "there's something wrong with the parameters");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
            A.CallTo(() => client.CreateSubmission("myapp", "myflight")).MustHaveHappened();
        }
    }
}
