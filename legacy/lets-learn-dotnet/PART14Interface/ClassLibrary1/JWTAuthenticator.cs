using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    internal interface JWTAuthenticator
    {
        User? Authenticate(string jwt);
    }
}
