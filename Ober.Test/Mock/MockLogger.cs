using System.Collections.Generic;
using Ober.Tool.Interfaces;

namespace Ober.Test.Mock
{
    internal class MockLogger: ILogger
    {
        public IList<string> Message { get; set; } = new List<string>();
        public bool Progress { get; set; }

        public bool Verbose { get; set; }
        public void Debug(string message)
        {
            Message.Add(message);
        }

        public void Error(string message)
        {
            Message.Add(message);
        }

        public void Info(string message)
        {
            Message.Add(message);
        }

        public void InfoWithProgress(string message)
        {
            Message.Add(message);
            Progress = true;
        }

        public void StopProgress()
        {
            Progress = false;
        }
    }
}
