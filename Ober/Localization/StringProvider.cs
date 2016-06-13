using System.Reflection;
using System.Resources;
using Ober.Tool.Interfaces;

namespace Ober.Tool.Localization
{
    public class StringProvider : IStringProvider
    {
        private readonly ResourceManager _resourceManager = new ResourceManager("Ober.Tool.Localization.strings", Assembly.GetExecutingAssembly());//ResourceManager.CreateFileBasedResourceManager("Localization", "strings", null);

        public string GetString(Strings resource)
        {
            return _resourceManager.GetString(resource.ToString());
        }
    }
    
    public enum Strings
    {
        UploadingPackage,
        CommandNotYetImplemented,
        ValidatePackageNoZip,
        ValidatePackageNonExistant,
        ValidatePackageNoPackages,
        SubmitUpdateError,
        SubmitCreateError,
        VerifyParameters,
        SubmitUploading,
        SubmitCreating,
        SubmitUpdating,
        SubmitCommitting,
        SubmitCommitSuccess,
        SubmitCommitError,
        LoginError,
        LoginSuccess,
        LoginProgress,
        ConfigError,
        ConfigTemplate,
        ConfigFound,
        ConfigNotFound,
        ConfigMalformed,
        ConfigSuccess,
        DebugLoginError
    }
}
