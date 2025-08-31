class Block {
    constructor(x, y, width, height, color, label) {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.color = color;
        this.label = label;
        this.selected = false;

    }

    draw(ctx) {
        ctx.fillStyle = this.selected ? "#f09205ff" : this.color;
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

class Board {
    constructor(board) {
        this.board = board;
        this.map = [
            "00000000000000000",
            "00000000000000000",
            "000000x0000000000",
            "11111111111111111",
            "11111111111111111",
            "  111111111111111",
            "  111111111111111",
            "  111111111111111",
            "  111111111111111",
            "  2 2 2 2 2 2 2  "
        ];
        this.columnCount = 17;
        this.rowCount = 10;
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
    }

    

    configBoard() {
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

    init() {
        this.configBoard();
        this.load_seat();
        this.draw();
        this.load_pan();
    }

    load_seat() {
        this.blocks = [];
        this.map.forEach((row, rowIndex) => {
            row.split("").forEach((seat, colIndex) => {
                if (seat === " ") return;
                let color = "#000";
                const x = colIndex * (this.blockWidth + this.spacing) + 16;
                const y = rowIndex * (this.blockHeight + this.spacing) + 16;
                const label = indexToLetters(rowIndex) + (colIndex + 1);

                if (seat === "0") {
                    color = "#4C13A8";
                } else if (seat === "1") {
                    color = "#DC030E";
                } else if (seat === "2") {
                    color = "#F005A2";
                    const block = new Block(x, y, this.blockWidth * 2 + this.spacing, this.blockHeight, color, label);
                    this.blocks.push(block);
                    return;
                } else if (seat === "x") {
                    color = "#555555ff";
                }
                const block = new Block(x, y, this.blockWidth, this.blockHeight, color, label);
                this.blocks.push(block);
                //console.log("Đã thêm block " + `${block.x}`);
            })
        });
        //console.log("Load seat");
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
            const block = new Block(baseX, baseY, blockSize, blockSize, color, "");
            block.draw(this.ctx);
            //this.blocks.push(block);

            this.ctx.fillStyle = "white";
            this.ctx.font = "14px Arial";
            this.ctx.textAlign = "left";
            this.ctx.textBaseline = "top";
            this.ctx.fillText(type, baseX + 30, baseY + 4);
        });

        //console.log("Load type seat");
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
                // limitPan();
                //console.log(`${offsetX} - ${offsetY}`);
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

        this.board.addEventListener("click", e => {
            const rect = this.board.getBoundingClientRect();
            const mx = e.clientX - rect.left;
            const my = e.clientY - rect.top;

            // 🔑 quy đổi về tọa độ logic
            const lx = (mx - this.offsetX) / this.scale;
            const ly = (my - this.offsetY) / this.scale;

            this.blocks.forEach(block => {
                if (block.contains(lx, ly)) {
                    block.selected = !block.selected;
                    console.log(`${block.label} - ${block.selected}`);
                }
            });

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
}

function indexToLetters(idx) {
    let s = "";
    idx = idx + 1;
    while (idx > 0) {
        const rem = (idx - 1) % 26;
        s = String.fromCharCode(65 + rem) + s;
        idx = Math.floor((idx - 1) / 26);
    }
    return s;
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


// 0: ghế thường, 1: ghế VIP, 2: ghế đôi 
const map = [
    "00000000000000000",
    "00000000000000000",
    "000000x0000000000",
    "11111111111111111",
    "11111111111111111",
    "  111111111111111",
    "  111111111111111",
    "  111111111111111",
    "  111111111111111",
    "  2 2 2 2 2 2 2  "
]


let scale = 1;
const minScale = 0.5;   // nhỏ nhất
const maxScale = 3;     // lớn nhất

function clamp(value, min, max) {
    return Math.min(Math.max(value, min), max);
}

function limitPan() {
    const scaledWidth = boardWidth * scale;
    const scaledHeight = boardHeight * scale;
    console.log(`${offsetX} - ${offsetY}`);
    console.log(`${scaledWidth} - ${scaledHeight}`);

    const limitX = ((Math.abs(offsetX) - scaledWidth) / scaledWidth) * 100 > -30;
    const limitY = ((Math.abs(offsetY) - scaledHeight) / scaledHeight) * 100 > -90;
    if (limitX) {
        offsetX = 0;
        console.log("Đạt giới hạn X")
    }
    // if (limitY) {
    //     offsetY = 0;
    //     console.log("Đạt giới hạn Y")
    // }

    // const minX = boardWidth - scaledWidth;
    // const minY = boardHeight - scaledHeight;

    // offsetX = clamp(offsetX, minX, 0);
    // offsetY = clamp(offsetY, minY, 0);
}