using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class Comment
    {
        public string ID;
        public string Text;
        public string SenderId;
        public string RecieverId;
        public DateTime PostTime;

        public Comment(){  }

        public Comment(string text, string senderId, string recieverId)
        {
            Text = text;
            SenderId = senderId;
            RecieverId = recieverId;
            PostTime = DateTime.Now;
        }
    }
}
