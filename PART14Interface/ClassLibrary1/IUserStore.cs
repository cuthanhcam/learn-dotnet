using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    internal interface IUserStore
    {
        User? GetUser(string username);
        void UpdateUser(User user);
        void DeleteUser(string username);
    }
}
