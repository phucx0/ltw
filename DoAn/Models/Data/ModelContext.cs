using DoAn.Models.Accounts;
using DoAn.Models.Booking;
using DoAn.Models.Cinema;
using DoAn.Models.Movies;
using DoAn.Models.Payments;
using Microsoft.EntityFrameworkCore;

namespace DoAn.Models.Data;

public partial class ModelContext : DbContext
{
    private readonly static string server = "localhost";
    private readonly static string database = "MOVIE_TICKET";
    private readonly static string user_id = "sa";
    private readonly static string password = "123";
    private readonly static string trustServerCertificate = "True";
    private static string connectionString = $"Server={server};Database={database};User Id={user_id};Password={password};TrustServerCertificate={trustServerCertificate};";
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
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRolePermission> UserRolePermissions { get; set; }
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
        => optionsBuilder.UseSqlServer(connectionString);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ========================
        // 1. Branches (Chi nhánh)
        // ========================
        modelBuilder.Entity<Branch>(entity =>
        {
            entity.ToTable("branches");
            entity.HasKey(b => b.BranchId);

            entity.Property(b => b.BranchId).HasColumnName("branch_id");
            entity.Property(b => b.Name).HasColumnName("name");
            entity.Property(b => b.Address).HasColumnName("address");
            entity.Property(b => b.Phone).HasColumnName("phone");

            //entity.HasIndex(b => b.Name).IsUnique().HasDatabaseName("uni_branches_name");
            entity.HasIndex(b => b.Address).IsUnique().HasDatabaseName("uni_branches_address");
            entity.HasIndex(b => b.Phone).IsUnique().HasDatabaseName("uni_branches_phone");

        });

        //modelBuilder.Entity<Branch>()
        //    .HasIndex(b => b.Address)
        //    .IsUnique();

        //modelBuilder.Entity<Branch>()
        //    .HasIndex(b => b.Phone)
        //    .IsUnique();

        // ========================
        // 2. RoomTypes (Loại phòng)
        // ========================
        modelBuilder.Entity<RoomType>(entity =>
        {
            entity.ToTable("room_types");
            entity.HasKey(rt => rt.RoomTypeId);

            entity.Property(rt => rt.RoomTypeId).HasColumnName("room_type_id");
            entity.Property(rt => rt.TypeName).HasColumnName("type_name");
            entity.Property(rt => rt.BasePrice).HasColumnName("base_price").HasColumnType("decimal(10,2)");

            entity.HasIndex(rt => rt.TypeName).IsUnique().HasDatabaseName("uni_room_type_name");
        });

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

        //modelBuilder.Entity<Room>()
        //    .HasCheckConstraint("chk_rooms_capacity", "capacity > 0");

        // ========================
        // 4. SeatTypes (Loại ghế)
        // ========================
        modelBuilder.Entity<SeatType>(entity =>
        {
            entity.ToTable("seat_types");
            entity.HasKey(st => st.TypeId);

            entity.Property(st => st.TypeId).HasColumnName("type_id");
            entity.Property(st => st.Name).HasColumnName("name");
            entity.Property(st => st.ExtraPrice).HasColumnName("extra_price").HasColumnType("decimal(10,2)");
        });

        //modelBuilder.Entity<SeatType>()
        //    .Property(st => st.ExtraPrice);

        //modelBuilder.Entity<SeatType>()
        //    .HasCheckConstraint("chk_seat_types_price", "extra_price >= 0");

        // ========================
        // 5. Seats (Ghế)
        // ========================
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("seats");
            entity.HasKey(s => s.SeatId);

            entity.HasOne(s => s.Room)
            .WithMany(r => r.Seats)
            .HasForeignKey(s => s.RoomId);

            entity.HasOne(s => s.SeatType)
            .WithMany(st => st.Seats)
            .HasForeignKey(s => s.TypeId);

