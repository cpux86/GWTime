using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ardalis.Specification;
using Domain;

namespace Application.Specification
{
    public sealed class UserByIdSpec : Specification<User>
    {
        public UserByIdSpec(int id)
        {
            Query.Where(u => u.Id == id)
                .AsNoTracking()
                .Include(u => u.Events.Where(e=>e.DateTime > DateTime.Parse("01.02.2024")));
        }
    }
}
