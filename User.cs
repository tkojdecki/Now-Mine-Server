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
        public String Name { get; set; }
        List<MusicPiece> queued = new List<MusicPiece>();
        List<MusicPiece> history = new List<MusicPiece>();
        private static User serverUser;
        private Color color;

        public User(String name)
        {
            this.Name = name;
            Random rnd = new Random();
            this.color = Color.FromRgb((byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255), (byte)rnd.Next(0, 255));
        }

        public void addToQueue(MusicPiece piece)
        {
            piece.lbluserName.Content = this.Name;
            queued.Add(piece);
        }

        public static User getServerUser()
        {
            if (serverUser == null)
            {
                serverUser = new User("Server");
            }
            return serverUser;
        }

        internal Color getColor()
        {
            return this.color;
        }
    }
}

