using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VideoGameCompendium.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; }
        public string RecieverId { get; set; }
        public DateTime PostTime { get; set; }

        public Comment(string text, string senderId, string recieverId)
        {
            Text = text;
            SenderId = senderId;
            RecieverId = recieverId;
        }
    }
}
