using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
using System.Xml.Serialization;
namespace GatherUp.Core.DO.Polls
{
    public class Poll:IEntity
    {
        [XmlAttribute]
        public int Id {  get; set; }
        [XmlElement]
        public string Title { get; set; }= string.Empty;
        [XmlElement]
        public string Description { get; set; } = string.Empty;
        [XmlElement]
        public List<PollQuestion> Questions { get; set; } = new List<PollQuestion>();
    }
}
