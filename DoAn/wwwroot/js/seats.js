// Ô ghế
class Block {
    constructor(x, y, width, height, color) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = color;
        this.label = "";
        this.selected = false;
        this.booked = false;
        this.blockId = null;
    }

    draw(ctx) {
        ctx.fillStyle = this.booked ? "#555555ff" : (this.selected ? "#f09205ff" : this.color);
        ctx.fillRect(this.x, this.y, this.width, this.height);

        ctx.fillStyle = "#fff";
        ctx.font = "12px Arial";
        ctx.textAlign = "center";
        ctx.textBaseline = "middle";
        ctx.fillText(this.label, this.x + this.width / 2, this.y + this.height / 2);
    }

    contains(mx, my) {
        return (
            mx >= this.x && mx <= this.x + this.width &&
            my >= this.y && my <= this.y + this.height
        );
    }
}

export class Board {
    constructor(board, showtimeId) {
        this.board = board;
        this.showtimeId = showtimeId;
        this.map = {};
        this.columnCount = 0;
        this.rowCount = 0;
        this.ctx = board.getContext("2d");
        this.offsetX = 0;
        this.offsetY = 0;
        this.boardWidth = board.clientWidth;
        this.boardHeight = board.clientHeight;

        this.board.width = board.clientWidth;
        this.board.height = board.clientHeight;

        this.spacing = 8;
        this.blockWidth = 0;
        this.blockHeight = 0;

        this.blocks = [];

        this.scale = 1;
        this.minScale = 0.5;
        this.maxScale = 3;  

        this.isDragging = false;
        this.startX, this.startY;

        // Danh sách ghế user đã chọn
        this.selectedSeats = new Set();
    }

    
    configBoard() {
        const lengths = Object.entries(this.map).map(([row, seats]) => ({
            row,
            count: seats.length
        }));
        const maxCount = Math.max(...lengths.map(l => l.count));

        this.columnCount = maxCount;
        this.rowCount = Object.keys(this.map).length;

        const deviceType = getDeviceType();
        if (deviceType === "Mobile") {
            this.spacing = 4;
            this.blockWidth = 24;
            this.blockHeight = this.blockWidth;
        }
        else {
            this.spacing = 4;
            this.blockWidth = (this.boardWidth - (2 * 16 + (this.columnCount - 1) * this.spacing)) / this.columnCount;
            this.blockHeight = this.blockWidth;
        }
    }

    async init() {
        this.map = await this.fetchSeat(this.showtimeId);
        this.configBoard();
        this.load_seat();
        this.draw();
        this.load_pan();
    }

    load_seat() {
        this.blocks = [];
        let rowIndex = 0;
        Object.keys(this.map).forEach(rowKey => {
            const rowSeats = this.map[rowKey];
            let color = "#000";
            rowSeats.forEach(seat => {
                const x = (seat.seatNumber - 1) * (this.blockWidth + this.spacing) + 16;
                const y = rowIndex * (this.blockHeight + this.spacing) + 16;
                const label = seat.seatRow + seat.seatNumber;

                // Cần thêm trường hợp ghế đã bán!
                if (seat.booked) {
                    color = "#555555ff"
                    if (seat.seatType.name == "Couple") {
                        const block = new Block(x * 2 - 16, y, this.blockWidth * 2 + this.spacing, this.blockHeight, color);
                        block.label = label;
                        block.blockId = seat.seatId;
                        block.booked = seat.booked;
                        this.blocks.push(block);
                        return;
                    }
                    
                }
                else if (seat.seatType.name == "Regular") color = "#4C13A8";
                else if (seat.seatType.name == "VIP") color = "#DC030E";
                else if (seat.seatType.name == "Couple") {
                    color = "#F005A2";
                    const block = new Block(x * 2 - 16, y, this.blockWidth * 2 + this.spacing, this.blockHeight, color);
                    block.label = label;
                    block.blockId = seat.seatId;
                    this.blocks.push(block);
                    return;
                }

                const block = new Block(x, y, this.blockWidth, this.blockHeight, color);
                block.label = label;
                block.blockId = seat.seatId;
                block.booked = seat.booked;
                this.blocks.push(block);
            });

            rowIndex++;
        });
    };

    load_seat_type() {
        const seat_type = [
            { "Đã đặt": "#555555ff" },
            { "Ghế đã chọn": "#f09205ff" },
            { "Ghế thường": "#4C13A8" },
            { "Ghế VIP": "#DC030E" },
            { "Ghế đôi": "#F005A2" },
            { "Vùng trung tâm": "#00cc0aff" }
        ];

        seat_type.forEach((item, index) => {
            const type = Object.keys(item)[0];
            const color = item[type];

            const row = Math.floor(index / 3);
            const col = index % 3;

            const baseX = col * 150 + 16; // khoảng cách ngang giữa các cột legend
            const baseY = 40 + row * 32 + this.rowCount * (this.blockHeight + this.spacing);

            const blockSize = 20;
            const block = new Block(baseX, baseY, blockSize, blockSize, color);
            block.draw(this.ctx);

            this.ctx.fillStyle = "white";
            this.ctx.font = "14px Arial";
            this.ctx.textAlign = "left";
            this.ctx.textBaseline = "top";
            this.ctx.fillText(type, baseX + 30, baseY + 4);
        });
    };

