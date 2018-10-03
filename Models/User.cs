using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using NowMineCommon.Models;

namespace NowMine.Models
{
    public class User : NowMineCommon.Models.BaseUser
    {
        private static User _serverUser;
        public static User serverUser
        {
            get
            {
                if (_serverUser == null)
                    _serverUser = new User("Server", 0);
                return _serverUser;
            }
        }

        public User(string name, int id)
        {
            this.Name = name;
            Random rnd = new Random();
            Id = id;
            for (int i = 0; i < 3; i++)
                UserColor[i] = (byte)rnd.Next(0, 255);
        }

        internal Color GetColor()
        {
            return Color.FromRgb(UserColor[0], UserColor[1], UserColor[2]);
        }
    }
}

