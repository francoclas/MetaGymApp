let currentSlide = 0;

window.addEventListener("DOMContentLoaded", () => {
    const slides = document.querySelectorAll("#slideshow .slide");
    if (slides.length === 0) return;

    slides[0].classList.add("active");

    setInterval(() => {
        slides[currentSlide].classList.remove("active");
        currentSlide = (currentSlide + 1) % slides.length;
        slides[currentSlide].classList.add("active");
    }, 4000);
});