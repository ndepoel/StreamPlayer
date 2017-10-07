using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace StreamPlayer
{
    [XmlRoot("rtmp")]
    public class ServerStats: XmlObject<ServerStats>
    {
        [XmlElement("nginx_version")]
        public string NginxVersion { get; set; }

        [XmlElement("server")]
        public ServerData Server { get; set; }
        
        public override string ToString()
        {
            return string.Format("NGINX RTMP version {0}", NginxVersion);
        }
    }

    public class ServerData
    {
        [XmlElement("application")]
        public ApplicationData Application { get; set; }
    }

    public class ApplicationData
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("live")]
        public LiveData Live { get; set; }

        public List<StreamData> LiveStreams { get { return Live?.Stream; } }
    }

    public class LiveData
    {
        [XmlElement("stream")]
        public List<StreamData> Stream { get; set; }

        [XmlElement("nclients")]
        public int ClientCount { get; set; }
    }
    
    public class StreamData
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("time")]
        public int Time { get; set; }   // milliseconds

        [XmlElement("meta")]
        public StreamMetaData Meta { get; set; }
    }

    public class StreamMetaData
    {
        [XmlElement("video")]
        public VideoData Video { get; set; }

        [XmlElement("audio")]
        public AudioData Audio { get; set; }
    }

    public class VideoData
    {
        [XmlElement("width")]
        public int Width { get; set; }

        [XmlElement("height")]
        public int Height { get; set; }

        [XmlElement("frame_rate")]
        public int Framerate { get; set; }

        [XmlElement("codec")]
        public string Codec { get; set; }

        [XmlElement("profile")]
        public string Profile { get; set; }
    }

    public class AudioData
    {
        [XmlElement("codec")]
        public string Codec { get; set; }

        [XmlElement("profile")]
        public string Profile { get; set; }

        [XmlElement("channels")]
        public int Channels { get; set; }

        [XmlElement("sample_rate")]
        public int SampleRate { get; set; }
    }
}
