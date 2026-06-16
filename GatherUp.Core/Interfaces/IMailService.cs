using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GatherUp.Core.Interfaces
{
    public interface IMailService
    {
        void Send(string to, string subject, string body);
    }
}

