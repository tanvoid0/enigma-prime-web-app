﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using enigma_prime.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace enigma_prime.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return (await base.SaveChangesAsync(acceptAllChangesOnSuccess,
                cancellationToken));
        }
        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            var utcNow = DateTime.UtcNow;

            foreach (var entry in entries)
            {
                // for entities that inherit from BaseEntity,
                // set UpdatedOn / CreatedOn appropriately
                if (entry.Entity is BaseEntity trackable)
                {
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            // set the updated date to "now"
                            trackable.UpdatedAt = utcNow;

                            // mark property as "don't touch"
                            // we don't want to update on a Modify operation
                            entry.Property("CreatedAt").IsModified = false;
                            break;

                        case EntityState.Added:
                            // set both updated and created date to "now"
                            trackable.CreatedAt = utcNow;
                            trackable.UpdatedAt = utcNow;
                            break;
                    }
                }
            }
        }
        
        // protected override void OnModelCreating(ModelBuilder builder)
        // {
        //     base.OnModelCreating(builder);
        //
        //     builder.Entity<ApplicationUserRole>(userRole =>
        //     {
        //         userRole.HasKey(ur => new { ur.UserId, ur.RoleId });
        //
        //         userRole.HasOne(ur => ur.Role)
        //             .WithMany(r => r.UserRoles)
        //             .HasForeignKey(ur => ur.RoleId)
        //             .IsRequired();
        //
        //         userRole.HasOne(ur => ur.User)
        //             .WithMany(r => r.UserRoles)
        //             .HasForeignKey(ur => ur.UserId)
        //             .IsRequired();
        //     });
        // }
        
        public DbSet<Password> Password { get; set; }
    }
}