            entity.HasIndex(s => new { s.RoomId, s.SeatRow, s.SeatNumber })
            .IsUnique();
        });

        //modelBuilder.Entity<Seat>()
        //    .HasOne(s => s.Room)
        //    .WithMany(r => r.Seats)
        //    .HasForeignKey(s => s.RoomId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Seat>()
        //    .HasOne(s => s.SeatType)
        //    .WithMany(st => st.Seats)
        //    .HasForeignKey(s => s.TypeId)
        //    .OnDelete(DeleteBehavior.SetNull);

        //modelBuilder.Entity<Seat>()
        //    .HasIndex(s => new { s.RoomId, s.SeatRow, s.SeatNumber })
        //    .IsUnique();

        //modelBuilder.Entity<Seat>()
        //    .HasCheckConstraint("chk_seats_number", "seat_number > 0");

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
            .Property(m => m.ImdbRating).HasColumnName("imdb_rating").HasColumnType("decimal(3,1)");
        modelBuilder.Entity<Movie>()
            .HasOne(m => m.AgeRating)
            .WithMany(ar => ar.Movies)
            .HasForeignKey(m => m.RatingId)
            .OnDelete(DeleteBehavior.SetNull);

        //modelBuilder.Entity<Movie>()
        //    .HasCheckConstraint("chk_movies_imdb_rating", "imdb_rating BETWEEN 0 AND 10");

        //modelBuilder.Entity<Movie>()
        //    .HasCheckConstraint("chk_movies_duration", "duration > 0");

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
        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.ToTable("showtimes");
            entity.HasKey(s => s.ShowtimeId);

            entity.Property(s => s.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(s => s.MovieId).HasColumnName("movie_id");
            entity.Property(s => s.RoomId).HasColumnName("room_id");
            entity.Property(s => s.StartTime).HasColumnName("start_time").HasColumnType("datetime");
            entity.Property(s => s.EndTime).HasColumnName("end_time").HasColumnType("datetime");
            entity.Property(s => s.BasePrice).HasColumnName("base_price").HasColumnType("decimal(10,2)");

            entity.HasIndex(s => new { s.MovieId, s.RoomId, s.StartTime }).IsUnique().HasDatabaseName("uni_showtimes");

            entity.HasOne(s => s.Movie)
            .WithMany(m => m.Showtimes)
            .HasForeignKey(s => s.MovieId);

            entity.HasOne(s => s.Room)
            .WithMany(r => r.Showtimes)
            .HasForeignKey(s => s.RoomId);
        });
            

        //modelBuilder.Entity<Showtime>()
        //    .HasOne(s => s.Movie)
        //    .WithMany(m => m.Showtimes)
        //    .HasForeignKey(s => s.MovieId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Showtime>()
        //    .HasOne(s => s.Room)
        //    .WithMany(r => r.Showtimes)
        //    .HasForeignKey(s => s.RoomId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Showtime>()
        //    .HasCheckConstraint("chk_showtimes_time", "end_time > start_time");

        //modelBuilder.Entity<Showtime>()
        //    .HasCheckConstraint("chk_showtimes_base_price", "base_price >= 0");

        //modelBuilder.Entity<Showtime>()
        //    .HasIndex(s => new { s.MovieId, s.RoomId, s.StartTime })
        //    .IsUnique();

        // ========================
        // 11. Permissions (Vai trò người dùng)
        // ========================
        modelBuilder.Entity<Permission>()
            .HasKey(p => p.PermissionId);

        // ========================
        // 12. UserRolePermissions (Vai trò người dùng)
        // ========================
        modelBuilder.Entity<UserRolePermission>(entity =>
        {
            entity.HasKey(urp => new { urp.RoleId, urp.PermissionId });
            entity.HasOne<UserRole>(urp => urp.Role)
                  .WithMany(ur => ur.RolePermissions)
                  .HasForeignKey(urp => urp.RoleId)
                  .HasConstraintName("fk_user_role_permissions_roles");

            entity.HasOne<Permission>(urp => urp.Permission)
                  .WithMany(ur => ur.RolePermissions)
                  .HasForeignKey(urp => urp.PermissionId)
                  .HasConstraintName("fk_user_role_permissions_permissions");
        });
            

        // ========================
        // 13. UserRoles (Vai trò người dùng)
        // ========================
        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.ToTable("user_roles");

            entity.HasKey(ur => ur.RoleId);

            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        // ========================
        // 14. Users (Người dùng)
        // ========================
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(u => u.UserId);

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.PasswordHash).HasColumnName("password_hash");
            entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.ToTable(tb => tb.HasTrigger("trg_create_membership"));

            entity.HasOne(u => u.Role).WithMany(ur => ur.Users).HasForeignKey(u => u.RoleId).HasConstraintName("fk_users_roles");

            entity.HasIndex(u => u.Email).IsUnique().HasDatabaseName("uni_users_email");
            entity.HasIndex(u => u.Phone).IsUnique().HasDatabaseName("uni_users_phone");
        });

        //modelBuilder.Entity<User>()
        //    .HasOne(u => u.Role)
        //    .WithMany(ur => ur.Users)
        //    .HasForeignKey(u => u.RoleId);

        //modelBuilder.Entity<User>()
        //    .HasCheckConstraint("chk_users_birthday", "birthday <= GETDATE()");

        //modelBuilder.Entity<User>()
        //    .HasIndex(u => u.Email)
        //    .IsUnique();

        //modelBuilder.Entity<User>()
        //    .HasIndex(u => u.Phone)
        //    .IsUnique();

        // ========================
        // 15. Promotions (Khuyến mãi)
        // ========================
        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("promotions");
            entity.HasKey(p => p.PromotionId);

            entity.Property(e => e.PromotionId).HasColumnName("promotion_id");
            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.DiscountType).HasColumnName("discount_type");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Value).HasColumnName("value").HasColumnType("decimal(10,2)");
            entity.Property(e => e.StartDate).HasColumnName("start_date").HasColumnType("datetime");
            entity.Property(e => e.EndDate).HasColumnName("end_date").HasColumnType("datetime");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasIndex(p => p.Code).IsUnique().HasDatabaseName("uni_promotions_code");
        });

        //modelBuilder.Entity<Promotion>()
        //    .HasIndex(p => p.Code)
        //    .IsUnique();

        //modelBuilder.Entity<Promotion>()
        //    .HasCheckConstraint("chk_promotions_value", "value >= 0");

        //modelBuilder.Entity<Promotion>()
        //    .HasCheckConstraint("chk_promotions_date", "end_date >= start_date");

        //modelBuilder.Entity<Promotion>()
        //    .HasCheckConstraint("chk_promotions_status", "status IN ('active', 'expired', 'upcoming')");

        // ========================
        // 16. Tickets (Vé)
        // ========================
        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.ToTable("tickets");
            entity.HasKey(t => t.TicketId);

            entity.Property(t => t.TicketId).HasColumnName("ticket_id");
            entity.Property(t => t.BookingTime).HasColumnName("booking_time");
            entity.Property(t => t.ShowtimeId).HasColumnName("showtime_id");
            entity.Property(t => t.SeatId).HasColumnName("seat_id");
            entity.Property(t => t.UserId).HasColumnName("user_id");

            entity.HasOne(t => t.Showtime).WithMany(s => s.Tickets).HasForeignKey(t => t.ShowtimeId);
            entity.HasOne(t => t.Seat).WithMany(s => s.Tickets).HasForeignKey(t => t.SeatId);
            entity.HasOne(t => t.User).WithMany(u => u.Tickets).HasForeignKey(t => t.UserId);
        });

        //modelBuilder.Entity<Ticket>()
        //    .HasOne(t => t.Showtime)
        //    .WithMany(s => s.Tickets)
        //    .HasForeignKey(t => t.ShowtimeId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Ticket>()
        //    .HasOne(t => t.Seat)
        //    .WithMany(s => s.Tickets)
        //    .HasForeignKey(t => t.SeatId);

        //modelBuilder.Entity<Ticket>()
        //    .HasOne(t => t.User)
        //    .WithMany(u => u.Tickets)
        //    .HasForeignKey(t => t.UserId)
        //    .OnDelete(DeleteBehavior.SetNull);

        //modelBuilder.Entity<Ticket>()
        //    .HasCheckConstraint("chk_tickets_status", "status IN ('booked', 'canceled', 'used')");

        //modelBuilder.Entity<Ticket>()
        //    .HasCheckConstraint("chk_tickets_price", "price >= 0");

        // ========================
        // 17. TicketCombos (Combo vé)
        // ========================
        modelBuilder.Entity<TicketCombo>(entity =>
        {
            entity.ToTable("ticket_combos");

            entity.HasKey(tc => new { tc.TicketId, tc.ComboId });

            entity.HasOne(tc => tc.Ticket)
            .WithMany(t => t.TicketCombos)
            .HasForeignKey(tc => tc.TicketId);

            entity.HasOne(tc => tc.ComboItem)
            .WithMany(ci => ci.TicketCombos)
            .HasForeignKey(tc => tc.ComboId);
        });

        //modelBuilder.Entity<TicketCombo>()
        //    .HasOne(tc => tc.Ticket)
        //    .WithMany(t => t.TicketCombos)
        //    .HasForeignKey(tc => tc.TicketId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<TicketCombo>()
        //    .HasOne(tc => tc.ComboItem)
        //    .WithMany(ci => ci.TicketCombos)
        //    .HasForeignKey(tc => tc.ComboId)
        //    .OnDelete(DeleteBehavior.Cascade);

        // ========================
        // 18. Payments (Thanh toán)
        // ========================
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("payments");
            entity.HasKey(p => p.PaymentId);

            entity.Property(p => p.PaymentId).HasColumnName("payment_id");
            entity.Property(p => p.TicketId).HasColumnName("ticket_id");
            entity.Property(p => p.PromotionId).HasColumnName("promotion_id");
            entity.Property(p => p.Amount).HasColumnName("amount").HasColumnType("decimal(10,2)");
            entity.Property(p => p.Method).HasColumnName("method");
            entity.Property(p => p.Status).HasColumnName("status");
            entity.Property(p => p.TransactionId).HasColumnName("transaction_id");
            entity.Property(p => p.PaymentTime).HasColumnName("payment_time");

            entity.HasOne(p => p.Ticket)
            .WithMany(t => t.Payments)
            .HasForeignKey(p => p.TicketId);

            entity.HasOne(p => p.Promotion)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.PromotionId);

        });

        //modelBuilder.Entity<Payment>()
        //    .HasOne(p => p.Ticket)
        //    .WithMany(t => t.Payments)
        //    .HasForeignKey(p => p.TicketId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Payment>()
        //    .HasOne(p => p.Promotion)
        //    .WithMany(p => p.Payments)
        //    .HasForeignKey(p => p.PromotionId)
        //    .OnDelete(DeleteBehavior.SetNull);

        //modelBuilder.Entity<Payment>()
        //    .HasCheckConstraint("chk_payments_status", "status IN ('pending', 'paid', 'canceled')");

        //modelBuilder.Entity<Payment>()
        //    .HasCheckConstraint("chk_payments_amount", "amount >= 0");

        // ========================
        // 19. MembershipTiers (Cấp bậc thành viên)
        // ========================
        modelBuilder.Entity<MembershipTier>(entity =>
        {
            entity.ToTable("membership_tiers");
            entity.HasKey(mt => mt.TierId);

            entity.Property(mt => mt.TierId).HasColumnName("tier_id");
            entity.Property(mt => mt.TierName).HasColumnName("tier_name");
            entity.Property(mt => mt.MinPoints).HasColumnName("min_points");
            entity.Property(mt => mt.MaxPoints).HasColumnName("max_points");
            entity.HasIndex(mt => mt.TierName).IsUnique();
        });

        // ========================
        // 20. Memberships (Thành viên)
        // ========================
        modelBuilder.Entity<Membership>(entity =>
        {
            entity.ToTable("memberships");
            entity.HasKey(m => m.MembershipId);

            entity.Property(m => m.MembershipId).HasColumnName("membership_id");
            entity.Property(m => m.TierId).HasColumnName("tier_id");
            entity.Property(m => m.UserId).HasColumnName("user_id");
            entity.Property(m => m.UpdatedAt).HasColumnName("updated_at");

            entity.HasOne(m => m.User).WithOne(u => u.Membership).HasForeignKey<Membership>(m => m.UserId);
            entity.HasOne(m => m.MembershipTier).WithMany(mt => mt.Memberships).HasForeignKey(m => m.TierId);
        });


        //modelBuilder.Entity<Membership>()
        //    .HasOne(m => m.User)
        //    .WithMany(u => u.Memberships)
        //    .HasForeignKey(m => m.UserId)
        //    .OnDelete(DeleteBehavior.Cascade);

        //modelBuilder.Entity<Membership>()
        //    .HasOne(m => m.MembershipTier)
        //    .WithMany(mt => mt.Memberships)
        //    .HasForeignKey(m => m.TierId);

        //modelBuilder.Entity<Membership>()
        //    .HasCheckConstraint("chk_memberships_points", "points >= 0");

        // ========================
        // 21. ComboItems (Món combo)
        // ========================
        modelBuilder.Entity<ComboItem>(entity =>
        {
            entity.ToTable("combo_items");
            entity.HasKey(ci => ci.ComboId);
            entity.Property(ci => ci.ComboId).HasColumnName("combo_id");
            entity.Property(ci => ci.Name).HasColumnName("name");
            entity.Property(ci => ci.Price).HasColumnName("price").HasColumnType("decimal(10,2)");
            entity.Property(ci => ci.Description).HasColumnName("description");
        });

        //modelBuilder.Entity<ComboItem>()
        //    .HasCheckConstraint("chk_comboitems_price", "price >= 0");

        // ========================
        // 22. TicketPriceHistory (Lịch sử thay đổi giá vé)
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

            //entity.HasCheckConstraint("chk_ticket_price_history_old_new_price", "OldPrice >= 0 AND NewPrice >= 0");
        });
    }

}
