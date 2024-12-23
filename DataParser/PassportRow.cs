using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public class PassportRow
    {
        public short PASSP_SERIES { get; set; }
        public int PASSP_NUMBER { get; set; }
        public string PASSP_SERIES_RAW { get; set; } 
        public string PASSP_NUMBER_RAW { get; set; } 
    }
}
