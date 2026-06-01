using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Polls
{
    internal class PollQuestion
    {
        public int QuestionId {  get; set; }
        public string QuestionText { get; set; }
        public List<string> Options { get; set; }
        public Dictionary<string, int> ParticipantVotes { get; set; }


    }
}
