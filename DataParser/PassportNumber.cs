using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class PassportNumber
    { 
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("number_id")]
        public int NumberId { get; set; }

        [Required]
        [ForeignKey("PassportSeries")]
        [Column("series_id")]
        public int SeriesId { get; set; }
        public PassportSeries PassportSeries { get; set; }

        [Column("number")]
        [Required]
        public int Number { get; set; }
        [Column("status")]
        [Required]
        public bool Status { get; set; } = false;

        public ICollection<Changelog> Changelogs { get; set; } = new List<Changelog>();
    }
}
