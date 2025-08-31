document.querySelectorAll(".movie-description-wrapper").forEach(wrapper => {
    const desc = wrapper.querySelector(".movie-description");
    const btn = wrapper.querySelector(".toggle-btn");

    btn.addEventListener("click", () => {
        desc.classList.toggle("expanded");
        btn.textContent = desc.classList.contains("expanded") ? "Thu gọn" : "Xem thêm";
    });
});


let cinema_item = null;

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


document.querySelectorAll('.showtime-item').forEach(item => {
    item.addEventListener('click', () => {
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
    
                    <button class="btn-buy">Mua vé</button>
                </div>
            </div>
        </div>
        `
        document.body.appendChild(popup);

        requestAnimationFrame(() => {
            popup.classList.add("show");
        });

        let board = popup.querySelector("#board");
        let closePopup = popup.querySelector("#close-popup-btn");
        if (!board) return;
        const canvas = new Board(board);
        canvas.init();

        closePopup.addEventListener('click', () => {
            popup.classList.remove("show");
            popup.addEventListener("transitionend", () => popup.remove(), { once: true });
            document.body.removeChild(popup);
        })

    })
})