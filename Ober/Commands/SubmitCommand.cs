using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Ober.Tool.Interfaces;
using Ober.Tool.Options;

namespace Ober.Tool.Commands
{
    internal class SubmitCommand : CommandBase, ISubmitCommand
    {
        private SubmitOptions _submitOptions;

        public SubmitCommand(IStoreClient client, ILogger logger) : base(client, logger) { }

        public async Task<int> CreateSubmission(SubmitOptions options)
        {
            _submitOptions = options;
            return await HandleCommand(options.ConfigFile, options.Verbose);
        }

        /// <summary>
        /// Flow:
        ///  <para>* Create new submission using POST (Success == HttpStatusCode 201 Created)</para>
        ///  <para>* Get the JSON object and mark all packages as PendingDelete</para>
        ///  <para>* Add the new packages from the zip as PendingUpload</para>
        ///  <para>* Upload the changes using PUT (entire JSON in body)</para>
        ///  <para>* Upload the zip file</para>
        ///  <para>* Commit the changes to trigger the submission</para>
        /// </summary>
        /// <returns>StatusCode of the command.</returns>
        protected override async Task<int> CommandLogic()
        {
            IList<string> packages;
            if (!ValidatePackagesFile(_submitOptions.PackagesFile, out packages))
                return -1;
            //Create new submission using POST(Success == HttpStatusCode 201 Created)
            var submission = await NewSubmission(_submitOptions);
            // Get the JSON object and mark all packages as PendingDelete
            submission = UpdatePackages(_submitOptions, submission, packages);
            // Add the changes using PUT(entire JSON in body)
            var idReference = string.IsNullOrWhiteSpace(_submitOptions.Flight) ? "id" : "submissionId";
            var updateSuccess = await UpdateSubmission(_submitOptions, submission[idReference].ToString(), submission);
            if (updateSuccess)
            {
                // Upload the zip file
                Logger.InfoWithProgress("Uploading package");
                await Client.UploadPackages(_submitOptions.PackagesFile, submission["fileUploadUrl"].ToString());
                Logger.StopProgress();
                // Commit the changes 
                var commitResult = await CommitSubmission(_submitOptions, submission[idReference].ToString());
                return commitResult ? 0 : -1;
            }
            Logger.Error("I was unable to update the submission.");
            return -1;
        }

        private bool ValidatePackagesFile(string packagesFile, out IList<string> appPackages)
        {
            appPackages = new List<string>();
            if (!_submitOptions.PackagesFile.EndsWith(".zip"))
            {
                Logger.Error($"the provided packages file at {packagesFile} is not a zip file.");
                return false;
            }
            if (!File.Exists(packagesFile))
            {
                Logger.Error($"the provided packages file at {packagesFile} does not exist.");
                return false;
            }
            using (var archive = new ZipArchive(new FileStream(_submitOptions.PackagesFile, FileMode.Open)))
            {
                var validPackages = archive.Entries.Where(x => x.Name.EndsWith(".appxupload"));
                if (!validPackages.Any())
                {
                    Logger.Error($"the provided packages file at {packagesFile} does not contain any .appxupload package.");
                    return false;
                }
                foreach (var entry in archive.Entries.Where(x => x.Name.EndsWith(".appxupload")))
                {
                    appPackages.Add(entry.Name);
                }
            }
            return true;
        }

        private async Task<JObject> NewSubmission(SubmitOptions submitOptions)
        {
            Tuple<JObject, HttpStatusCode> deployResult;
            Logger.InfoWithProgress("Creating new submission");
            if (string.IsNullOrWhiteSpace(submitOptions.Flight))
            {
                deployResult = await Client.CreateSubmission(submitOptions.Application);
            }
            else
            {
                deployResult = await Client.CreateSubmission(submitOptions.Application, submitOptions.Flight);
            }
            Logger.StopProgress();
            if (deployResult.Item2.Equals(HttpStatusCode.Created))
                return deployResult.Item1;
            Logger.Error("I was unable to create a new submission.");
            Logger.Info("Would you mind to verify the provided parameters and order again?");
            return null;
        }

        private JObject UpdatePackages(SubmitOptions submitOptions, JObject submission, IEnumerable<string> zipPackages)
        {
            var packagesReference = string.IsNullOrWhiteSpace(submitOptions.Flight) ? "applicationPackages" : "flightPackages";
            var packages = (JArray)submission[packagesReference];
            var newPackages = new JArray();
            foreach (var package in packages)
            {
                package["fileStatus"] = "PendingDelete";
                newPackages.Add(package);
            }
            // Add the new packages as PendingUpload
            foreach (var package in zipPackages.Select(fileName => new JObject { { "fileName", fileName }, { "fileStatus", "PendingUpload" } }))
            {
                newPackages.Add(package);
            }
            packages.Replace(newPackages);
            return submission;
        }

        private async Task<bool> UpdateSubmission(SubmitOptions submitOptions, string submissionId, JObject body)
        {
            bool updateResult;
            Logger.InfoWithProgress("Updating submission");
            if (string.IsNullOrWhiteSpace(submitOptions.Flight))
            {
                updateResult = await Client.UpdateSubmission(submitOptions.Application, submissionId, body.ToString());
            }
            else
            {
                updateResult = await Client.UpdateSubmission(submitOptions.Application, submitOptions.Flight, submissionId, body.ToString());
            }
            Logger.StopProgress();
            return updateResult;
        }

        private async Task<bool> CommitSubmission(SubmitOptions submitOptions, string submissionId)
        {
            bool commitResult;
            Logger.InfoWithProgress("Committing submission");
            if (string.IsNullOrWhiteSpace(submitOptions.Flight))
            {
                commitResult = await Client.Commit(submitOptions.Application, submissionId);
            }
            else
            {
                commitResult = await Client.Commit(submitOptions.Application, submitOptions.Flight, submissionId);
            }
            Logger.StopProgress();
            if (commitResult)
            {
                Logger.Info("The submission has been successfully comitted, sir.");
            }
            else
            {
                Logger.Error("the submission could not be comitted.");
            }
            return commitResult;
        }
    }
}
