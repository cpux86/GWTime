using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;
using Microsoft.Extensions.Logging;

namespace Domain
{
    public class User : BaseEntity
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
        public List<Event>? Events { get; set; } = new List<Event>();

        /// <summary>
        /// Группа в которую входит сотрудник
        /// </summary>
        public UserGroup? UserGroup { get; set; }

    }
}
