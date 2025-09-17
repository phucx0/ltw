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
        => optionsBuilder.UseSqlServer("Server=localhost;Database=CINEMA;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // USER 
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.ToTable(tb => tb.HasTrigger("trg_create_membership"));
        });

        // BRANCHES
        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.Address).IsUnique();
        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.Phone).IsUnique();

        // ROOMS
        modelBuilder.Entity<Room>()
            .HasOne(r => r.Branch)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Room>()
            .ToTable(t => t.HasCheckConstraint("chk_rooms_capacity", "capacity > 0"));

        // SEATS
        modelBuilder.Entity<Seat>()
            .HasOne(s => s.Room)
            .WithMany(r => r.Seats)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.SeatType)
            .WithMany()
            .HasForeignKey(s => s.TypeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.RoomId, s.SeatRow, s.SeatNumber })
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .ToTable(t => t.HasCheckConstraint("chk_seats_number", "seat_number > 0"));

        // SEAT TYPES
        modelBuilder.Entity<SeatType>()
            .HasKey(s => s.TypeId);

        modelBuilder.Entity<SeatType>()
            .ToTable(t => t.HasCheckConstraint("chk_seat_types_price", "extra_price >= 0"));

        // MOVIES
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.AgeRating)
            .WithMany()
            .HasForeignKey(m => m.RatingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Movie>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("chk_movies_imdb_rating", "imdb_rating BETWEEN 0 AND 10");
                t.HasCheckConstraint("chk_movies_duration", "duration > 0");
            });


        // AGE RATING 
        modelBuilder.Entity<AgeRating>()
            .HasKey(a => a.RatingId);

        // ACTORS, DIRECTORS, MOVIE_ACTORS, MOVIE_DIRECTORS
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });
        modelBuilder.Entity<MovieDirector>()
            .HasKey(md => new { md.MovieId, md.DirectorId });

        // SHOWTIMES
        modelBuilder.Entity<Showtime>()
            .HasIndex(st => new { st.MovieId, st.RoomId, st.StartTime })
            .IsUnique();

        modelBuilder.Entity<Showtime>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("chk_showtimes_time", "end_time > start_time");
                t.HasCheckConstraint("chk_showtimes_base_price", "base_price >= 0");
            });

        // USERS
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone).IsUnique();

        modelBuilder.Entity<User>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("chk_users_is_active", "is_active IN (0,1)");
                t.HasCheckConstraint("chk_users_birthday", "birthday <= GETDATE()");
            });

        // PROMOTIONS
        modelBuilder.Entity<Promotion>()
            .HasIndex(p => p.Code).IsUnique();

        modelBuilder.Entity<Promotion>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("chk_promotions_value", "value >= 0");
                t.HasCheckConstraint("chk_promotions_date", "end_date >= start_date");
                t.HasCheckConstraint("chk_promotions_status", "status IN ('active','expired','upcoming')");
            });

        // TICKETS
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Showtime)
            .WithMany(st => st.Tickets)
            .HasForeignKey(t => t.ShowtimeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Seat)
            .WithMany()
            .HasForeignKey(t => t.SeatId);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .ToTable(t => t.HasCheckConstraint("chk_tickets_status", "status IN ('booked','canceled','used')"));

        // PAYMENTS
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Ticket)
            .WithMany(t => t.Payments)
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Promotion)
            .WithMany()
            .HasForeignKey(p => p.PromotionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Payment>()
            .ToTable(t =>
            {
                t.HasCheckConstraint("chk_payments_status", "status IN ('pending','paid','canceled')");
                t.HasCheckConstraint("chk_payments_amount", "amount >= 0");
            });

        // MEMBERSHIPS
        modelBuilder.Entity<Membership>()
            .HasOne(m => m.User)
            .WithOne(u => u.Membership)
            .HasForeignKey<Membership>(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Membership>()
            .ToTable(t => t.HasCheckConstraint("chk_memberships_points", "points >= 0"));

        // COMBO ITEMS
        modelBuilder.Entity<ComboItem>()
            .HasKey(c => c.ComboId);

        modelBuilder.Entity<ComboItem>()
            .ToTable(t => t.HasCheckConstraint("chk_combo_items_price", "price >= 0"));

        // TICKET_COMBOS
        modelBuilder.Entity<TicketCombo>()
            .HasKey(tc => new { tc.TicketId, tc.ComboId });

        modelBuilder.Entity<TicketCombo>()
            .ToTable(t => t.HasCheckConstraint("chk_ticket_combos_quantity", "quantity >= 0"));

        modelBuilder.Entity<TicketCombo>()
            .HasOne(tc => tc.Ticket)
            .WithMany(t => t.TicketCombos)
            .HasForeignKey(tc => tc.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketCombo>()
            .HasOne(tc => tc.ComboItem)
            .WithMany(c => c.TicketCombos)
            .HasForeignKey(tc => tc.ComboId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
