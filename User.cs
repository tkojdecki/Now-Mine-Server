using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NowMine
{
<<<<<<< HEAD
    public class User
    {
        public String name;
        List<MusicPiece> queued = new List<MusicPiece>();
        List<MusicPiece> history = new List<MusicPiece>();
        private static User serverUser;

        public User(String name)
        {
            this.name = name;
        }

        public void addToQueue(MusicPiece piece)
        {
            piece.lbluserName.Content = this.name;
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
=======
    class User
    {

>>>>>>> 3e8e90aa518eb41b9e9876abd6e8831da805f7c6
    }
}
