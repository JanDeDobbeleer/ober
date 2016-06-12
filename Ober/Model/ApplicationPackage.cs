using Newtonsoft.Json;

namespace Ober.Tool.Model
{
    public class ApplicationPackage
    {
        [JsonProperty(PropertyName = "fileName")]
        public string FileName { get; set; }
        [JsonProperty(PropertyName = "fileStatus")]
        public string FileStatus { get; set; } = "PendingUpload";
    }
}
