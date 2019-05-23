using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options):base (options)
        {
            
        }
        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }    
        public DbSet<Like> Likes { get; set; }    
        public DbSet<Message> Messages { get; set; }    

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasKey(k => new {k.LikerId, k.LikeeId});
                entity.HasOne(e => e.Liker).WithMany(e=>e.Likees).HasForeignKey(fk=>fk.LikerId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Likee).WithMany(e=>e.Likers).HasForeignKey(fk=>fk.LikeeId).OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(e => e.Sender).WithMany(e => e.MessagesSent).HasForeignKey(fk => fk.SenderId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(e => e.Recipient).WithMany(e => e.MessagesReceived).HasForeignKey(fk => fk.RecipientId).OnDelete(DeleteBehavior.Restrict);
            });
            // modelBuilder.Entity<User>(entity=>{
            //     // entity.HasOne(e=>e.User).WithMany().HasForeignKey(k=>k.UserId).OnDelete(DeleteBehavior.Restrict);
            //     entity.HasMany(e=>e.Photos).WithOne().HasForeignKey("UserId").IsRequired();
            // });
        }
    }
}