using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
    public static class EventManager
    {
        private static uint _eventID;
        public static uint EventID
        {
            get
            { 
                return _eventID;
            }
        }

        public static uint NextEventID
        {
            get
            {
                _eventID++;
                return _eventID;
            }
        }
    }
}
