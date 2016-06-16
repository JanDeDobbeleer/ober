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
    public class SubmitCommandSuccessTests
    {

        [Test]
        public async Task TestSubmitSuccess()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var json = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\submission.json");
            A.CallTo(() => client.CreateSubmission("myapp")).Returns(Task.Factory.StartNew(() => new Tuple<JObject, HttpStatusCode>(JObject.Parse(json), HttpStatusCode.Created)));
            var updatedJson = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\submissionupdate.json");
            A.CallTo(() => client.UpdateSubmission("myapp", "submissionid", JObject.Parse(updatedJson).ToString())).Returns(Task.Factory.StartNew(() => true));
            A.CallTo(() => client.Commit("myapp", "submissionid")).Returns(Task.Factory.StartNew(() => true));
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
            result.Should().BeGreaterOrEqualTo(0, "the zip file contains app packages and everything worked out ok");
            logger.Message.Last().Should().Be(stringprovider.GetString(Strings.SubmitCommitSuccess), "everything went according to plan");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
            A.CallTo(() => client.CreateSubmission("myapp")).MustHaveHappened();
            A.CallTo(() => client.UpdateSubmission("myapp", "submissionid", JObject.Parse(updatedJson).ToString())).MustHaveHappened();
            A.CallTo(() => client.Commit("myapp", "submissionid")).MustHaveHappened();
        }



        [Test]
        public async Task TestSubmitFlightSuccess()
        {
            var client = A.Fake<IStoreClient>();
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).Returns(Task.Factory.StartNew(() => true));
            var json = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\flight.json");
            A.CallTo(() => client.CreateSubmission("myapp", "myflight")).Returns(Task.Factory.StartNew(() => new Tuple<JObject, HttpStatusCode>(JObject.Parse(json), HttpStatusCode.Created)));
            var updatedJson = File.ReadAllText($"{this.GetExecutingPath()}\\Files\\flightupdate.json");
            A.CallTo(() => client.UpdateSubmission("myapp", "myflight", "submissionid", JObject.Parse(updatedJson).ToString())).Returns(Task.Factory.StartNew(() => true));
            A.CallTo(() => client.Commit("myapp", "myflight", "submissionid")).Returns(Task.Factory.StartNew(() => true));
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
            result.Should().BeGreaterOrEqualTo(0, "the zip file contains app packages and everything worked out ok");
            logger.Message.Last().Should().Be(stringprovider.GetString(Strings.SubmitCommitSuccess), "everything went according to plan");
            A.CallTo(() => client.Login("myId", "myKey", "myTenantId")).MustHaveHappened();
            A.CallTo(() => client.CreateSubmission("myapp", "myflight")).MustHaveHappened();
            A.CallTo(() => client.UpdateSubmission("myapp", "myflight", "submissionid", JObject.Parse(updatedJson).ToString())).MustHaveHappened();
            A.CallTo(() => client.Commit("myapp", "myflight", "submissionid")).MustHaveHappened();
        }
    }
}
