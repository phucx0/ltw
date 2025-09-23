CREATE DATABASE MOVIE_TICKET
DROP DATABASE MOVIE_TICKET
-- =======================
-- 1. Chi nhánh và phòng
-- =======================
CREATE TABLE branches (
    branch_id INT IDENTITY(1,1) NOT NULL,
    name NVARCHAR(50),
    address NVARCHAR(100),
    phone VARCHAR(15),
    CONSTRAINT pk_branches PRIMARY KEY (branch_id),
    CONSTRAINT uni_branches_address UNIQUE (address),
    CONSTRAINT uni_branches_phone UNIQUE (phone)
);

CREATE TABLE room_types (
    room_type_id INT IDENTITY(1,1) NOT NULL,
    type_name NVARCHAR(50),
    base_price DECIMAL(10,2) NOT NULL,
    CONSTRAINT pk_room_types PRIMARY KEY (room_type_id),
    CONSTRAINT uni_room_type_name UNIQUE (type_name)
);

CREATE TABLE rooms (
    room_id INT IDENTITY(1,1) NOT NULL,
    branch_id INT NOT NULL,
    name NVARCHAR(50),
    capacity INT,
    room_type_id INT,
    CONSTRAINT pk_rooms PRIMARY KEY (room_id),
    CONSTRAINT fk_rooms_branch FOREIGN KEY (branch_id) REFERENCES branches(branch_id) ON DELETE CASCADE,
    CONSTRAINT fk_rooms_room_types FOREIGN KEY (room_type_id) REFERENCES room_types(room_type_id)
);

-- =======================
-- 2. Ghế & loại ghế
-- =======================
CREATE TABLE seat_types (
    type_id INT IDENTITY(1,1) NOT NULL,
    name NVARCHAR(50),
    extra_price DECIMAL(10,2) DEFAULT 0,
    CONSTRAINT pk_seat_types PRIMARY KEY (type_id)
);

CREATE TABLE seats (
    seat_id INT IDENTITY(1,1) NOT NULL,
    room_id INT NOT NULL,
    type_id INT,
    seat_row VARCHAR(5),
    seat_number INT,
    CONSTRAINT pk_seats PRIMARY KEY (seat_id),
    CONSTRAINT fk_seats_rooms FOREIGN KEY (room_id) REFERENCES rooms(room_id) ON DELETE CASCADE,
    CONSTRAINT fk_seats_types FOREIGN KEY (type_id) REFERENCES seat_types(type_id) ON DELETE SET NULL,
    CONSTRAINT uni_seats UNIQUE (room_id, seat_row, seat_number)
);

-- =======================
-- 3. Phim, diễn viên, đạo diễn
-- =======================
CREATE TABLE age_ratings (
    rating_id INT IDENTITY(1,1) NOT NULL,
    rating_code VARCHAR(10),
    description NVARCHAR(255),
    CONSTRAINT pk_age_ratings PRIMARY KEY (rating_id),
    CONSTRAINT uni_age_ratings UNIQUE (rating_code)
);

CREATE TABLE movies (
    movie_id INT IDENTITY(1,1) NOT NULL,
    title NVARCHAR(255),
    description NVARCHAR(MAX),
    duration INT,
    genre NVARCHAR(50),
    rating_id INT,
    release_date DATETIME,
    poster_url VARCHAR(255),
    cover_url VARCHAR(255),
    trailer_url VARCHAR(255),
    status NVARCHAR(50),
    imdb_rating DECIMAL(3,1),
    CONSTRAINT pk_movies PRIMARY KEY (movie_id),
    CONSTRAINT fk_movies_age_ratings FOREIGN KEY (rating_id) REFERENCES age_ratings(rating_id) ON DELETE SET NULL
);

CREATE TABLE actors (
    actor_id INT IDENTITY(1,1) NOT NULL,
    actor_name NVARCHAR(255),
    CONSTRAINT pk_actors PRIMARY KEY (actor_id)
);

CREATE TABLE directors (
    director_id INT IDENTITY(1,1) NOT NULL,
    director_name NVARCHAR(255),
    CONSTRAINT pk_directors PRIMARY KEY (director_id)
);

