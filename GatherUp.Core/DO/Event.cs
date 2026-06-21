using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
using System.Xml.Serialization;
namespace GatherUp.Core.DO
{
    public class Event :IEntity 
    {
        [SetsRequiredMembers]
        public Event() { }

        [XmlAttribute]
        public int Id { get; set; }
        [XmlElement]
        public required string Title { get; set; } = string.Empty;
        [XmlElement]
        public DateTime EventDate { get; set; } 
        [XmlElement]
        public string? Location { get; set; }      
        [XmlElement]
        public required int EventManagerId { get; set; }         
        [XmlElement]
        public required int EventHostId { get; set; }            
        [XmlElement]
        public List<int> ParticipantIds { get; set; } = new List<int>();
        [XmlElement]
        public List<int> VendorIds { get; set; } = new List<int>();
        [XmlElement]
        public List<int> PollIds { get; set; } = new List<int>();
    }
}
    

