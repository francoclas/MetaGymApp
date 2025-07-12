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

    // Like (simulado)
    document.querySelectorAll(".pub-btn-like").forEach(btn => {
        btn.addEventListener("click", () => {
            const span = btn.querySelector("span");
            span.innerText = parseInt(span.innerText) + 1;
        });
    });
});
