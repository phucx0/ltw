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
    trailer_url VARCHAR(555),
    status NVARCHAR(50),
    imdb_rating DECIMAL(3,1),
    CONSTRAINT pk_movies PRIMARY KEY (movie_id),
    CONSTRAINT fk_movies_age_ratings FOREIGN KEY (rating_id) REFERENCES age_ratings(rating_id) ON DELETE SET NULL
);
ALTER TABLE movies ALTER COLUMN status INT;


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
-- 5. Người dùng và phân quyền
-- =======================
CREATE TABLE permissions (
    permission_id INT IDENTITY(1,1) NOT NULL,
    permission_name NVARCHAR(50),
    permission_description NVARCHAR(255),
    CONSTRAINT pk_permission PRIMARY KEY (permission_id),
    CONSTRAINT uni_permission_name UNIQUE (permission_name)
);

CREATE TABLE user_roles (
    role_id INT IDENTITY(1,1) NOT NULL,
    role_name NVARCHAR(50),
    CONSTRAINT pk_user_roles PRIMARY KEY (role_id),
    CONSTRAINT uni_user_role_name UNIQUE (role_name)
);

CREATE TABLE user_role_permissions (
    role_id INT,
    permission_id INT,
    CONSTRAINT pk_user_role_permissions PRIMARY KEY (role_id, permission_id),
    CONSTRAINT fk_user_role_permissions_roles FOREIGN KEY (role_id) REFERENCES user_roles(role_id),
    CONSTRAINT fk_user_role_permissions_permissions FOREIGN KEY (permission_id) REFERENCES permissions(permission_id)
);

GO
CREATE FUNCTION fn_default_user_role()
RETURNS INT
AS
BEGIN
    RETURN (SELECT role_id FROM user_roles WHERE role_name = 'user');
END;
GO

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
    CONSTRAINT uni_users_phone UNIQUE (phone),
    CONSTRAINT df_users_role_id DEFAULT (fn_default_user_role()) FOR role_id
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


-- Nhập liệu test phân quyền users --
-- Thêm các quyền (permissions)
INSERT INTO permissions (permission_name, permission_description) VALUES
-- Quản lý phim
('CREATE_MOVIE', N'Quyền thêm phim mới'),
('UPDATE_MOVIE', N'Quyền cập nhật thông tin phim'),
('DELETE_MOVIE', N'Quyền xóa phim'),
('VIEW_MOVIE', N'Quyền xem danh sách phim'),

-- Quản lý lịch chiếu
('CREATE_SHOWTIME', N'Quyền tạo lịch chiếu mới'),
('UPDATE_SHOWTIME', N'Quyền cập nhật lịch chiếu'),
('DELETE_SHOWTIME', N'Quyền xóa lịch chiếu'),
('VIEW_SHOWTIME', N'Quyền xem lịch chiếu'),

-- Quản lý rạp chiếu
('CREATE_CINEMA', N'Quyền thêm rạp chiếu mới'),
('UPDATE_CINEMA', N'Quyền cập nhật thông tin rạp chiếu'),
('DELETE_CINEMA', N'Quyền xóa rạp chiếu'),
('VIEW_CINEMA', N'Quyền xem danh sách rạp chiếu'),

-- Quản lý phòng chiếu
('CREATE_ROOM', N'Quyền thêm phòng chiếu mới'),
('UPDATE_ROOM', N'Quyền cập nhật thông tin phòng chiếu'),
('DELETE_ROOM', N'Quyền xóa phòng chiếu'),
('VIEW_ROOM', N'Quyền xem danh sách phòng chiếu'),

-- Quản lý vé
('BUY_TICKET', N'Quyền mua vé xem phim'),
('CANCEL_TICKET', N'Quyền hủy vé đã mua'),
('VIEW_TICKET', N'Quyền xem thông tin vé'),

-- Quản lý tài khoản
('CREATE_USER', N'Quyền thêm tài khoản người dùng'),
('UPDATE_USER', N'Quyền chỉnh sửa thông tin người dùng'),
('DELETE_USER', N'Quyền xóa tài khoản người dùng'),
('VIEW_USER', N'Quyền xem danh sách tài khoản'),

-- Quản lý báo cáo & doanh thu
('VIEW_REPORTS', N'Quyền xem báo cáo thống kê'),
('EXPORT_REPORTS', N'Quyền xuất báo cáo doanh thu'),

