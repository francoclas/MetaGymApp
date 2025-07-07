document.addEventListener("DOMContentLoaded", () => {
    const tabs = document.querySelectorAll('[data-tab]');
    const buscador = document.getElementById("buscadorEjercicios");
    const mensaje = document.getElementById("mensaje-sin-resultados");

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            const destino = tab.dataset.tab;

            document.querySelectorAll(".contenedor-ejercicios").forEach(div => div.classList.add("d-none"));
            document.getElementById(`contenedor-${destino}`).classList.remove("d-none");

            buscar(destino); // Filtra al cambiar de pestaña
        });
    });

    buscador.addEventListener("input", () => {
        const visible = document.querySelector(".contenedor-ejercicios:not(.d-none)");
        if (visible) buscar(visible.id.replace("contenedor-", ""));
    });

    function buscar(seccion) {
        const termino = buscador.value.toLowerCase().trim();
        const cards = document.querySelectorAll(`#contenedor-${seccion} .ejercicio-card`);
        let hayCoincidencia = false;

        cards.forEach(card => {
            const nombre = (card.dataset.nombre || "").toLowerCase();
            const tipo = (card.dataset.tipo || "").toLowerCase();
            const grupo = (card.dataset.grupo || "").toLowerCase();

            const coincide = nombre.includes(termino) || tipo.includes(termino) || grupo.includes(termino);
            card.style.display = coincide ? "flex" : "none";
            if (coincide) hayCoincidencia = true;
        });

        mensaje.classList.toggle("d-none", hayCoincidencia || termino === "");
    }
});