CREATE TABLE movie_actors (
    movie_id INT NOT NULL,
    actor_id INT NOT NULL,
    CONSTRAINT pk_movie_actors PRIMARY KEY (movie_id, actor_id),
    CONSTRAINT fk_movie_actors_movies FOREIGN KEY (movie_id) REFERENCES movies(movie_id) ON DELETE CASCADE,
    CONSTRAINT fk_movie_actors_actors FOREIGN KEY (actor_id) REFERENCES actors(actor_id) ON DELETE CASCADE
);

CREATE TABLE movie_directors (
    movie_id INT NOT NULL,
    director_id INT NOT NULL,
    CONSTRAINT pk_movie_directors PRIMARY KEY (movie_id, director_id),
    CONSTRAINT fk_movie_directors_movies FOREIGN KEY (movie_id) REFERENCES movies(movie_id) ON DELETE CASCADE,
    CONSTRAINT fk_movie_directors_directors FOREIGN KEY (director_id) REFERENCES directors(director_id) ON DELETE CASCADE
);

-- =======================
-- 4. Lịch chiếu
-- =======================
CREATE TABLE showtimes (
    showtime_id INT IDENTITY(1,1) NOT NULL,
    movie_id INT NOT NULL,
    room_id INT NOT NULL,
    start_time DATETIME,
    end_time DATETIME,
    base_price DECIMAL(10,2),
    CONSTRAINT pk_showtimes PRIMARY KEY (showtime_id),
    CONSTRAINT fk_showtimes_movies FOREIGN KEY (movie_id) REFERENCES movies(movie_id) ON DELETE CASCADE,
    CONSTRAINT fk_showtimes_rooms FOREIGN KEY (room_id) REFERENCES rooms(room_id) ON DELETE CASCADE,
    CONSTRAINT uni_showtimes UNIQUE (movie_id, room_id, start_time)
);

-- =======================
-- 5. Người dùng
-- =======================
CREATE TABLE user_roles (
    role_id INT IDENTITY(1,1) NOT NULL,
    role_name NVARCHAR(50),
    description NVARCHAR(255),
    CONSTRAINT pk_user_roles PRIMARY KEY (role_id),
    CONSTRAINT uni_user_role_name UNIQUE (role_name)
);

CREATE TABLE users (
    user_id INT IDENTITY(1,1) NOT NULL,
    full_name NVARCHAR(50),
    birthday DATETIME,
    email VARCHAR(50),
    password_hash VARCHAR(100),
    phone VARCHAR(255),
    role_id INT,
    created_at DATETIME DEFAULT GETDATE(),
    is_active BIT DEFAULT 1,
    CONSTRAINT pk_users PRIMARY KEY (user_id),
    CONSTRAINT fk_users_roles FOREIGN KEY (role_id) REFERENCES user_roles(role_id),
    CONSTRAINT uni_users_email UNIQUE (email),
    CONSTRAINT uni_users_phone UNIQUE (phone)
);

-- =======================
-- 6. Khuyến mãi
-- =======================
CREATE TABLE promotions (
    promotion_id INT IDENTITY(1,1) NOT NULL,
    code VARCHAR(20),
    description NVARCHAR(255),
    discount_type NVARCHAR(50),
    value DECIMAL(10,2),
    start_date DATETIME,
    end_date DATETIME,
    status NVARCHAR(50),
    CONSTRAINT pk_promotions PRIMARY KEY (promotion_id),
    CONSTRAINT uni_promotions_code UNIQUE (code)
);

-- =======================
-- 7. Vé & thanh toán
-- =======================
CREATE TABLE tickets (
    ticket_id INT IDENTITY(1,1) NOT NULL,
    showtime_id INT NOT NULL,
    seat_id INT NOT NULL,
    user_id INT,
    booking_time DATETIME DEFAULT GETDATE(),
    status NVARCHAR(50),
    price DECIMAL(10,2),
    CONSTRAINT pk_tickets PRIMARY KEY (ticket_id),
    CONSTRAINT fk_tickets_showtimes FOREIGN KEY (showtime_id) REFERENCES showtimes(showtime_id) ON DELETE CASCADE,
    CONSTRAINT fk_tickets_seats FOREIGN KEY (seat_id) REFERENCES seats(seat_id),
    CONSTRAINT fk_tickets_users FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE SET NULL
);

