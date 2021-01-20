using System.Collections.Generic;

namespace BungeeCore.Model
{
    public class Chat
    {
        public string text { get; set; }
        public string color { get; set; }
        public bool bold { get; set; }
        public List<Extra> extras { get; set; }
        public Chat() { }
        public Chat(Extra extra)
        {
            this.text = extra.text;
            this.color = extra.color;
            this.bold = extra.bold;
        }
        public Chat(Extra extra, List<Extra> extras) : this(extra)
        {
            this.extras = extras;
        }

        public class Extra
        {
            public string text { get; set; }
            public string color { get; set; }
            public bool bold { get; set; }
            public Extra() { }
        }
    }
}
