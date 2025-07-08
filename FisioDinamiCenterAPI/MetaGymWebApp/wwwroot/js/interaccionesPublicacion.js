
document.addEventListener("DOMContentLoaded", function () {
    // Expansión de descripciones largas
    document.querySelectorAll(".btn-expandir").forEach(btn => {
        btn.addEventListener("click", function () {
            const desc = this.previousElementSibling;
            desc.classList.remove("descripcion-recortada");
            this.remove(); // Eliminar botón
        });
    });

    // Likes (simulado localmente)
    document.querySelectorAll(".btn-like").forEach(btn => {
        btn.addEventListener("click", function () {
            const countSpan = this.querySelector(".like-count");
            let count = parseInt(countSpan.innerText);
            countSpan.innerText = count + 1;
        });
    });

    // Expandir/comprimir sección de comentarios
    document.querySelectorAll(".btn-toggle-comentarios").forEach(btn => {
        btn.addEventListener("click", function () {
            const contenedor = this.closest(".publicacion").querySelector(".comentarios-contenedor");
            contenedor.classList.toggle("d-none");
        });
    });
    document.addEventListener("DOMContentLoaded", () => {
        document.querySelectorAll(".descripcion-text").forEach(p => {
            const textoCompleto = p.dataset.completa || "";
            if (textoCompleto.length > 130) {
                p.innerHTML = textoCompleto.substring(0, 130) + "...";
            } else {
                p.innerHTML = textoCompleto;
                const boton = p.parentElement.querySelector(".btn-expandir");
                if (boton) boton.classList.add("d-none");
            }
        });
    });

    function expandirDescripcion(id) {
        const p = document.getElementById("desc-" + id);
        p.innerHTML = p.dataset.completa;
        const btn = p.parentElement.querySelector(".btn-expandir");
        if (btn) btn.remove();
    }

});