CREATE TABLE ticket_price_history (
    history_id INT IDENTITY(1,1) NOT NULL,
    showtime_id INT NOT NULL,
    old_price DECIMAL(10,2),
    new_price DECIMAL(10,2),
    changed_at DATETIME DEFAULT GETDATE(),
    changed_by INT,
    CONSTRAINT pk_tph PRIMARY KEY (history_id),
    CONSTRAINT fk_tph_showtime FOREIGN KEY (showtime_id) REFERENCES showtimes(showtime_id),
    CONSTRAINT fk_tph_user FOREIGN KEY (changed_by) REFERENCES users(user_id)
);

-- Trigger log khi thay đổi giá suất chiếu
CREATE TRIGGER trg_ticket_price_history
ON showtimes
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    IF UPDATE(base_price)
    BEGIN
        INSERT INTO ticket_price_history (showtime_id, old_price, new_price, changed_by)
        SELECT d.showtime_id, d.base_price, i.base_price,
               CONVERT(INT, SESSION_CONTEXT(N'user_id'))
        FROM deleted d
        JOIN inserted i ON d.showtime_id = i.showtime_id
        WHERE d.base_price <> i.base_price;
    END
END;

CREATE TABLE payments (
    payment_id INT IDENTITY(1,1) NOT NULL,
    ticket_id INT NOT NULL,
    promotion_id INT,
    amount DECIMAL(10,2),
    method NVARCHAR(50),
    status NVARCHAR(50),
    transaction_id VARCHAR(100),
    payment_time DATETIME DEFAULT GETDATE(),
    CONSTRAINT pk_payments PRIMARY KEY (payment_id),
    CONSTRAINT fk_payments_tickets FOREIGN KEY (ticket_id) REFERENCES tickets(ticket_id) ON DELETE CASCADE,
    CONSTRAINT fk_payments_promotions FOREIGN KEY (promotion_id) REFERENCES promotions(promotion_id) ON DELETE SET NULL
);

-- =======================
-- 8. Membership
-- =======================
CREATE TABLE membership_tiers (
    tier_id INT IDENTITY(1,1) NOT NULL,
    tier_name NVARCHAR(50),
    min_points INT NOT NULL,
    max_points INT NOT NULL,
    CONSTRAINT pk_membership_tier PRIMARY KEY (tier_id),
    CONSTRAINT uni_membership_tier_name UNIQUE (tier_name)
);

CREATE TABLE memberships (
    membership_id INT IDENTITY(1,1) NOT NULL,
    user_id INT NOT NULL,
    points INT DEFAULT 0,
    tier_id INT,
    updated_at DATETIME DEFAULT GETDATE(),
    CONSTRAINT pk_memberships PRIMARY KEY (membership_id),
    CONSTRAINT fk_memberships_user FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT fk_membership_tier FOREIGN KEY (tier_id) REFERENCES membership_tiers(tier_id),
    CONSTRAINT uni_memberships_user UNIQUE (user_id)
);

-- Trigger tự động tạo membership khi tạo mới USERS
CREATE TRIGGER trg_create_membership
ON users
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO memberships (user_id, points, tier_id, updated_at)
    SELECT i.user_id, 0, (
        SELECT TOP 1 tier_id FROM membership_tiers ORDER BY min_points ASC
    ), GETDATE()
    FROM inserted i;
END;

-- Trigger cộng điểm khi mua vé
CREATE TRIGGER trg_update_membership_points
ON tickets
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE m
    SET points = m.points + 10,
        tier_id = (
            SELECT TOP 1 t.tier_id
            FROM membership_tiers t
            WHERE m.points + 10 BETWEEN t.min_points AND t.max_points
        )
    FROM memberships m
    JOIN inserted i ON m.user_id = i.user_id;
