using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
namespace GatherUp.Core.DO.Polls
{
    public class Poll:IEntity
    {
        public int Id {  get; set; }
        public string Title { get; set; }= string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<PollQuestion> Questions { get; set; } = new List<PollQuestion>();
    }
}
