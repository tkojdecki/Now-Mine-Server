using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    public class EventManager
    {
        private uint _eventID;
        public uint EventID
        {
            get
            {
                _eventID++;
                return _eventID;
            }
        }


    }
}
