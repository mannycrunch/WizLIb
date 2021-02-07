using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WizLib_Model.Models;

namespace WizLib_DataAccess.Data.FluentConfig
{
    public class FluentBookAuthorConfig : IEntityTypeConfiguration<Fluent_BookAuthor>
    {
        public void Configure(EntityTypeBuilder<Fluent_BookAuthor> modelBuilder)
        {
            //Many to Many relation
            modelBuilder.HasKey(ba => new { ba.Author_Id, ba.Book_Id });
            modelBuilder
                .HasOne(b => b.Fluent_Book)
                .WithMany(p => p.Fluent_BookAuthors).HasForeignKey(b => b.Book_Id);
            modelBuilder
                .HasOne(b => b.Fluent_Author)
                .WithMany(p => p.Fluent_BookAuthors).HasForeignKey(b => b.Author_Id);
        }
    }
}
