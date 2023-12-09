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
        /// Ключ-карта
        /// </summary>
       // public string? Key { get; set; } = string.Empty;
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
        public string? Group { get; set; } = string.Empty;
        public UserGroup? UserGroup { get; set; }


        public List<string> GetWorkList(List<int> inputReader, List<int> outputReader)
        {


            var list = new List<string>();
            var preEvent = new Event();
            //foreach (var current in from current in events let exists = inputList.Exists(x => x == preEvent.ReaderId) select current)
            foreach (var current in this.Events!)
            {
                //var exists = inputList.Exists(x => x == preEvent.ReaderId);

                //if (preEvent.ReaderId == inputReader && current.ReaderId == outputReader && preEvent.UserId == current.UserId)

                //if (inputReader.Exists(x => x == preEvent.ReaderId) && outputReader.Exists(x => x == current.ReaderId) && preEvent.UserId == current.UserId)
                if (inputReader.Exists(x => x == preEvent.ReaderId) && outputReader.Exists(x => x == current.ReaderId))
                {


                    list.Add($"{current.User?.FullName} " +
                             $"{preEvent.Reader.Name}: {preEvent.DateTime}" +
                             $" {current.Reader.Name}: {current.DateTime} " +
                             $"Итого: {(int)current.DateTime.Subtract(preEvent.DateTime).TotalHours:00}:{current.DateTime.Subtract(preEvent.DateTime).Minutes:00}"
                    );
                }
                preEvent = current;

            }


            return list;
        }


        public bool Equals(User? u)
        {
            if (u == null) return false;
            return Group != null && Id == u.Id && FullName.Equals(u.FullName) && Name.Equals(u.Name) && Group.Equals(u.Group);
        }

    }
}
