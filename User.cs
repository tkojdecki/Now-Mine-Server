using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace NowMine
{
    public class User
    {
        public string Name { get; set; } 
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
        
        public int Id { get; set; }
        private byte[] _color;
        public byte[] UserColor
        {
            get
            {
                if (_color == null)
                    _color = new byte[3];
                return _color;
            }

            set
            {
                _color = value;
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

        public void addToQueue(MusicPiece piece)
        {
            piece.lbluserName.Content = this.Name;
        }

        internal Color getColor()
        {
            return Color.FromRgb(UserColor[0], UserColor[1], UserColor[2]);
        }
    }
}

