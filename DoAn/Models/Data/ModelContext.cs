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

    public DbSet<Branch> Branches { get; set; }
    public DbSet<RoomType> RoomTypes { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<SeatType> SeatTypes { get; set; }
    public DbSet<Seat> Seats { get; set; }
    public DbSet<AgeRating> AgeRatings { get; set; }
    public DbSet<Movie> Movies { get; set; }
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Director> Directors { get; set; }
    public DbSet<MovieActor> MovieActors { get; set; }
    public DbSet<MovieDirector> MovieDirectors { get; set; }
    public DbSet<Showtime> Showtimes { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<TicketPriceHistory> TicketPriceHistories { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<MembershipTier> MembershipTiers { get; set; }
    public DbSet<Membership> Memberships { get; set; }
    public DbSet<ComboItem> ComboItems { get; set; }
    public DbSet<TicketCombo> TicketCombos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Server=A108PC06;Database=MOVIE_TICKET;User Id=sa;Password=123;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================
        // 1. Branches (Chi nhánh)
        // ========================
        modelBuilder.Entity<Branch>()
            .HasKey(b => b.BranchId);

        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.Address)
            .IsUnique();

        modelBuilder.Entity<Branch>()
            .HasIndex(b => b.Phone)
            .IsUnique();

        // ========================
        // 2. RoomTypes (Loại phòng)
        // ========================
        modelBuilder.Entity<RoomType>()
            .HasKey(rt => rt.RoomTypeId);

        modelBuilder.Entity<RoomType>()
            .HasIndex(rt => rt.TypeName)
            .IsUnique();

        // ========================
        // 3. Rooms (Phòng)
        // ========================
        modelBuilder.Entity<Room>()
            .HasKey(r => r.RoomId);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.Branch)
            .WithMany(b => b.Rooms)
            .HasForeignKey(r => r.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Room>()
            .HasOne(r => r.RoomType)
            .WithMany(rt => rt.Rooms)
            .HasForeignKey(r => r.RoomTypeId);

        modelBuilder.Entity<Room>()
            .HasCheckConstraint("chk_rooms_capacity", "capacity > 0");

        // ========================
        // 4. SeatTypes (Loại ghế)
        // ========================
        modelBuilder.Entity<SeatType>()
            .HasKey(st => st.TypeId);

        modelBuilder.Entity<SeatType>()
            .Property(st => st.ExtraPrice);

        modelBuilder.Entity<SeatType>()
            .HasCheckConstraint("chk_seat_types_price", "extra_price >= 0");

        // ========================
        // 5. Seats (Ghế)
        // ========================
        modelBuilder.Entity<Seat>()
            .HasKey(s => s.SeatId);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.Room)
            .WithMany(r => r.Seats)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Seat>()
            .HasOne(s => s.SeatType)
            .WithMany(st => st.Seats)
            .HasForeignKey(s => s.TypeId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Seat>()
            .HasIndex(s => new { s.RoomId, s.SeatRow, s.SeatNumber })
            .IsUnique();

        modelBuilder.Entity<Seat>()
            .HasCheckConstraint("chk_seats_number", "seat_number > 0");

        // ========================
        // 6. AgeRatings (Độ tuổi)
        // ========================
        modelBuilder.Entity<AgeRating>()
            .HasKey(ar => ar.RatingId);

        modelBuilder.Entity<AgeRating>()
            .HasIndex(ar => ar.RatingCode)
            .IsUnique();

        // ========================
        // 7. Movies (Phim)
        // ========================
        modelBuilder.Entity<Movie>()
            .HasKey(m => m.MovieId);

        modelBuilder.Entity<Movie>()
            .HasOne(m => m.AgeRating)
            .WithMany(ar => ar.Movies)
            .HasForeignKey(m => m.RatingId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Movie>()
            .HasCheckConstraint("chk_movies_imdb_rating", "imdb_rating BETWEEN 0 AND 10");

        modelBuilder.Entity<Movie>()
            .HasCheckConstraint("chk_movies_duration", "duration > 0");

        // =========================
        // 8. Actor (Diễn viên)
        // =========================
        modelBuilder.Entity<Actor>()
            .HasKey(a => a.ActorId);  // Khóa chính cho Actor

        modelBuilder.Entity<Actor>()
            .Property(a => a.ActorName)
            .IsRequired()  // Nếu cần, có thể yêu cầu ActorName không null
            .HasMaxLength(200);  // Đặt độ dài tối đa cho ActorName nếu cần

        // =========================
        // 9. Director (Đạo diễn)
        // =========================
        modelBuilder.Entity<Director>()
            .HasKey(d => d.DirectorId);  // Khóa chính cho Director

        modelBuilder.Entity<Director>()
            .Property(d => d.DirectorName)
            .IsRequired()  // Nếu cần, có thể yêu cầu DirectorName không null
            .HasMaxLength(200);  // Đặt độ dài tối đa cho DirectorName nếu cần

        // ========================
        // 8. MovieActors (Diễn viên)
        // ========================
        modelBuilder.Entity<MovieActor>()
            .HasKey(ma => new { ma.MovieId, ma.ActorId });

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Movie)
            .WithMany(m => m.MovieActors)
            .HasForeignKey(ma => ma.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieActor>()
            .HasOne(ma => ma.Actor)
            .WithMany(a => a.MovieActors)
            .HasForeignKey(ma => ma.ActorId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // 9. MovieDirectors (Đạo diễn)
        // ========================
        modelBuilder.Entity<MovieDirector>()
            .HasKey(md => new { md.MovieId, md.DirectorId });

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Movie)
            .WithMany(m => m.MovieDirectors)
            .HasForeignKey(md => md.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MovieDirector>()
            .HasOne(md => md.Director)
            .WithMany(d => d.MovieDirectors)
            .HasForeignKey(md => md.DirectorId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // 10. Showtimes (Lịch chiếu)
        // ========================
        modelBuilder.Entity<Showtime>()
            .HasKey(s => s.ShowtimeId);

        modelBuilder.Entity<Showtime>()
            .HasOne(s => s.Movie)
            .WithMany(m => m.Showtimes)
            .HasForeignKey(s => s.MovieId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Showtime>()
            .HasOne(s => s.Room)
            .WithMany(r => r.Showtimes)
            .HasForeignKey(s => s.RoomId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Showtime>()
            .HasCheckConstraint("chk_showtimes_time", "end_time > start_time");

        modelBuilder.Entity<Showtime>()
            .HasCheckConstraint("chk_showtimes_base_price", "base_price >= 0");

        modelBuilder.Entity<Showtime>()
            .HasIndex(s => new { s.MovieId, s.RoomId, s.StartTime })
            .IsUnique();

        // ========================
        // 11. UserRoles (Vai trò người dùng)
        // ========================
        modelBuilder.Entity<UserRole>()
            .HasKey(ur => ur.RoleId);

        modelBuilder.Entity<UserRole>()
            .HasIndex(ur => ur.RoleName)
            .IsUnique();

        // ========================
        // 12. Users (Người dùng)
        // ========================
        modelBuilder.Entity<User>()
            .HasKey(u => u.UserId);

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.ToTable(tb => tb.HasTrigger("trg_create_membership"));
        });

        modelBuilder.Entity<User>()
            .HasOne(u => u.Role)
            .WithMany(ur => ur.Users)
            .HasForeignKey(u => u.RoleId);

        modelBuilder.Entity<User>()
            .HasCheckConstraint("chk_users_birthday", "birthday <= GETDATE()");

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Phone)
            .IsUnique();

        // ========================
        // 13. Promotions (Khuyến mãi)
        // ========================
        modelBuilder.Entity<Promotion>()
            .HasKey(p => p.PromotionId);

        modelBuilder.Entity<Promotion>()
            .HasIndex(p => p.Code)
            .IsUnique();

        modelBuilder.Entity<Promotion>()
            .HasCheckConstraint("chk_promotions_value", "value >= 0");

        modelBuilder.Entity<Promotion>()
            .HasCheckConstraint("chk_promotions_date", "end_date >= start_date");

        modelBuilder.Entity<Promotion>()
            .HasCheckConstraint("chk_promotions_status", "status IN ('active', 'expired', 'upcoming')");

        // ========================
        // 14. Tickets (Vé)
        // ========================
        modelBuilder.Entity<Ticket>()
            .HasKey(t => t.TicketId);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Showtime)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.ShowtimeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Seat)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.SeatId);

        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.User)
            .WithMany(u => u.Tickets)
            .HasForeignKey(t => t.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Ticket>()
            .HasCheckConstraint("chk_tickets_status", "status IN ('booked', 'canceled', 'used')");

        modelBuilder.Entity<Ticket>()
            .HasCheckConstraint("chk_tickets_price", "price >= 0");

        // ========================
        // 15. TicketCombos (Combo vé)
        // ========================
        modelBuilder.Entity<TicketCombo>()
            .HasKey(tc => new { tc.TicketId, tc.ComboId });

        modelBuilder.Entity<TicketCombo>()
            .HasOne(tc => tc.Ticket)
            .WithMany(t => t.TicketCombos)
            .HasForeignKey(tc => tc.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<TicketCombo>()
            .HasOne(tc => tc.ComboItem)
            .WithMany(ci => ci.TicketCombos)
            .HasForeignKey(tc => tc.ComboId)
            .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // 16. Payments (Thanh toán)
        // ========================
        modelBuilder.Entity<Payment>()
            .HasKey(p => p.PaymentId);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Ticket)
            .WithMany(t => t.Payments)
            .HasForeignKey(p => p.TicketId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Promotion)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.PromotionId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Payment>()
            .HasCheckConstraint("chk_payments_status", "status IN ('pending', 'paid', 'canceled')");

        modelBuilder.Entity<Payment>()
            .HasCheckConstraint("chk_payments_amount", "amount >= 0");

        // ========================
        // 17. MembershipTiers (Cấp bậc thành viên)
        // ========================
        modelBuilder.Entity<MembershipTier>()
            .HasKey(mt => mt.TierId);

        modelBuilder.Entity<MembershipTier>()
            .HasIndex(mt => mt.TierName)
            .IsUnique();

        // ========================
        // 18. Memberships (Thành viên)
        // ========================
        modelBuilder.Entity<Membership>()
            .HasKey(m => m.MembershipId);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.User)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Membership>()
            .HasOne(m => m.MembershipTier)
            .WithMany(mt => mt.Memberships)
            .HasForeignKey(m => m.TierId);

        modelBuilder.Entity<Membership>()
            .HasCheckConstraint("chk_memberships_points", "points >= 0");

        // ========================
        // 19. ComboItems (Món combo)
        // ========================
        modelBuilder.Entity<ComboItem>()
            .HasKey(ci => ci.ComboId);

        modelBuilder.Entity<ComboItem>()
            .HasCheckConstraint("chk_comboitems_price", "price >= 0");

        // ========================
        // 20. TicketPriceHistory (Lịch sử thay đổi giá vé)
        // ========================
        modelBuilder.Entity<TicketPriceHistory>(entity =>
        {
            entity.HasKey(e => e.HistoryId);

            entity.HasOne(e => e.Showtime)
                  .WithMany()
                  .HasForeignKey(e => e.ShowtimeId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(e => e.OldPrice)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.NewPrice)
                  .HasColumnType("decimal(10,2)")
                  .IsRequired();

            entity.Property(e => e.ChangedAt)
                  .HasColumnType("datetime")
                  .IsRequired()
                  .HasDefaultValueSql("GETDATE()");

            entity.Property(e => e.ChangedBy)
                  .HasColumnName("ChangedBy")
                  .HasMaxLength(50);

            entity.HasCheckConstraint("chk_ticket_price_history_old_new_price", "OldPrice >= 0 AND NewPrice >= 0");
        });
    }

}
