let board;
let ctx;

const rowCount = 10;
const columnCount = 17;

let boardWidth;
let boardHeight;

let spacing;
let width;
let height;

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

function configBoard(board) {
    boardWidth = board.clientWidth;
    boardHeight = board.clientHeight;

    board.height = boardHeight;
    board.width = boardWidth;

    const deviceType = getDeviceType();
    if (deviceType === "Mobile") {
        spacing = 4;
        width = 24;
        height = width;
    }
    else {
        spacing = 8;
        width = (boardWidth - (2 * 16 + (columnCount - 1) * spacing)) / columnCount;
        height = width;
    }

    return boardWidth, boardHeight;
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

// state zoom & pan
let offsetX = 0;
let offsetY = 0;
let isDragging = false;
let startX, startY;
let blocks = [];

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

function load_seat() {
    blocks = [];
    map.forEach((row, rowIndex) => {
        row.split("").forEach((seat, colIndex) => {
            if (seat === " ") return;
            let color = "#000";
            const x = colIndex * (width + spacing) + 16;
            const y = rowIndex * (height + spacing) + 16;
            const label = indexToLetters(rowIndex) + (colIndex + 1);

            if (seat === "0") {
                color = "#4C13A8"; // màu tô
            } else if (seat === "1") {
                color = "#DC030E"; // màu tô
            } else if (seat === "2") {
                color = "#F005A2"; // màu tô

                const block = new Block(x, y, width * 2 + spacing, height, color, label);
                blocks.push(block);
                return;
            } else if (seat === "x") {
                color = "#555555ff"; // màu tô
            }
            const block = new Block(x, y, width, height, color, label);
            blocks.push(block);
        })
    })
}

function load_seat_type() {
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
        const baseY = 40 + row * 32 + rowCount * (height + spacing);

        const blockSize = 20;
        const block = new Block(baseX, baseY, blockSize, blockSize, color, "");
        block.draw(ctx);

        ctx.fillStyle = "white";
        ctx.font = "16px Arial";
        ctx.textAlign = "left";
        ctx.textBaseline = "top";
        ctx.fillText(type, baseX + 30, baseY + 4);
    });
}

// Hàm vẽ tổng
function draw() {
    ctx.clearRect(0, 0, boardWidth, boardHeight);
    ctx.save();
    ctx.translate(offsetX, offsetY);
    ctx.scale(scale, scale);

    blocks.forEach(block => block.draw(ctx));
    load_seat_type();

    ctx.restore();
}


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