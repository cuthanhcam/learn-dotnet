using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    internal class UserManager : IUserStore
    {
        public void DeleteUser(string username)
        {
            throw new NotImplementedException();
        }

        public User? GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public void UpdateUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}
