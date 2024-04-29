using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Key]
        public short Id { get; set; }
        //public byte Id { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
