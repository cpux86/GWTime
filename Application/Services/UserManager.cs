using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Specification;
using Ardalis.Specification.EntityFrameworkCore;
//using Ardalis.Specification.EntityFrameworkCore;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Application.Services
{
    public class UserManager : IUserManager
    {
        private readonly IEventsDbContext _context;

        public UserManager(IEventsDbContext dbContext)
        {
            _context = dbContext;
        }


        /// <summary>
        /// Список групп
        /// </summary>
        /// <returns></returns>
        public async Task<List<Group>> GetGroupsListAsync()
        {
            var groups = await _context.Groups
                .AsNoTracking()
                .ToListAsync(CancellationToken.None);
            return groups;
        }

        /// <summary>
        /// Поиск по фамилии
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<List<User>> GetUserByNameAsync(string username)
        {

            //var test = await _context.Users.WithSpecification(new UserByIdSpec(3390))
            //    .FirstOrDefaultAsync(CancellationToken.None);

            var users = await _context.Users
                .AsNoTracking()
                .Include(e => e.Events)
                .Include(u => u.Group)
                .Where(u => u.Name.StartsWith(username) || u.FullName.StartsWith(username))
                .OrderBy(u => u.Name).ToListAsync(CancellationToken.None);
            return users;
        }


        public async Task<User> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(e => e.Events)
                .Include(u => u.Group)
                .Where(u => u.Id == userId)
                .FirstOrDefaultAsync(CancellationToken.None) ?? throw new Exception("пользователь не найден");
            //return user;
        }

        /// <summary>
        /// Возвращает список всех сотрудников 
        /// </summary>
        /// <returns></returns>
        public async Task<List<User>> GetUserListAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderBy(u=>u.Name)
                .ToListAsync(CancellationToken.None);
            return users;
        }
        /// <summary>
        /// Список всех сотрудников в группе 
        /// </summary>
        /// <param name="groupId">ID группы</param>
        /// <returns></returns>
        public async Task<List<User>> GetUserListByGroupIdAsync(int groupId)
        {
            var users = await _context.Users
                .AsNoTracking()
                .Where(u=>u.Group.Id == groupId)
                .OrderBy(u=>u.Group.Id)
                .ToListAsync(CancellationToken.None);
            return users;
        }
    }
}
