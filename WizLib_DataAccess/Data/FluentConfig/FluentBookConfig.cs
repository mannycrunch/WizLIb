using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;
using WizLib_Model.Models;

namespace WizLib_DataAccess.Data.FluentConfig
{
    public class FluentBookConfig : IEntityTypeConfiguration<Fluent_Book>
    {
        public void Configure(EntityTypeBuilder<Fluent_Book> modelBuilder)
        {
            //Name of Table

            //Book
            modelBuilder.HasKey(b => b.Book_Id);

            modelBuilder.Property(b => b.ISBN).IsRequired().HasMaxLength(15);
            modelBuilder.Property(b => b.Title).IsRequired();
            modelBuilder.Property(b => b.Price).IsRequired();


            //One to One relation bewteen Book Detail & Book
            modelBuilder
                .HasOne(b => b.Fluent_BookDetail)
                .WithOne(d => d.Fluent_Book).HasForeignKey<Fluent_Book>("BookDetail_Id");
            //One to Many relation between Publisher & Books
            modelBuilder
                .HasOne(b => b.Fluent_Publisher)
                .WithMany(p => p.Fluent_Books).HasForeignKey(b => b.Publisher_Id);
        }
    }
}
