document.querySelectorAll(".faq-item").forEach(q => {
    q.addEventListener("click", () => {
        if (q.classList.contains("active")) {
            q.classList.remove("active");
            return;
        }
        q.classList.add("active");
    })
})