-- Quản lý khuyến mãi
('CREATE_PROMOTION', N'Quyền thêm khuyến mãi mới'),
('UPDATE_PROMOTION', N'Quyền cập nhật khuyến mãi'),
('DELETE_PROMOTION', N'Quyền xóa khuyến mãi'),
('VIEW_PROMOTION', N'Quyền xem danh sách khuyến mãi'),


-- Quản lý nhân viên
('CREATE_STAFF', N'Quyền thêm nhân viên mới'),
('UPDATE_STAFF', N'Quyền chỉnh sửa thông tin nhân viên'),
('DELETE_STAFF', N'Quyền xóa nhân viên'),
('VIEW_STAFF', N'Quyền xem danh sách nhân viên');


-- Thêm các role (user_roles)
INSERT INTO user_roles (role_name) VALUES ('admin');
INSERT INTO user_roles (role_name) VALUES ('staff');
INSERT INTO user_roles (role_name) VALUES ('user');
INSERT INTO user_roles (role_name) VALUES ('manager');

select * from permissions

-- Gán quyền cho role admin
INSERT INTO user_role_permissions (role_id, permission_id)
SELECT r.role_id, p.permission_id
FROM user_roles r, permissions p
WHERE r.role_name = 'admin';

-- Gán quyền cho role manager (ví dụ chỉ được thêm và cập nhật phim, không được xóa)
INSERT INTO user_role_permissions (role_id, permission_id)
SELECT r.role_id, p.permission_id
FROM user_roles r
JOIN permissions p ON p.permission_name IN ('CREATE_CINEMA', 'UPDATE_CINEMA', 'DELETE_CINEMA', 'VIEW_CINEMA', 'CREATE_ROOM', 'UPDATE_ROOM', 'DELETE_ROOM', 'VIEW_ROOM', 'VIEW_REPORTS')
WHERE r.role_name = 'manager';

-- Gán quyền cho role staff (ví dụ chỉ được thêm và cập nhật phim, không được xóa)
INSERT INTO user_role_permissions (role_id, permission_id)
SELECT r.role_id, p.permission_id
FROM user_roles r
JOIN permissions p ON p.permission_name IN ('VIEW_MOVIE', 'VIEW_SHOWTIME', 'VIEW_CINEMA', 'VIEW_ROOM')
WHERE r.role_name = 'staff';

-- Gán quyền cho role user (chỉ được mua vé)
INSERT INTO user_role_permissions (role_id, permission_id)
SELECT r.role_id, p.permission_id
FROM user_roles r
JOIN permissions p ON p.permission_name IN ( 'BUY_TICKET', 'CANCEL_TICKET', 'VIEW_TICKET')
WHERE r.role_name = 'user';

--select * from users 

--update users 
--set role_id = (select role_id from user_roles where role_name = 'admin')
--where user_id = 10 

-- Thêm các membership tiers
INSERT INTO membership_tiers (tier_name, min_points, max_points) VALUES 
('Bronze', 0, 999),
('Silver', 1000, 4999),
('Gold', 5000, 9999),
('Platinum', 10000, 999999);


-- Thêm 
INSERT INTO age_ratings (rating_code, description)
VALUES
('G', N'General Audiences – Phù hợp cho mọi lứa tuổi'),
('PG', N'Parental Guidance Suggested – Có một số nội dung cha mẹ cần lưu ý'),
('PG-13', N'Parents Strongly Cautioned – Một số nội dung không phù hợp cho trẻ dưới 13 tuổi'),
('R', N'Restricted – Trẻ dưới 17 tuổi cần có cha mẹ hoặc người giám hộ đi cùng'),
('NC-17', N'Adults Only – Chỉ dành cho khán giả từ 18 tuổi trở lên');


-- Thêm danh sách phim
INSERT INTO movies 
(title, description, duration, genre, rating_id, release_date, poster_url, cover_url, trailer_url, status, imdb_rating)
VALUES
('The Shawshank Redemption',
 'Two imprisoned men bond over a number of years, finding solace and eventual redemption through acts of common decency.',
 142, 'Drama', 3, '1994-09-23',
 'https://m.media-amazon.com/images/M/MV5BMDAyY2FhYjctNDc5OS00MDNlLThiMGUtY2UxYWVkNGY2ZjljXkEyXkFqcGc@._V1_.jpg',
 'https://www.hollywoodreporter.com/wp-content/uploads/2018/03/the_shawshank_redemption_-_h_-_1994.jpg',
 'https://www.youtube.com/watch?v=NmzuHjWmXOc',
 1, 9.3),

