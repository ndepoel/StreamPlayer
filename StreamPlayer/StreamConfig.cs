using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StreamPlayer
{
    public class StreamConfig: XmlObject<StreamConfig>
    {
        private const string ConfigFile = "config.xml";

        [XmlIgnore]
        public string FFPlay { get; set; } = "ffplay.exe";

        [XmlIgnore]
        public string FontFile { get; set; } = "FreeSerif.ttf";

        public string StatUrl { get; set; } = "http://foo.bar.com:8080/stat";

        public string StreamBaseUrl { get; set; } = "rtmp://foo.bar.com";

        public string MyStream { get; set; } = "";

        public bool UseBorderless { get; set; } = false;

        public bool UseBuffering { get; set; } = false;

        private static StreamConfig _instance;
        public static StreamConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    try
                    {
                        _instance = LoadFromXmlFile(ConfigFile);
                    }
                    catch
                    {
                        _instance = new StreamConfig();
                    }
                }

                return _instance;
            }
        }

        public void Save()
        {
            SaveToXmlFile(ConfigFile);
        }
    }
}
