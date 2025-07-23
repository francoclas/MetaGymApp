document.addEventListener("DOMContentLoaded", function () {
    // Ver más
    document.querySelectorAll(".pub-descripcion").forEach(desc => {
        const span = desc.querySelector(".pub-texto-resumen");
        const btn = desc.querySelector(".pub-btn-vermas");
        const originalText = span.innerText;

        if (originalText.length <= 130) {
            btn.style.display = "none";
            return;
        }

        span.innerText = originalText.slice(0, 130) + "...";

        btn.addEventListener("click", () => {
            if (btn.innerText === "Ver más") {
                span.innerText = originalText;
                btn.innerText = "Ver menos";
            } else {
                span.innerText = originalText.slice(0, 130) + "...";
                btn.innerText = "Ver más";
            }
        });
    });

    // Comentarios toggle
    document.querySelectorAll(".pub-btn-comentarios").forEach(btn => {
        btn.addEventListener("click", () => {
            const id = btn.dataset.id;
            const comentarios = document.getElementById("comentarios-" + id);
            if (comentarios.classList.contains("d-none")) {
                comentarios.classList.remove("d-none");
            } else {
                comentarios.classList.add("d-none");
            }
        });
    });
    //Slide imagenes
    
    // Like (simulado)
    document.querySelectorAll(".pub-btn-like").forEach(btn => {
        btn.addEventListener("click", () => {
            const span = btn.querySelector("span");
            span.innerText = parseInt(span.innerText) + 1;
            span.innerText = parseInt(span.innerText) + 1;
        });
    });
   
});
const slideIndices = {};

function cambiarSlide(pubId, cambio) {
    const container = document.querySelector(`#slideshow-${pubId} .pub-slide-container`);
    const dots = document.querySelectorAll(`#slideshow-${pubId} .pub-slide-dot`);
    const total = dots.length;

    if (!slideIndices[pubId]) slideIndices[pubId] = 0;
    slideIndices[pubId] = (slideIndices[pubId] + cambio + total) % total;

    container.style.transform = `translateX(-${slideIndices[pubId] * 100}%)`;
    dots.forEach(dot => dot.classList.remove('active'));
    dots[slideIndices[pubId]].classList.add('active');
}
function habilitarSwipe(pubId) {
    const container = document.querySelector(`#slideshow-${pubId} .pub-slide-container`);
    let startX = 0;
    let isSwiping = false;

    if (!container) return;

    container.addEventListener("touchstart", e => {
        startX = e.touches[0].clientX;
        isSwiping = true;
    });

    container.addEventListener("touchmove", e => {
        if (!isSwiping) return;
        const diff = e.touches[0].clientX - startX;
        if (Math.abs(diff) > 50) {
            cambiarSlide(pubId, diff < 0 ? 1 : -1);
            isSwiping = false;
        }
    });

    container.addEventListener("touchend", () => {
        isSwiping = false;
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const publicaciones = document.querySelectorAll(".pub-slideshow");
    publicaciones.forEach(slide => {
        const pubId = slide.id.split("-")[1];
        habilitarSwipe(pubId);
    });
});
