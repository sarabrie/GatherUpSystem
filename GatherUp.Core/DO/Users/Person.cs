using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
using System.Xml.Serialization;
namespace GatherUp.Core.DO.Users
{
    public class Person:IEntity
    {
        [XmlAttribute]
        public int Id { get; set; }
        [XmlElement]
        public string Name { get; set; } =string.Empty;
        [XmlElement]
        public  string Email { get; set; } =string.Empty;

    }
}
