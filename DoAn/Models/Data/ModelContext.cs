using System;
using System.Collections.Generic;
using DoAn.Models.Accounts;
using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Movies;
using DoAn.Models.Payments;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Models.Data;

public partial class ModelContext : DbContext
{
    public ModelContext()
    {
    }

    public ModelContext(DbContextOptions<ModelContext> options) : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }
    public virtual DbSet<AgeRating> AgeRatings { get; set; }
    public virtual DbSet<Branch> Branches { get; set; }
    public virtual DbSet<ComboItem> ComboItems { get; set; }
    public virtual DbSet<Director> Directors { get; set; }
    public virtual DbSet<Membership> Memberships { get; set; }
    public virtual DbSet<Movie> Movies { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Promotion> Promotions { get; set; }
    public virtual DbSet<Room> Rooms { get; set; }
    public virtual DbSet<Seat> Seats { get; set; }
    public virtual DbSet<SeatType> SeatTypes { get; set; }
    public virtual DbSet<Showtime> Showtimes { get; set; }
    public virtual DbSet<Ticket> Tickets { get; set; }
    public virtual DbSet<TicketCombo> TicketCombos { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=LAPTOP-8QIUBP6K\\SQLEXPRESS;Database=MOVIE_TICKET_db;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<TicketCombo>().HasKey(tc => new { tc.TicketId, tc.ComboId });

        modelBuilder.Entity<Branch>().HasIndex(b => b.Address).IsUnique();
        modelBuilder.Entity<Branch>().HasIndex(b => b.Phone).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Phone).IsUnique();
        modelBuilder.Entity<Promotion>().HasIndex(p => p.Code).IsUnique();
        modelBuilder.Entity<Membership>().HasIndex(m => m.UserId).IsUnique();
        modelBuilder.Entity<Seat>().HasIndex(s => new { s.RoomId, s.SeatRow, s.SeatNumber }).IsUnique();
        modelBuilder.Entity<Showtime>().HasIndex(s => new { s.MovieId, s.RoomId, s.StartTime }).IsUnique();
        modelBuilder.Entity<AgeRating>().HasIndex(r => r.RatingCode).IsUnique();

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Actors)
            .WithMany(a => a.Movies)
            .UsingEntity(j => j.ToTable("MovieActors"));

        modelBuilder.Entity<Movie>()
            .HasMany(m => m.Directors)
            .WithMany(d => d.Movies)
            .UsingEntity(j => j.ToTable("MovieDirectors"));

    }
}
