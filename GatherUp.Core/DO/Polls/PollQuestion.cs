using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Polls
{
    public class PollQuestion
    {
        public int QuestionId {  get; set; }=0;
        public string QuestionText { get; set; }=string.Empty;
        public List<string> Options { get; set; }= new List<string>();
        public Dictionary<string, int> ParticipantVotes { get; set; } = new Dictionary<string, int>();


    }
}
