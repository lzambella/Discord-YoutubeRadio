using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
namespace Discord_NetCore
{
    [XmlRoot("discordSettings")]
    public class Settings
    {
        [XmlElement("discordToken")]
        public string DiscordToken { get; set; }
        [XmlElement("dacebookToken")]
        public string FacebookToken { get; set; }
        [XmlElement("databaseString")]
        public string DatabaseString { get; set; }
        [XmlElement("facebookEnabled")]
        public bool FacebookEnabled { get; set; }
        [XmlElement("databaseEnabled")]
        public bool DatabaseEnabled { get; set; }
    }

}
