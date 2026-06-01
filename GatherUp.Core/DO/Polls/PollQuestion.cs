using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Polls
{
    public class PollQuestion
    {
        public int QuestionId {  get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }= new List<string>();
        public Dictionary<string, int> ParticipantVotes { get; set; } = new Dictionary<string, int>();


    }
}
