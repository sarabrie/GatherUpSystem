using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GatherUp.Core.Interfaces;
using System.Xml.Serialization;
namespace GatherUp.Core.DO.Polls
{
    public class PollQuestion
    {
        [XmlAttribute]
        public int QuestionId {  get; set; }=0;
        [XmlElement]
        public string QuestionText { get; set; }=string.Empty;
        [XmlElement]
        public List<string> Options { get; set; }= new List<string>();
        [XmlElement]
        public Dictionary<string, int> ParticipantVotes { get; set; } = new Dictionary<string, int>();
    }
}
