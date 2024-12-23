using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataParser
{
    public class ChangelogConfiguration : IEntityTypeConfiguration<Changelog>
    {
      public void Configure(EntityTypeBuilder<Changelog> builder)
        {
            builder.ToTable("changelog");
            builder.HasKey(e => e.ChangeId);
            builder.Property(e => e.ChangeStatus).IsRequired();
            builder.Property(e => e.DateOfChange).IsRequired();
            builder.HasOne(e => e.PassportNumber)
                .WithMany(p => p.Changelogs)
                .HasForeignKey(e => e.NumberId);
        }
    }
}
