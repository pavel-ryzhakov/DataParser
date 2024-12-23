using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataParser
{
    public class PassportSeriesConfiguration : IEntityTypeConfiguration<PassportSeries>
    {
        public void Configure(EntityTypeBuilder<PassportSeries> builder)
        {
            builder.ToTable("passport_series");
            builder.HasKey(e => e.SeriesId);
            builder.Property(e => e.Series)
                .IsRequired()
                .HasMaxLength(10);
        }
    }

}
