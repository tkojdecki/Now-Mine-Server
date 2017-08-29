using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine.Helpers
{
    class EventArgsHelper
    {
    }

    public class GenericEventArgs<T> : EventArgs
    {
        public T EventData { get; private set; }

        public GenericEventArgs(T EventData)
        {
            this.EventData = EventData;
        }
    }
}
