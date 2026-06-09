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
        public int QuestionId { get; set; } = 0;

        [XmlElement]
        public string QuestionText { get; set; } = string.Empty;

        [XmlElement]
        public List<string> Options { get; set; } = new List<string>();

        [XmlIgnore]
        public Dictionary<string, int> ParticipantVotes { get; set; } = new Dictionary<string, int>();

        //בגלל שXML לא יודע לשמור אוביקט מסוג מילון, הוספנו מחלקת עזר ששומרת את המשתנים של המילון ובזמן שמירה לXML שומרת עת זה דרך המחלקה
        public class VoteEntry
        {
            [XmlAttribute]
            public string ParticipantName { get; set; } = string.Empty; 

            [XmlAttribute]
            public int ChosenOptionIndex { get; set; } 
        }

        [XmlArray("ParticipantVotes")]
        [XmlArrayItem("Vote")]
        public VoteEntry[] ParticipantVotesXml
        {
            get
            {
                return ParticipantVotes
                    .Select(kv => new VoteEntry { ParticipantName = kv.Key, ChosenOptionIndex = kv.Value })
                    .ToArray();
            }
            set
            {
                ParticipantVotes = value.ToDictionary(x => x.ParticipantName, x => x.ChosenOptionIndex);
            }
        }
    }
}