END;

-- =======================
-- 9. Combo bắp nước
-- =======================
CREATE TABLE combo_items (
    combo_id INT IDENTITY(1,1) NOT NULL,
    name NVARCHAR(100),
    description NVARCHAR(255),
    price DECIMAL(10,2),
    CONSTRAINT pk_combo_items PRIMARY KEY (combo_id)
);

CREATE TABLE ticket_combos (
    ticket_id INT NOT NULL,
    combo_id INT NOT NULL,
    quantity INT,
    CONSTRAINT pk_ticket_combos PRIMARY KEY (ticket_id, combo_id),
    CONSTRAINT fk_ticket_combos_ticket FOREIGN KEY (ticket_id) REFERENCES tickets(ticket_id) ON DELETE CASCADE,
    CONSTRAINT fk_ticket_combos_combo FOREIGN KEY (combo_id) REFERENCES combo_items(combo_id) ON DELETE CASCADE
);

-- =======================
-- CONSTRAINT CHECK
-- =======================

-- ROOMS
ALTER TABLE rooms
ADD CONSTRAINT chk_rooms_capacity CHECK (capacity > 0);

-- ROOM_TYPES
ALTER TABLE room_types
ADD CONSTRAINT chk_room_type_price CHECK (base_price >= 0);

-- SEATS
ALTER TABLE seats
ADD CONSTRAINT chk_seats_number CHECK (seat_number > 0);

-- SEAT_TYPES
ALTER TABLE seat_types
ADD CONSTRAINT chk_seat_types_price CHECK (extra_price >= 0);

-- MOVIES
ALTER TABLE movies
ADD CONSTRAINT chk_movies_imdb_rating CHECK (imdb_rating BETWEEN 0 AND 10),
    CONSTRAINT chk_movies_duration CHECK (duration > 0);

-- SHOWTIMES
ALTER TABLE showtimes
ADD CONSTRAINT chk_showtimes_time CHECK (end_time > start_time),
    CONSTRAINT chk_showtimes_base_price CHECK (base_price >= 0);

-- USERS
ALTER TABLE users
ADD CONSTRAINT chk_users_birthday CHECK (birthday <= GETDATE());

-- PROMOTIONS
ALTER TABLE promotions
ADD CONSTRAINT chk_promotions_value CHECK (value >= 0),
    CONSTRAINT chk_promotions_date CHECK (end_date >= start_date),
    CONSTRAINT chk_promotions_status CHECK (status IN ('active','expired','upcoming'));

-- TICKETS
ALTER TABLE tickets
ADD CONSTRAINT chk_tickets_status CHECK (status IN ('booked','canceled','used')),
    CONSTRAINT chk_tickets_price CHECK (price >= 0);

-- TICKET_PRICE_HISTORY
ALTER TABLE ticket_price_history
ADD CONSTRAINT chk_tph_old_price CHECK (old_price >= 0),
    CONSTRAINT chk_tph_new_price CHECK (new_price >= 0);

-- PAYMENTS
ALTER TABLE payments
ADD CONSTRAINT chk_payments_status CHECK (status IN ('pending','paid','canceled')),
    CONSTRAINT chk_payments_amount CHECK (amount >= 0);

-- MEMBERSHIPS
ALTER TABLE memberships
ADD CONSTRAINT chk_memberships_points CHECK (points >= 0);

-- COMBO_ITEMS
ALTER TABLE combo_items
ADD CONSTRAINT chk_combo_items_price CHECK (price >= 0);

-- TICKET_COMBOS
ALTER TABLE ticket_combos
ADD CONSTRAINT chk_ticket_combos_quantity CHECK (quantity >= 0);


-- Nhập liệu 2 role cho user_roles
INSERT INTO user_roles (role_name, description)
VALUES (N'user', N'Đặt vé')
UPDATE user_roles
SET description = N'Thêm, xóa phim, rạp, combo'
WHERE role_id = 1

INSERT INTO user_roles (role_name, description)
VALUES (N'user', N'Đặt vé')

SELECT * FROM users
SELECT * FROM user_roles