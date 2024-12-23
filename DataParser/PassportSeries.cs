using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class PassportSeries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("series_id")]
        public int SeriesId { get; set; }

        [Required]
        [Column("series",TypeName = "smallint")]
        public short Series { get; set; }

        public ICollection<PassportNumber> PassportNumbers { get; set; } = new List<PassportNumber>();
    }
}