    load_pan() {
        // === PC: Pan bằng chuột ===
        this.board.addEventListener("mousedown", e => {
            this.isDragging = true;
            this.startX = e.clientX - this.offsetX;
            this.startY = e.clientY - this.offsetY;
        });

        this.board.addEventListener("mousemove", e => {
            if (this.isDragging) {
                this.offsetX = e.clientX - this.startX;
                this.offsetY = e.clientY - this.startY;
                this.draw();
            }
        });

        this.board.addEventListener("mouseup", () => this.isDragging = false);
        this.board.addEventListener("mouseleave", () => this.isDragging = false);

        // === Mobile: Pan bằng 1 ngón ===
        this.board.addEventListener("touchstart", e => {
            if (e.touches.length === 1) {
                this.isDragging = true;
                startX = e.touches[0].clientX - offsetX;
                startY = e.touches[0].clientY - offsetY;
            }
        }, { passive: false });

        this.board.addEventListener("touchmove", e => {
            if (e.touches.length === 1 && this.isDragging) {
                e.preventDefault();
                this.offsetX = e.touches[0].clientX - this.startX;
                this.offsetY = e.touches[0].clientY - this.startY;
                // limitPan();
                this.draw();
            }
        }, { passive: false });

        this.board.addEventListener("touchend", e => {
            if (e.touches.length === 0) this.isDragging = false;
        });

        // Zoom bằng cuộn chuột
        this.board.addEventListener("wheel", e => {
            e.preventDefault();
            const zoomIntensity = 0.1;

            if (e.deltaY < 0) {
                // zoom in
                this.scale *= 1 + zoomIntensity;
            } else {
                // zoom out
                this.scale *= 1 - zoomIntensity;
            }

            // Giới hạn
            this.scale = Math.min(Math.max(this.scale, this.minScale), this.maxScale);

            this.draw();
        });

        this.board.addEventListener("click", async e => {
            const rect = this.board.getBoundingClientRect();
            const mx = e.clientX - rect.left;
            const my = e.clientY - rect.top;

            // quy đổi về tọa độ logic
            const lx = (mx - this.offsetX) / this.scale;
            const ly = (my - this.offsetY) / this.scale;

            for (const block of this.blocks) {
                if (!block.contains(lx, ly)) continue;

                if (block.booked) return;
                const id = block.blockId;

                if (this.selectedSeats.has(id)) {
                    const response = await fetch(`/SeatHold/Release?seatId=${id}&showtimeId=${this.showtimeId}`, {
                        method: "POST",
                        headers: { 'Content-Type': 'application/json' }
                    });
                    if (response.status === 401) {
                        window.location.href = "/User/Auth/Login";
                    }
                    const result = await response.json();
                    if (result.success) {
                        block.selected = false;
                    }
                    this.selectedSeats.delete(id);
                } else {
                    const response = await fetch(`/SeatHold/Hold?seatId=${id}&showtimeId=${this.showtimeId}`, {
                        method: "POST",
                        headers: { 'Content-Type': 'application/json' }
                    });
                    if (response.status === 401) {
                        window.location.href = "/User/Auth/Login";
                    }
                    if (!response.ok) {
                        console.error("Error:", response.status, response.statusText);
                        return;
                    }
                    const result = await response.json();

                    if (result.success) {
                        block.selected = true;
                        this.selectedSeats.add(id);
                    } else {
                        block.booked = true;
                        block.selected = false;
                        alert(result.message);
                    }
                }

                console.log(`id: ${id} - ${block.label} - ${block.selected} - ${block.booked}`);
            }
            this.draw();
        });
    }

    draw() {
        this.ctx.clearRect(0, 0, this.boardWidth, this.boardHeight);
        this.ctx.save();
        this.ctx.translate(this.offsetX, this.offsetY);
        this.ctx.scale(this.scale, this.scale);
        this.blocks.forEach(block => block.draw(this.ctx));
        this.load_seat_type();
        this.ctx.restore();
    }

    async fetchSeat(id) {
        const response = await fetch(`/Movie/GetSeatsByShowtime?showtimeId=${id}`);
        const result = await response.json();

        const grouped = {};
        result.forEach(seat => {
            const row = seat.seatRow;
            if (!grouped[row]) grouped[row] = [];
            grouped[row].push(seat);
        });

        console.log(grouped);

        return grouped;
    }

}

function getDeviceType() {
    const ua = navigator.userAgent.toLowerCase();

    if (/mobile|android|iphone|ipod|blackberry|iemobile|kindle|silk-accelerated/.test(ua)) {
        return "Mobile";
    } else if (/ipad|tablet|playbook/.test(ua)) {
        return "Tablet";
    } else {
        return "PC";
    }
}



