using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
namespace GatherUp.Core.DO.Users
{
    public class Person:IEntity
    {
        [SetsRequiredMembers]
        public Person() { }

        [XmlAttribute]
        public int Id { get; set; }
        [XmlElement]
        public required string Name { get; set; } =string.Empty;
        [XmlElement]
        public required string Email { get; set; } =string.Empty;

    }
}
