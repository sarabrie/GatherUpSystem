using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.DO.Finance
{
    public class VendorAllocation : IEntity
    {
        [SetsRequiredMembers]
        public VendorAllocation() { }

        [XmlAttribute]
        public int Id { get; set; }

        [XmlElement]
        public required string Name { get; set; } = string.Empty;

        [XmlElement]
        public decimal AmountOwed { get; set; } = Decimal.Zero;

        [XmlElement]
        public bool HasReceipt { get; set; } = false;

        [XmlArray("ReceiptIds")]
        [XmlArrayItem("ReceiptId")]
        public List<int> ReceiptIds { get; set; } = new List<int>();
    }
}
