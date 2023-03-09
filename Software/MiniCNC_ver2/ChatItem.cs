using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniCNC_ver2
{
    public class ChatItem
    {
        public string Sender { get; set; }
        public string Message { get; set; }
        public DateTime Time { get; set; }

        public void UpdateChat(string sender, string message)
        {
            this.Sender = sender;
            this.Message = message;
            this.Time = DateTime.Now;
        }
    }
}
