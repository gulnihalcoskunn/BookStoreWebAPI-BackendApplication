using Entities.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.EFCore.Config
{
    public class BookConfig : IEntityTypeConfiguration<Book>
    {
        public void Configure(EntityTypeBuilder<Book> builder)
        {
            builder.HasData(
                new Book { Id = 1, CategoryId = 1, Title = "Sol Ayağım", Price = 10 },
                new Book { Id = 2, CategoryId = 2, Title = "Güzel Çirkin", Price = 15 },
                new Book { Id = 3, CategoryId = 1, Title = "Kelebekler", Price = 20 }
            );
        }
    }
}
