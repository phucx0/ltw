# Movie Ticket Booking System

A full-stack web application for online movie ticket booking, built with ASP.NET (C#) and SQL Server. This project was developed as an academic assignment ("Đồ Án") covering cinema management, seat reservation, payment processing, and a membership rewards system.

---

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Backend | C# / ASP.NET Web Forms |
| Frontend | HTML, CSS, JavaScript |
| Database | Microsoft SQL Server (T-SQL) |
| IDE | Visual Studio |

**Language breakdown:** HTML 44.9% · C# 32.2% · CSS 9.4% · T-SQL 8.0% · JavaScript 5.5%

---

## Features

### For Users
- Browse movies currently showing or coming soon
- View movie details: poster, trailer, genre, IMDB rating, cast, and director
- Select a showtime and cinema branch
- Interactive seat picker with real-time seat hold (prevents double booking)
- Add food & drink combos to the order
- Apply promotional discount codes at checkout
- Pay via QR code (SePay integration)
- View booking history and ticket status
- Earn and track membership points automatically

### For Admins / Staff
- Manage movies, actors, directors, and age ratings
- Create and update showtimes across multiple branches
- Manage cinema branches, screening rooms, and seat layouts
- Create and manage promotional campaigns
- View revenue reports and booking statistics
- Role-based access control (Admin, Manager, Staff, User)

---

## Database Schema

The database (`MOVIE_TICKET`) is organized into nine logical modules:

### 1. Branches & Rooms
- `branches` — cinema locations (name, address, phone)
- `room_types` — room categories with base prices (Standard, Deluxe, IMAX)
- `rooms` — individual screening rooms linked to a branch and room type

### 2. Seats
- `seat_types` — seat categories with surcharges (Regular, VIP, Couple, Disabled)
- `seats` — individual seats identified by row + number within a room

### 3. Movies
- `movies` — title, description, duration, genre, release date, poster/cover/trailer URLs, IMDB rating, and status (`Now Showing` / `Coming Soon`)
- `age_ratings` — rating codes (G, PG, PG-13, R, NC-17) with descriptions
- `actors` / `directors` — person entities
- `movie_actors` / `movie_directors` — many-to-many relationships

### 4. Showtimes
- `showtimes` — links a movie to a room with a start/end time and base price

### 5. Users & Permissions
- `users` — registered accounts with hashed passwords and active status
- `user_roles` — Admin, Manager, Staff, User
- `permissions` — fine-grained action permissions (e.g. `CREATE_MOVIE`, `BUY_TICKET`)
- `user_role_permissions` — maps roles to their allowed permissions

### 6. Promotions
- `promotions` — discount codes with type (percentage/fixed), value, validity dates, and status

### 7. Bookings & Tickets
- `bookings` — a booking session per user per showtime
- `booking_seats` — individual seat assignments within a booking, with computed price
- `tickets` — ticket records linked to a booking (status: `pending`, `booked`, `used`, `canceled`)
- `seat_hold` — temporary seat reservation with expiry time to prevent race conditions
- `payments` — payment records linked to a booking (method, status, transaction ID)
- `ticket_price_history` — audit log of price changes on showtimes (via trigger)

### 8. Membership
- `membership_tiers` — Bronze, Silver, Gold, Platinum (defined by point thresholds)
- `memberships` — per-user points balance and current tier

### 9. Combos
- `combo_items` — food & drink packages with price
- `ticket_combos` — combo items attached to a ticket with quantity

---

## Key Database Logic

### Triggers
| Trigger | Event | Behavior |
|---------|-------|----------|
| `trg_create_membership` | `AFTER INSERT` on `users` | Auto-creates a membership record for every new user |
| `trg_update_membership_points` | `AFTER INSERT` on `tickets` | Awards 10 points per ticket and upgrades tier if threshold is reached |
| `trg_users_default_role` | `AFTER INSERT` on `users` | Assigns the `user` role automatically if none is specified |
| `trg_ticket_price_history` | `AFTER UPDATE` on `showtimes` | Logs old and new prices to `ticket_price_history` when `base_price` changes |

### Stored Procedures
- **`sp_FinalizeBooking`** — atomically converts held seats into a confirmed booking and pending payment within a transaction. Rolls back if the hold has expired.
- **`HoldSeat`** — places a temporary hold on a seat for a given showtime and user, rejecting the request if the seat is already held or booked.

---

## Project Structure

```
ltw/
├── DoAn/               # ASP.NET web application (C# + HTML/CSS/JS)
├── DoAn.sln            # Visual Studio solution file
└── Movie_Ticket.sql    # Full database schema + seed data
```

---

## Getting Started

### Prerequisites
- Visual Studio 2022 (or later)
- SQL Server 2019 (or later) / SQL Server Express
- .NET Framework (version compatible with the solution)

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/phucx0/ltw.git
   cd ltw
   ```

2. **Set up the database**
   - Open SQL Server Management Studio (SSMS)
   - Run `Movie_Ticket.sql` to create the database, tables, triggers, stored procedures, and seed data

3. **Configure the connection string**
   - Open `DoAn.sln` in Visual Studio
   - Update the connection string in `Web.config` to point to your SQL Server instance:
     ```xml
     <connectionStrings>
       <add name="MovieTicketDB"
            connectionString="Server=YOUR_SERVER;Database=MOVIE_TICKET;Trusted_Connection=True;"
            providerName="System.Data.SqlClient" />
     </connectionStrings>
     ```

4. **Run the application**
   - Press `F5` in Visual Studio or use `Ctrl+F5` to start without debugging

---

## Sample Data

The SQL script seeds the database with:
- **7 movies** — including The Shawshank Redemption, The Dark Knight, Inception, and Interstellar
- **3 cinema branches** — Ho Chi Minh City, Hanoi, and Da Nang
- **7 screening rooms** across all branches (Standard, Deluxe, IMAX types)
- **60 seats** in Room A of Branch 1 across rows A–J with mixed seat types
- **14 actors** and **5 directors** with movie associations
- **8 showtimes** spread across branches
- **4 membership tiers** — Bronze (0–999 pts), Silver (1000–4999 pts), Gold (5000–9999 pts), Platinum (10000+ pts)
- **5 age ratings** — G, PG, PG-13, R, NC-17
- **34 permissions** covering all major system actions

---

## Role Permissions

| Role | Permissions |
|------|------------|
| **Admin** | Full access to all permissions |
| **Manager** | Cinema & room management, view reports |
| **Staff** | View-only access to movies, showtimes, cinemas, and rooms |
| **User** | Buy tickets, cancel tickets, view own tickets |

---

## Contributing

This is an academic project. Feel free to fork it and adapt it for your own learning or coursework.

---

## License

No license specified. All rights reserved by the original authors.
