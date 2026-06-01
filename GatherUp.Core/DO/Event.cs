using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
namespace GatherUp.Core.DO
{
    public class Event :IEntity 
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public DateTime EventDate { get; set; } 
        public string? Location { get; set; }      
        public int EventManagerId { get; set; }         
        public int EventHostId { get; set; }            
        public List<int> ParticipantIds { get; set; } = new List<int>();
        public List<int> VendorIds { get; set; } = new List<int>();
        public List<int> PollIds { get; set; } = new List<int>();
    }
}
    

