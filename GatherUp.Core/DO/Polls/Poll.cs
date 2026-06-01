using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Polls
{
    internal class Poll
    {
        public int id {  get; set; }
        public string name { get; set; }
        public string description { get; set; }
        List <PollQuestion> Questions {  get; set; }
    }
}
