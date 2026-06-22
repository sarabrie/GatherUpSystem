using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using GatherUp.Core.Interfaces;
using System.Xml.Serialization;

namespace GatherUp.Core.DO
{
    public class Event : IEntity
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

        [XmlArray("ParticipantIds")]
        [XmlArrayItem("int")]
        public List<int> ParticipantIds { get; set; } = new List<int>();

        [XmlArray("VendorIds")]
        [XmlArrayItem("int")]
        public List<int> VendorIds { get; set; } = new List<int>();

        [XmlArray("PollIds")]
        [XmlArrayItem("int")]
        public List<int> PollIds { get; set; } = new List<int>();

        [XmlArray("ReceiptIds")]
        [XmlArrayItem("int")]
        public List<int> ReceiptIds { get; set; } = new List<int>();
    }
}
