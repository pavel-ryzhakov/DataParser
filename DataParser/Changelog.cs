using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class Changelog
    {
        [Key]
        [Column("change_id")]
        public int ChangeId { get; set; }

        [Required]
        [ForeignKey("PassportNumber")]
        [Column("number_id")]
        public int NumberId { get; set; }
        public PassportNumber PassportNumber { get; set; }

        [Required]
        [Column("date_of_change",TypeName = "int")]
        public int DateOfChange { get; set; }

        [Required]
        [Column("change_status")]
        public bool ChangeStatus { get; set; }
    }
}