('The Godfather',
 'The aging patriarch of an organized crime dynasty transfers control of his clandestine empire to his reluctant son.',
 175, 'Crime', 3, '1972-03-24',
 'https://i.pinimg.com/736x/96/5c/8b/965c8b35458e5a14b4dacaa0a91b0a77.jpg',
 'https://imgs.search.brave.com/a06gQzxqWP4jzDNrafHNB8KDPXKTQ8K89XdVc6axwlk/rs:fit:860:0:0:0/g:ce/aHR0cHM6Ly9pbWFn/ZXMudGltZXNub3du/ZXdzLmNvbS9waG90/by9tc2lkLTE1Mjc5/MTM1OS8xNTI3OTEz/NTkuanBn',
 'https://www.youtube.com/watch?v=UaVTIH8mujA',
 1, 9.2),

('The Dark Knight',
 'Batman must accept one of the greatest psychological and physical tests of his ability to fight injustice.',
 152, 'Action', 3, '2008-07-18',
 'https://m.media-amazon.com/images/M/MV5BMTMxNTMwODM0NF5BMl5BanBnXkFtZTcwODAyMTk2Mw@@._V1_.jpg',
 'https://m.media-amazon.com/images/M/MV5BMTM5NjA1OTYyOV5BMl5BanBnXkFtZTcwMzgzMTk2Mw@@._V1_.jpg',
 'https://imdb-video.media-imdb.com/vi324468761/1434659607842-pgv4ql-1563712918369.mp4?Expires=1759060140&Signature=RyUDmTv6FWYHfmFfZAzo-fYEd7dUkDSFAiAPk1QNaqa42jcMRuEz4DvKjjPe2cSDQzSebQJ144d4a4qkCNijzRlkNDcF1eVyOSMPhRmB~PsGr04US9DGKKGHCbC~dp28vZJQXsc~Txr0anhFOJqimLjj3nK~MHDWcJScrMM1h9ilVhtYYLlNWdDZnYXpADriXsoR-xoXKb4R6OEacE4pQKauGIcsQYxre0ERSAVnp5wQfyDH7q5y4jVa8JAmsHfht4Upm-vCHzRoxnsbWZnEM0mqnaaQ2xXLkDvc4L-ni~KXguglB-Ka3Q285tV3TzscjxpCAwQMsz1xNKZAhyC-WQ__&Key-Pair-Id=APKAIFLZBVQZ24NQH3KA',
 1, 9.0),

('Pulp Fiction',
 'The lives of two mob hitmen, a boxer, a gangster and his wife intertwine in four tales of violence and redemption.',
 154, 'Crime', 3, '1994-10-14',
 'https://m.media-amazon.com/images/M/MV5BYTViYTE3ZGQtNDBlMC00ZTAyLTkyODMtZGRiZDg0MjA2YThkXkEyXkFqcGc@._V1_.jpg',
 'https://static0.srcdn.com/wordpress/wp-content/uploads/2025/07/quentin-tarantino-s-most-quotable-movie-is-a-major-hit-after-hitting-streaming-platform-for-free.jpg',
 'https://imdb-video.media-imdb.com/vi2620371481/1434659607842-pgv4ql-1450205469045.mp4?Expires=1759060243&Signature=aZsfjHdhY4u~rqSruqrIF-NlBRef9XUBReWlONrUu4lZow77gMIh~R2tfQnZbecZMCFx1sJ6JZBlnQoUCS6mBz89eHNx2SoiGRgPQk3x2-AY1eEsktwXUSwwXvKeEU0lj29cu-JMbG32UWuGnZDWABVCqufYcz0CjfM23NynwO5ekJqqcpX~5RUp6A6MMaN7XjwbSUkfLs46mnFEEeBNuEP-L70JQzxAlc-hyDnNnuyxhMVva1VvwCRjM1chti2uTJW-IiOSGcUCwP2FUA72vRqgcAbRsYvSdinFD6nShzrq6aLQm7Kyh1yFBg7jqoyrQtNZoTzBnDSIWsV3ZZzLTQ__&Key-Pair-Id=APKAIFLZBVQZ24NQH3KA',
 1, 8.9),

