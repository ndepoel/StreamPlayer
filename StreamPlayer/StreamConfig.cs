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
        public string FontFile { get; set; } = "Helvetica.ttf";

        public string StatUrl { get; set; } = "http://foo.bar.com:8080/stat";

        /// <summary>
        /// Kept for backward compatibility with configs made by version 1.1.
        /// Existing StreamBaseUrl values are automatically migrated to StreamServer.
        /// </summary>
        public string StreamBaseUrl
        {
            get
            {
                return "rtmp://" + StreamServer;
            }
            set
            {
                Uri uri;
                if (Uri.TryCreate(value, UriKind.Absolute, out uri))
                {
                    if (uri.Scheme == "rtmp")
                    {
                        StreamServer = uri.Host;
                    }
                }
            }
        }

        // Read StreamBaseUrl from config when present, but don't write it back again.
        // This allows existing StreamBaseUrl values to be migrated to StreamServer.
        public bool ShouldSerializeStreamBaseUrl()
        {
            return false;
        }

        public string StreamServer { get; set; } = "foo.bar.com";

        public string Application { get; set; } = "live";

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
