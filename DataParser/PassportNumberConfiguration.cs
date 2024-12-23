using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataParser
{
    public class PassportNumberConfiguration : IEntityTypeConfiguration<PassportNumber>
    {
        public void Configure(EntityTypeBuilder<PassportNumber> builder)
        {
            builder.ToTable("passport_numbers");
            builder.HasKey(e => e.NumberId);
            builder.Property(e => e.Status)
                .IsRequired();
            builder.HasOne(e => e.PassportSeries)
                .WithMany(p => p.PassportNumbers)
                .HasForeignKey(e => e.SeriesId);
        }
    }

}
