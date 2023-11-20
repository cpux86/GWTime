using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IEventsService
    {
        public Task<List<Event>> GetEventsByUserId(int inputReader, int outputReader, int userId);
        public Task<List<User>> GetUserListAsync();
        public Task<List<User>> GetUserByNameAsync(string name);

    }
}
