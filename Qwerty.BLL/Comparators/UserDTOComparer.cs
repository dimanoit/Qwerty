using Qwerty.BLL.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qwerty.BLL.Comparators
{
    public class UserDTOComparer : IEqualityComparer<UserDTO>
    {
        public bool Equals(UserDTO x, UserDTO y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;
            return x.Id == y.Id;
        }

        public int GetHashCode(UserDTO user)
        {
            if (Object.ReferenceEquals(user, null)) return 0;
            return user.Id == null ? 0 : user.Id.GetHashCode();
        }

    }
}
