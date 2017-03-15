using System.Collections.Generic;
using System.Threading;

namespace SiteParser.Application.Loader
{
    class ParallelTask
    {
        public delegate void Work();
        private Work _callback;

        private sbyte _threadCount;
        private List<ManualResetEvent> _events;

        public ParallelTask(sbyte threadCount)
        {
            _threadCount = threadCount;
            _events = new List<ManualResetEvent>();
        }

        public ParallelTask(sbyte threadCount, Work callback)
            : this(threadCount)            
        {
            _callback = callback;
        }

        public void setTask(Work task)
        {
            _callback = task;
        }

        public void start()
        {
            for (sbyte c = 0; c < _threadCount; c++)
            {
                var signal = new ManualResetEvent(false);
                _events.Add(signal);
                ThreadPool.QueueUserWorkItem(new WaitCallback((s) =>
                {
                    _callback();
                    signal.Set();
                }));
            }
        }

        public void Wait()
        {
            WaitHandle.WaitAll(_events.ToArray());
        }






    }
}
