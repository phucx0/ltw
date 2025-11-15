import { Board } from "./seats.js";

document.querySelectorAll(".movie-description-wrapper").forEach(wrapper => {
    const desc = wrapper.querySelector(".movie-description");
    const btn = wrapper.querySelector(".toggle-btn");

    btn.addEventListener("click", () => {
        desc.classList.toggle("expanded");
        btn.textContent = desc.classList.contains("expanded") ? "Thu gọn" : "Xem thêm";
    });
});

function load_showtime_item() {
    document.querySelectorAll('.showtime-item').forEach(item => {
        item.addEventListener('click', (e) => {
            const showtimeId = e.currentTarget.dataset.showtimeId;
            console.log(showtimeId);
            const popup = document.createElement("div");
            popup.className = "popup-cover";
            popup.innerHTML = `
            <div class="popup-container">
            <div class="popup-header">
                <h5 class="text-white fw-bold text-center">Mua vé xem phim</h5>
                <i class="bi bi-x-lg text-white" id="close-popup-btn"></i>
            </div>
            <canvas id="board"></canvas>
            <div class="booking-summary">
                <div class="ticket-info">
                    <div class="movie-detail">
                        <div class="movie-title">Tên phim</div>
                        <div class="movie-session">
                            19:30 ~ 21:34 - Hôm nay, 28/08 - Phòng chiếu P4 - 2D Phụ đề
                        </div>
                    </div>

                    <div class="ticket-extra">
                        <div class="d-flex justify-content-between w-100">
                            <div class="ticket-seats">
                                <span class="label">Chỗ ngồi:</span>
                                <span class="value">--</span>
                            </div>
        
                            <div class="ticket-total">
                                <span class="value fs-5">0đ</span>
                            </div>
                        </div>
    
                        <button class="btn-buy" id="book-tickets" >Mua vé</button>
                    </div>
                </div>
            </div>
            `
            document.body.appendChild(popup);

            requestAnimationFrame(() => {
                popup.classList.add("show");
            });
            const bookButton = document.getElementById("book-tickets");

            let board = popup.querySelector("#board");
            let closePopup = popup.querySelector("#close-popup-btn");
            if (!board) return;
            const canvas = new Board(board, showtimeId);
            canvas.init();

            bookButton.addEventListener("click", async () => {
                canvas.selectedSeats.forEach(t => console.log("Ghế: " + t));
                const response = await fetch("/Booking/Booking/CreateBooking", {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json' },
                    body: JSON.stringify({
                        showtimeId: Number(showtimeId),
                        seatIds: Array.from(canvas.selectedSeats),
                    })
                })
                // Kiểm tra status trước khi đọc JSON
                if (response.status === 401) {
                    window.location.href = "/User/Auth/Login";
                    return;
                }

                const result = await response.json();
                console.log(result);
                if (result.success) {
                    setTimeout(() => {
                        window.location.href = `/Booking/Booking/Checkout?bookingId=${result.bookingId}`;
                    }, 300);
                } else {
                    alert(result.message);
                }
            })

            closePopup.addEventListener('click', () => {
                popup.classList.remove("show");
                popup.addEventListener("transitionend", () => popup.remove(), { once: true });
                //document.body.removeChild(popup);
            })
        })
    })
}

// Tải danh sách rạp theo ngày
let cinema_item = null;
document.querySelectorAll('.btn.day').forEach(btn => {
    btn.addEventListener('click', function () {
        const movieId = this.dataset.movie;
        const date = this.dataset.date;

        // Bỏ class active cũ
        document.querySelectorAll('.btn.day').forEach(b => b.classList.remove('active'));
        this.classList.add('active');

        // Gửi request để lấy partial view
        fetch(`/Movie/GetShowtimesPartial?id=${movieId}&date=${date}`)
            .then(response => {
                if (!response.ok) throw new Error('Network error');
                return response.text();
            })
            .then(html => {
                document.getElementById('showtimes-container').innerHTML = html;

                document.querySelectorAll(".cinema-wrapper").forEach(c => {
                    c.classList.add("collapsed");
                })
                document.querySelectorAll(".show-cinema-toggle").forEach(toggleBtn => {
                    toggleBtn.addEventListener('click', () => {
                        document.querySelectorAll(".cinema-wrapper").forEach(c => {
                            c.classList.replace("expanded", "collapsed");
                        })
                        const wrapper = toggleBtn.closest(".cinema-wrapper"); // tìm wrapper cha

                        if (cinema_item !== wrapper) {
                            wrapper.classList.replace("collapsed", "expanded");
                            cinema_item = wrapper;
                        } else {
                            wrapper.classList.replace("expanded", "collapsed");
                            cinema_item = null;
                        }
                    })
                })
                load_showtime_item();
            })
            .catch(err => console.error('Fetch error:', err));
    });
});