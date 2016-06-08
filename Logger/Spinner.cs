using System;
using System.Threading;
using Ober.Tool.Interfaces;

namespace Ober.Tool.Logger
{
    public class Spinner : IDisposable, ISpinner
    {
        private const string Sequence = @"/-\|";
        private int _counter;
        private readonly int _delay;
        private bool _active;
        private Thread _thread;

        public Spinner()
        {
            _delay = 100;
            _thread = new Thread(Spin);
        }

        public void Start()
        {
            Console.CursorVisible = false;
            _active = true;
            if (_thread.IsAlive)
                return;
            _thread.Abort();
            _thread = new Thread(Spin);
            _thread.Start();
        }

        public void Stop()
        {
            _active = false;
            Draw(' ');
            Console.CursorVisible = true;
        }

        private void Spin()
        {
            while (_active)
            {
                Turn();
                Thread.Sleep(_delay);
            }
        }

        private void Draw(char c)
        {
            try
            {
                Console.SetCursorPosition(Console.CursorLeft <= 0? 0 : Console.CursorLeft - 1, Console.CursorTop);
                Console.Write(c);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void Turn()
        {
            Draw(Sequence[++_counter % Sequence.Length]);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}