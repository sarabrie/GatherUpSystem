using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.DO.Users
{
    internal class Person
    {
        public int Id { get; set; }
        public required string Name { get; init; }
        public required string Email { get; init; }


    }
}
