using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Domain;

namespace Domain
{
    public class Reader : BaseEntity, IEquatable<Reader>
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public short Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public List<Event>? Events { get; set; }



        public bool Equals(Reader? other)
        {
            if (ReferenceEquals(null, other)) return false;
            return other.Id == Id && other.Name.Equals(Name);
        }
    }
}
