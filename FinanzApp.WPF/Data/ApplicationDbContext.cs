using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace FinanzApp.WPF.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<FinanceEntry> FinanceEntries => Set<FinanceEntry>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=finanzapp.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.UserId);
            modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
            modelBuilder.Entity<FinanceEntry>().HasKey(f => f.EntryId);
            modelBuilder.Entity<FinanceEntry>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(f => f.UserId);

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            var users = new List<User>
            {
                new() { UserId = 1, Username = "Stefan", PasswordHash = "1000" },
                new() { UserId = 2, Username = "Stefan2", PasswordHash = "1200" },
                new() { UserId = 3, Username = "Stefan3", PasswordHash = "1400" },
                new() { UserId = 4, Username = "Stefan4", PasswordHash = "1600" }
            };
            modelBuilder.Entity<User>().HasData(users);

            var entries = new List<FinanceEntry>();
            var random = new Random(0);
            int id = 1;
            foreach (var user in users)
            {
                for (int month = 1; month <= 12; month++)
                {
                    for (int i = 0; i < 30; i++)
                    {
                        var date = new DateTime(2025, month, random.Next(1, DateTime.DaysInMonth(2025, month)));
                        entries.Add(new FinanceEntry
                        {
                            EntryId = id++,
                            UserId = user.UserId,
                            Amount = random.Next(10, 200),
                            Type = (i % 2 == 0) ? EntryType.Income : EntryType.Expense,
                            Category = "General",
                            Description = "Demo",
                            EntryDate = date
                        });
                    }
                }
            }
            modelBuilder.Entity<FinanceEntry>().HasData(entries);
        }
    }
}
