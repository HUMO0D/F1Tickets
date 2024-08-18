using static System.Reflection.Metadata.BlobBuilder;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using F1Tickets.Models;

namespace F1Tickets.Data
{
  
    public class AppDbContext :  DbContext
       
    { 

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }
        public DbSet<User> User { get; set; }
        public DbSet<Race> Race { get; set; }
        public DbSet<TicketType> TicketType { get; set; }
        public DbSet<Order> Order { get; set; }




        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TicketType>().HasData(
                new TicketType { Id = 1, Type = "Race Day", Price = 100.0m },
                new TicketType { Id = 2, Type = "Full Weekend", Price = 400.0m },
                new TicketType { Id = 3, Type = "F1 PADDOCK CLUB", Price = 1000.0m }

            );

        }



    }


}
