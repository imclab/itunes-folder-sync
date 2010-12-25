using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ITunesLibrarySynchronizer.lib.Utils
{
    class BackgroundWorkerMessage
    {
        public long MilisecondsElapsed { get; set; }
        public object WorkResult { get; set; }
        public Exception WorkerException { get; set; }
        public bool Cancelled { get; set; }
    }
}
