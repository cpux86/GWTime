using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserManager : IUserManager
    {
        private readonly IEventsDbContext context;

        public UserManager(IEventsDbContext dbContext)
        {
            context = dbContext;
        }

        public async Task<List<UserGroup>> GetGroupsListAsync()
        {
            var groups = await context.UserGroups
                .AsNoTracking()
                .ToListAsync(CancellationToken.None);
            return groups;
        }

        public async Task<User> GetUserByName(string username)
        {
            var user = await context.Users
                .AsNoTracking()
                .Where(u => u.Name.StartsWith(username))
                .OrderBy(u=>u.Name)
                .FirstOrDefaultAsync(CancellationToken.None);
            return user;
        }

        public async Task<List<User>> GetUserListAsync()
        {
            var users = await context.Users
                .AsNoTracking()
                .OrderBy(u=>u.Name)
                .ToListAsync(CancellationToken.None);
            return users;
        }

        public async Task<List<User>> GetUserListByGroupIdAsync(int groupId)
        {
            var users = await context.Users
                .AsNoTracking()
                .Where(u=>u.UserGroup.Id == groupId)
                .OrderBy(u=>u.UserGroup.Id)
                .ToListAsync(CancellationToken.None);
            return users;
        }
    }
}
