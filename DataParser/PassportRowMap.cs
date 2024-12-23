using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataParser
{
    public sealed class PassportRowMap : ClassMap<PassportRow>
    {
        public PassportRowMap()
        {
            Map(m => m.PASSP_SERIES_RAW).Index(0);
            Map(m => m.PASSP_NUMBER_RAW).Index(1);
        }
    }
}
