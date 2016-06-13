namespace Ober.Tool.Interfaces
{
    public interface ILogger
    {
        bool Verbose { get; set; }
        void Debug(string message);
        void Error(string message);
        void Info(string message);
        void InfoWithProgress(string message);
        void StopProgress();
    }
}