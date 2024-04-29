using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class UserGroup
    {
        public int Id { get; set; }

        /// <summary>
        /// Название группы
        /// </summary>
        public string Name { get; set; } = string.Empty;
        public List<User>? Users { get; set; }
    }

}
