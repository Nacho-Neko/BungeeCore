using System.Collections.Generic;

namespace BungeeCore.Model.ClientBound
{
    public class Response
    {
        /// <summary>
        /// 
        /// </summary>
        public Version version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Players players { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Description description { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string favicon { get; set; }

        public Response() { }
        public Response(string name, int protocol)
        {
            version = new Version(name, protocol);
            players = new Players();
            description = new Description();
        }
        public class Version
        {
            public Version() { }
            public Version(string name, int protocol)
            {
                this.name = name;
                this.protocol = protocol;
            }

            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int protocol { get; set; }
        }
        public class SampleItem
        {
            /// <summary>
            /// 
            /// </summary>
            public string name { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string id { get; set; }
        }
        public class Players
        {
            /// <summary>
            /// 
            /// </summary>
            public int max { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int online { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public List<SampleItem> sample { get; set; }

            public Players() { }
            public Players(List<SampleItem> sample) { this.sample = sample; }
        }
        public class Description
        {
            /// <summary>
            /// 
            /// </summary>
            public string text { get; set; }
        }
    }
}
