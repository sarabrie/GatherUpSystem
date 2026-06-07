using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatherUp.Core.Interfaces;
namespace GatherUp.Core.DO.Users
{
    public class Person:IEntity
    {
        public int Id { get; set; }
        public required string Name { get; set; } =string.Empty;
        public required string Email { get; set; } =string.Empty;

    }
}
