using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.Extensions.Logging;

namespace Domain
{
    public class User : BaseEntity, IEquatable<User>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }
        /// <summary>
        /// Фамилия и инициалы. Иванов И.И.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Ф.И.О полностью.  Иванов Иван Иванович
        /// </summary>
        public string FullName { get; set; } = string.Empty;
        /// <summary>
        /// События использования ключа
        /// </summary>
        public HashSet<Event>? Events { get; set; } = new HashSet<Event>();

        /// <summary>
        /// Группа в которую входит сотрудник
        /// </summary>
       // public string? Group { get; set; } = string.Empty;
        public UserGroup? UserGroup { get; set; }

        public bool Equals(User? other)
        {
            throw new NotImplementedException();
        }

    }
}
