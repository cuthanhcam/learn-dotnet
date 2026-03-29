using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    internal interface IPasswordAuthenticator
    {
        User? Authenticate(string username, string password);
    }
}
