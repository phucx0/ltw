// trending movies 
// carousel
const movies = [
    {
        rank: 1,
        name: "Thanh gươm diệt quỷ",
        author: "Koyoharu Gotouge",
        rating: 4.6,
        poster_url: "https://iguov8nhvyobj.vcdn.cloud/media/catalog/product/cache/1/thumbnail/240x388/c88460ec71d04fa96e628a21494d2fd3/p/o/poster_dm.jpg"
    },
    {
        rank: 2,
        name: "One Piece Film: Red",
        author: "Eiichiro Oda",
        rating: 4.8,
        poster_url: "https://static.nutscdn.com/vimg/300-0/3032221de3bc86bcfbc979635b53d047.jpg"
    },
    {
        rank: 3,
        name: "Jujutsu Kaisen 0",
        author: "Gege Akutami",
        rating: 4.7,
        poster_url: "https://static.nutscdn.com/vimg/300-0/09e4c57b152db1ecdb639eadef6b9356.jpg"
    },
    {
        rank: 4,
        name: "Your Name",
        author: "Makoto Shinkai",
        rating: 4.9,
        poster_url: "https://static.nutscdn.com/vimg/300-0/1cb31180dc6e0d527a7faef6918b6bb3.jpg"
    },
    {
        rank: 5,
        name: "Spirited Away",
        author: "Hayao Miyazaki",
        rating: 4.9,
        poster_url: "https://static.nutscdn.com/vimg/300-0/f416e981c5594516dcdedede5c359895.jpg"
    },
    {
        rank: 6,
        name: "Attack on Titan: The Final Season",
        author: "Hajime Isayama",
        rating: 4.8,
        poster_url: "https://static.nutscdn.com/vimg/300-0/08ac7a1026ddff07926d62d622236c68.jpg"
    },
    {
        rank: 7,
        name: "Naruto Shippuden",
        author: "Masashi Kishimoto",
        rating: 4.7,
        poster_url: "https://static.nutscdn.com/vimg/300-0/1fc5564f23fac6f4fb7de60eb7caaeb4.jpg"
    },
    {
        rank: 8,
        name: "My Hero Academia",
        author: "Kohei Horikoshi",
        rating: 4.5,
        poster_url: "https://static.nutscdn.com/vimg/300-0/d43c068be78c939ccbe08a680bb23964.jpg"
    },
    {
        rank: 9,
        name: "Fullmetal Alchemist: Brotherhood",
        author: "Hiromu Arakawa",
        rating: 4.9,
        poster_url: "https://static.nutscdn.com/vimg/300-0/940af01859762d8ddee47074306244ed.jpg"
    },
    {
        rank: 10,
        name: "Dragon Ball Super: Broly",
        author: "Akira Toriyama",
        rating: 4.6,
        poster_url: "https://static.nutscdn.com/vimg/300-0/f5ac7685073ea99b83397437bc5371f4.webp"
    }
];

let height = 0;
let width = 0;
const carousel = document.querySelector(".carousel");

function updateSize() {
    let col = 4;
    if (carousel.clientWidth < 768) {
        col = 2;
    } else if (carousel.clientWidth < 1000) {
        col = 3;
    }
    width = ((carousel.clientWidth - ((col - 1) * 24)) / col);
    height = width * (4 / 3);

    const wrappers = document.querySelectorAll(".trending-movie-wrapper");
    wrappers.forEach(w => w.style.width = width + "px");
    const images = document.querySelectorAll(".movie-thumbnail-content img");
    images.forEach(img => {
        img.style.width = width + "px";
        img.style.height = height + "px";
    });

    console.log("width:", width, "height:", height);
}

window.addEventListener("resize", updateSize);

function load_trending_movies_list() {
    // const width = ((carousel.clientWidth - (3 * 24)) / 4);
    // const height = width * (4/3);
    console.log(width)
    movies.forEach(movie => {
        const item = document.createElement("div");
        item.className = "trending-movie-wrapper";
        item.style.width = width + "px";
        item.innerHTML = `
        <a href="Details">
            <div class="trending-movie-thumbnail">
                <div class="movie-thumbnail-content">
                    <img width="${width}" height="${height}" src="${movie.poster_url}" alt="">
                    <div class="movie-rating">${movie.rating}</div>
                </div>
            </div>
            <div class="trending-movie-info">
                <p class="rank">${movie.rank}</p>
                <div>
                    <p class="trending-movie-name">${movie.name}</p>
                    <p class="trending-movie-author">${movie.author}</p>
                </div>
            </div>
        </a>
        `
        carousel.appendChild(item);
    })
}

document.addEventListener("DOMContentLoaded", () => {
    updateSize();
    //load_trending_movies_list();
})

document.querySelector('.prev-btn').addEventListener('click', () => {
    if (movies.length === 0) return

    // const width = ((carousel.clientWidth - (3 * 24)) / 4);
    carousel.scrollBy({
        left: -(width + 24),
        behavior: "smooth"
    });
})

document.querySelector('.next-btn').addEventListener('click', () => {
    if (movies.length === 0) return

    // const width = ((carousel.clientWidth - (3 * 24)) / 4);
    carousel.scrollBy({
        left: (width + 24),
        behavior: "smooth"
    });
})


