using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PS4_Tools.LibOrbis.GP4
{


    public class ChunkInfo
    {
        [XmlAttribute(AttributeName = "chunk_count")]
        public int chunk_count;

        [XmlAttribute(AttributeName = "scenario_count")]
        public int scenario_count;

        [XmlArrayItem(Type = typeof(Chunk), ElementName = "chunk")]
        [XmlArray(ElementName = "chunks")]
        public List<Chunk> chunks = new List<Chunk>();

        [XmlElement(ElementName = "scenarios")]
        public Scenarios scenarios;

        [XmlArrayItem(Type = typeof(ScenarioInfo), ElementName = "scenario_info")]
        [XmlArray(ElementName = "scenarios_info")]
        public List<ScenarioInfo> ScenariosInfo;
    }
    public class Chunk
    {
        [XmlAttribute(AttributeName = "id")]
        public int id;
        [XmlAttribute(AttributeName = "layer_no")]
        public int layer_no;
        [XmlAttribute(AttributeName = "label")]
        public string label;
    }
    public class Scenarios
    {
        [XmlAttribute(AttributeName = "default_id")]
        public int default_id;

        [XmlElement(Type = typeof(Scenario), ElementName = "scenario")]
        public List<Scenario> scenarios = new List<Scenario>();
    }
    public class Scenario
    {
        [XmlAttribute(AttributeName = "id")]
        public int id;
        [XmlAttribute(AttributeName = "type")]
        public string type;
        [XmlAttribute(AttributeName = "initial_chunk_count")]
        public int initial_chunk_count;
        [XmlAttribute(AttributeName = "label")]
        public string label;
        [XmlText]
        public string chunks;
    }
    public class ScenarioInfo
    {
        [XmlAttribute("id")]
        public int id;
        [XmlAttribute("type")]
        public string type;

        [XmlElement(ElementName = "lang")]
        public List<ScenarioLang> Langs;
    }
    public class ScenarioLang
    {
        [XmlAttribute("type")]
        public string type;
        [XmlAttribute("name")]
        public string name;
    }
}
