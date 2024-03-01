using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IUserManager
    {
        public Task<User> GetUserByName(string username);

        public Task<List<User>> GetUserListAsync();

        public Task<List<UserGroup>> GetGroupsListAsync();
        public Task<List<User>> GetUserListByGroupIdAsync(int groupId);
    }
}
