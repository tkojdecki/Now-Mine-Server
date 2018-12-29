using NowMineCommon.Enums;
using NowMineCommon.Models;
using System.Collections.Generic;
using System.Linq;

namespace NowMine
{
    public static class EventManager
    {
        private static List<EventItem> EventHistory = new List<EventItem>();
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

        public static uint GetIDForEvent(CommandType commandType, object data)
        {
            uint nextID = NextEventID;
            var historyItem = new EventItem(commandType, data, nextID);
            EventHistory.Add(historyItem);
            return nextID;
        }

        public static List<EventItem>GetEventsFrom(uint fromID)
        {
            var eventItems = new List<EventItem>();

            return EventHistory.Where(e => e.EventID > fromID).ToList();
        }
    }
}
