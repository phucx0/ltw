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

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.HasDefaultSchema("MOVIE_TICKETS");
        modelBuilder.HasDefaultSchema("CINEMA");

        modelBuilder.Entity<Actor>(entity =>
        {
            entity.ToTable("ACTORS");

            entity.Property(e => e.ActorId)
                .HasPrecision(10)
                .HasColumnName("ACTOR_ID");
            entity.Property(e => e.ActorName)
                .HasMaxLength(255)
                .HasColumnName("ACTOR_NAME");
        });

        modelBuilder.Entity<AgeRating>(entity =>
        {
            entity.HasKey(e => e.RatingId);

            entity.ToTable("AGE_RATINGS");

            entity.HasIndex(e => e.RatingCode, "UNI_AGE_RATINGS").IsUnique();

            entity.Property(e => e.RatingId)
                .HasPrecision(10)
                .HasColumnName("RATING_ID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.RatingCode)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("RATING_CODE");
        });

        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("BRANCHES");

            entity.HasIndex(e => e.Address, "UNI_BRANCHES_ADDRESS").IsUnique();

            entity.HasIndex(e => e.Phone, "UNI_BRANCHES_PHONE").IsUnique();

            entity.Property(e => e.BranchId)
                .HasPrecision(10)
                .HasColumnName("BRANCH_ID");
            entity.Property(e => e.Address)
                .HasMaxLength(100)
                .HasColumnName("ADDRESS");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PHONE");
        });

        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.HasKey(e => e.ComboId);

            entity.ToTable("COMBO_ITEMS");

            entity.Property(e => e.ComboId)
                .HasPrecision(10)
                .HasColumnName("COMBO_ID");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("NAME");
            entity.Property(e => e.Price)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("PRICE");
        });

        modelBuilder.Entity<Director>(entity =>
        {
            entity.ToTable("DIRECTORS");

            entity.Property(e => e.DirectorId)
                .HasPrecision(10)
                .HasColumnName("DIRECTOR_ID");
            entity.Property(e => e.DirectorName)
                .HasMaxLength(255)
                .HasColumnName("DIRECTOR_NAME");
        });

        modelBuilder.Entity<Membership>(entity =>
        {
            entity.ToTable("MEMBERSHIPS");

            entity.HasIndex(e => e.UserId, "UNI_MEMBERSHIPS_USER").IsUnique();

            entity.Property(e => e.MembershipId)
                .HasPrecision(10)
                .HasColumnName("MEMBERSHIP_ID");
            entity.Property(e => e.Points)
                .HasPrecision(10)
                .HasDefaultValueSql("0")
                .HasColumnName("POINTS");
            entity.Property(e => e.Tier)
                .HasMaxLength(50)
                .HasColumnName("TIER");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("UPDATED_AT");
            entity.Property(e => e.UserId)
                .HasPrecision(10)
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.User).WithOne(p => p.Membership)
                .HasForeignKey<Membership>(d => d.UserId)
                .HasConstraintName("FK_MEMBERSHIPS_USER");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.ToTable("MOVIES");

            entity.Property(e => e.MovieId)
                .HasPrecision(10)
                .HasColumnName("MOVIE_ID");
            entity.Property(e => e.Description)
                .HasColumnType("CLOB")
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.Duration)
                .HasPrecision(5)
                .HasColumnName("DURATION");
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .HasColumnName("GENRE");
            entity.Property(e => e.ImdbRating)
                .HasColumnType("NUMBER(3,1)")
                .HasColumnName("IMDB_RATING");
            entity.Property(e => e.PosterUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("POSTER_URL");
            entity.Property(e => e.RatingId)
                .HasPrecision(10)
                .HasColumnName("RATING_ID");
            entity.Property(e => e.ReleaseDate)
                .HasColumnType("DATE")
                .HasColumnName("RELEASE_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("TITLE");
            entity.Property(e => e.TrailerUrl)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("TRAILER_URL");

            entity.HasOne(d => d.Rating).WithMany(p => p.Movies)
                .HasForeignKey(d => d.RatingId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_MOVIES_AGE_RATINGS");

            entity.HasMany(d => d.Actors).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieActor",
                    r => r.HasOne<Actor>().WithMany()
                        .HasForeignKey("ActorId")
                        .HasConstraintName("FK_MOVIE_ACTORS_ACTORS"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .HasConstraintName("FK_MOVIE_ACTORS_MOVIES"),
                    j =>
                    {
                        j.HasKey("MovieId", "ActorId");
                        j.ToTable("MOVIE_ACTORS");
                        j.IndexerProperty<int>("MovieId")
                            .HasPrecision(10)
                            .HasColumnName("MOVIE_ID");
                        j.IndexerProperty<int>("ActorId")
                            .HasPrecision(10)
                            .HasColumnName("ACTOR_ID");
                    });

            entity.HasMany(d => d.Directors).WithMany(p => p.Movies)
                .UsingEntity<Dictionary<string, object>>(
                    "MovieDirector",
                    r => r.HasOne<Director>().WithMany()
                        .HasForeignKey("DirectorId")
                        .HasConstraintName("FK_MOVIE_DIRECTORS_DIRECTORS"),
                    l => l.HasOne<Movie>().WithMany()
                        .HasForeignKey("MovieId")
                        .HasConstraintName("FK_MOVIE_DIRECTORS_MOVIES"),
                    j =>
                    {
                        j.HasKey("MovieId", "DirectorId");
                        j.ToTable("MOVIE_DIRECTORS");
                        j.IndexerProperty<int>("MovieId")
                            .HasPrecision(10)
                            .HasColumnName("MOVIE_ID");
                        j.IndexerProperty<int>("DirectorId")
                            .HasPrecision(10)
                            .HasColumnName("DIRECTOR_ID");
                    });
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("PAYMENTS");

            entity.Property(e => e.PaymentId)
                .HasPrecision(10)
                .HasColumnName("PAYMENT_ID");
            entity.Property(e => e.Amount)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("AMOUNT");
            entity.Property(e => e.Method)
                .HasMaxLength(50)
                .HasColumnName("METHOD");
            entity.Property(e => e.PaymentTime)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("PAYMENT_TIME");
            entity.Property(e => e.PromotionId)
                .HasPrecision(10)
                .HasColumnName("PROMOTION_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");
            entity.Property(e => e.TicketId)
                .HasPrecision(10)
                .HasColumnName("TICKET_ID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("TRANSACTION_ID");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PromotionId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_PAYMENTS_PROMOTIONS");

            entity.HasOne(d => d.Ticket).WithMany(p => p.Payments)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_PAYMENTS_TICKETS");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("PROMOTIONS");

            entity.HasIndex(e => e.Code, "UNI_PROMOTIONS_CODE").IsUnique();

            entity.Property(e => e.PromotionId)
                .HasPrecision(10)
                .HasColumnName("PROMOTION_ID");
            entity.Property(e => e.Code)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("CODE");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("DESCRIPTION");
            entity.Property(e => e.DiscountType)
                .HasMaxLength(50)
                .HasColumnName("DISCOUNT_TYPE");
            entity.Property(e => e.EndDate)
                .HasColumnType("DATE")
                .HasColumnName("END_DATE");
            entity.Property(e => e.StartDate)
                .HasColumnType("DATE")
                .HasColumnName("START_DATE");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");
            entity.Property(e => e.Value)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("VALUE");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.ToTable("ROOMS");

            entity.Property(e => e.RoomId)
                .HasPrecision(10)
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.BranchId)
                .HasPrecision(10)
                .HasColumnName("BRANCH_ID");
            entity.Property(e => e.Capacity)
                .HasPrecision(5)
                .HasColumnName("CAPACITY");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
            entity.Property(e => e.RoomType)
                .HasMaxLength(50)
                .HasColumnName("ROOM_TYPE");

            entity.HasOne(d => d.Branch).WithMany(p => p.Rooms)
                .HasForeignKey(d => d.BranchId)
                .HasConstraintName("FK_ROOMS_BRANCH");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("SEATS");

            entity.HasIndex(e => new { e.RoomId, e.SeatRow, e.SeatNumber }, "UNI_SEATS").IsUnique();

            entity.Property(e => e.SeatId)
                .HasPrecision(10)
                .HasColumnName("SEAT_ID");
            entity.Property(e => e.RoomId)
                .HasPrecision(10)
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.SeatNumber)
                .HasPrecision(5)
                .HasColumnName("SEAT_NUMBER");
            entity.Property(e => e.SeatRow)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("SEAT_ROW");
            entity.Property(e => e.TypeId)
                .HasPrecision(10)
                .HasColumnName("TYPE_ID");

            entity.HasOne(d => d.Room).WithMany(p => p.Seats)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_SEATS_ROOMS");

            entity.HasOne(d => d.Type).WithMany(p => p.Seats)
                .HasForeignKey(d => d.TypeId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_SEATS_TYPES");
        });

        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.HasKey(e => e.TypeId);

            entity.ToTable("SEAT_TYPES");

            entity.Property(e => e.TypeId)
                .HasPrecision(10)
                .HasColumnName("TYPE_ID");
            entity.Property(e => e.ExtraPrice)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("EXTRA_PRICE");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("NAME");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.ToTable("SHOWTIMES");

            entity.HasIndex(e => new { e.MovieId, e.RoomId, e.StartTime }, "UNI_SHOWTIMES").IsUnique();

            entity.Property(e => e.ShowtimeId)
                .HasPrecision(10)
                .HasColumnName("SHOWTIME_ID");
            entity.Property(e => e.BasePrice)
                .HasColumnType("NUMBER(10,2)")
                .HasColumnName("BASE_PRICE");
            entity.Property(e => e.EndTime)
                .HasColumnType("DATE")
                .HasColumnName("END_TIME");
            entity.Property(e => e.MovieId)
                .HasPrecision(10)
                .HasColumnName("MOVIE_ID");
            entity.Property(e => e.RoomId)
                .HasPrecision(10)
                .HasColumnName("ROOM_ID");
            entity.Property(e => e.StartTime)
                .HasColumnType("DATE")
                .HasColumnName("START_TIME");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.MovieId)
                .HasConstraintName("FK_SHOWTIMES_MOVIES");

            entity.HasOne(d => d.Room).WithMany(p => p.Showtimes)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_SHOWTIMES_ROOMS");
        });

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("TICKETS");

            entity.Property(e => e.TicketId)
                .HasPrecision(10)
                .HasColumnName("TICKET_ID");
            entity.Property(e => e.BookingTime)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("BOOKING_TIME");
            entity.Property(e => e.SeatId)
                .HasPrecision(10)
                .HasColumnName("SEAT_ID");
            entity.Property(e => e.ShowtimeId)
                .HasPrecision(10)
                .HasColumnName("SHOWTIME_ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("STATUS");
            entity.Property(e => e.UserId)
                .HasPrecision(10)
                .HasColumnName("USER_ID");

            entity.HasOne(d => d.Seat).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.SeatId)
                .HasConstraintName("FK_TICKETS_SEATS");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.ShowtimeId)
                .HasConstraintName("FK_TICKETS_SHOWTIMES");

            entity.HasOne(d => d.User).WithMany(p => p.Tickets)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK_TICKETS_USERS");
        });

        modelBuilder.Entity<TicketCombo>(entity =>
        {
            entity.HasKey(e => new { e.TicketId, e.ComboId });

            entity.ToTable("TICKET_COMBOS");

            entity.Property(e => e.TicketId)
                .HasPrecision(10)
                .HasColumnName("TICKET_ID");
            entity.Property(e => e.ComboId)
                .HasPrecision(10)
                .HasColumnName("COMBO_ID");
            entity.Property(e => e.Quantity)
                .HasPrecision(5)
                .HasColumnName("QUANTITY");

            entity.HasOne(d => d.Combo).WithMany(p => p.TicketCombos)
                .HasForeignKey(d => d.ComboId)
                .HasConstraintName("FK_TICKET_COMBOS_COMBO");

            entity.HasOne(d => d.Ticket).WithMany(p => p.TicketCombos)
                .HasForeignKey(d => d.TicketId)
                .HasConstraintName("FK_TICKET_COMBOS_TICKET");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("USERS");

            entity.HasIndex(e => e.Email, "UNI_USERS_EMAIL").IsUnique();

            entity.HasIndex(e => e.Phone, "UNI_USERS_PHONE").IsUnique();

            entity.Property(e => e.UserId)
                .HasPrecision(10)
                .HasColumnName("USER_ID");
            entity.Property(e => e.Birthday)
                .HasColumnType("DATE")
                .HasColumnName("BIRTHDAY");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("SYSDATE")
                .HasColumnType("DATE")
                .HasColumnName("CREATED_AT");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("EMAIL");
            entity.Property(e => e.FullName)
                .HasMaxLength(50)
                .HasColumnName("FULL_NAME");
            entity.Property(e => e.IsActive)
                .HasDefaultValueSql("1")
                .HasColumnType("NUMBER(1)")
                .HasColumnName("IS_ACTIVE");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("PASSWORD_HASH");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("PHONE");
            entity.Property(e => e.Role)
                .HasMaxLength(50)
                .HasDefaultValueSql("'user'")
                .HasColumnName("ROLE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