('Forrest Gump',
 'The presidencies of Kennedy and Johnson, Vietnam, Watergate, and other history unfold through the perspective of an Alabama man.',
 142, 'Drama', 4, '1994-07-06',
 'https://m.media-amazon.com/images/M/MV5BZGY1NDllYjgtMzg1NS00YWMwLTkxYzgtM2I1MjU0MmMyNzA2XkEyXkFqcGc@._V1_.jpg',
 'https://m.media-amazon.com/images/M/MV5BOTI4NDZjYjQtMjBhZS00NDMxLTljOTktNzBmY2NhOTM1ZjA3XkEyXkFqcGc@._V1_.jpg',
 'https://imdb-video.media-imdb.com/vi3567517977/1434659607842-pgv4ql-1564505606861.mp4?Expires=1759060486&Signature=OP4hQpLq2cTYKhNQuvr1gF-uU7ktetUpOoHXFby1V6CwWsncY8x7qWRtu8mbCf9y6~3INQPwhiysuBpDhi93l1Ble~2qzpfAUzegBippZiYlrjVuC4XWsJJ1DQDJ2aLTDArI7dcguymC1uSNWzRlpyevyDrJQ5ah7V3y227zCtFuDqMoAaAV5DVqagMf~cfmJCKt3EQs5l8gALVS01ch3BY9I6b~jhpRYje1Y-mw21LaBYYSFZocuihi7Y7Z6tfNqj6cqjZsL36JtB8T~znNQfo8NFfyC8L42saQcOYD49UfFNN47h-35TZU90wmhFobfqky85JdacqyskJ-9HHK-Q__&Key-Pair-Id=APKAIFLZBVQZ24NQH3KA',
 1, 8.8),

('Inception',
 'A thief who steals corporate secrets through dream-sharing technology is given the inverse task of planting an idea.',
 148, 'Sci-Fi', 5, '2010-07-16',
 'https://m.media-amazon.com/images/M/MV5BMTMyMzYxMDQ3NV5BMl5BanBnXkFtZTcwNTA1NTcwMw@@._V1_.jpg',
 'https://pad.mymovies.it/cinemanews/2017/147071/coverlg_home.jpg',
 'https://imdb-video.media-imdb.com/vi2959588889/1434659607842-pgv4ql-1596404706743.mp4?Expires=1759060613&Signature=bJRgT5AtDIOiL3QX7Yqp1ekyY33NGbnC~mg8PgWZcJuOcdBO6FP18usy4Lcka6yt5KIgVj3kzWW7E0CyPb6mTc3lCNOM7o87-EWi~K-cFmyqOwZvANRQQHHallPpXZJ~lEFqKuUShTdoO2b8b-WhGj-~PwbB3RtXqxCIKE4fOJEZr1Xz86424N6xGfR6h9hvwaU64s2r59UQofpc1b7dJjL31kMr5U562wy27K-SoBEsxxsr6-9TDctc-CQDDQymPLN87UU2XcvxUa6X68OgofwcBXaCa6vcPCnkAgHU4LMcykzCwssZuFaoty3yGEas49Uhfpsxsbs68RXSyjyOnw__&Key-Pair-Id=APKAIFLZBVQZ24NQH3KA',
 1, 8.8),

('Interstellar',
 'A team of explorers travel through a wormhole in space in an attempt to ensure humanity’s survival.',
 169, 'Sci-Fi', 2, '2014-11-07',
 'https://m.media-amazon.com/images/M/MV5BYzdjMDAxZGItMjI2My00ODA1LTlkNzItOWFjMDU5ZDJlYWY3XkEyXkFqcGc@._V1_.jpg',
 'https://images.bauerhosting.com/legacy/media/6214/2fda/2bc7/18a7/e20d/f5fd/Whatisinterstellar.jpg',
 'https://imdb-video.media-imdb.com/vi1586278169/1434659607842-pgv4ql-1616202363366.mp4?Expires=1759060769&Signature=SS47po6TB5vicWVQIuLAnTwN7lfQYkGBE1H25En84B9NKR3y2YA-szRyUPYfgVsCe-H5-mHVxObY1h4c5rePmbYNWkH-~~MaXYeFxYxIvIE~I5K66yNELf8WFkrSUDt8xbAkcTJSvuhtB3GnrUfIZv6V5pHfRhltwqT7eiukWJShegSAHj1tLDgBJv3uUaA2ZsbDtAC2QRxw4cRKU7INorBs1K~ZvcLITLdhrvH8vaabYJxTJlDlzNZlfwJPgEVU1EooejuGLcBg5iyl~qXiZrYftD6hFppROqKX50e9ojNdzHsA53B5ZWuKvLROiDKJMmL9b8lpDEIEKlWK~bAXDw__&Key-Pair-Id=APKAIFLZBVQZ24NQH3KA',
 1, 8.6)
