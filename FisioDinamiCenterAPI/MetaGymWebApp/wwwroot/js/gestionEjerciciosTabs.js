document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll('[data-tab]');
    const contents = document.querySelectorAll('.tab-content');
    const buscador = document.getElementById("buscadorEjercicios");

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            contents.forEach(content => content.classList.add("d-none"));
            document.getElementById(tab.dataset.tab).classList.remove("d-none");
        });
    });

    buscador.addEventListener("input", () => {
        const termino = buscador.value.toLowerCase();
        const tarjetas = document.querySelectorAll(".ejercicio-card");

        tarjetas.forEach(card => {
            const nombre = card.dataset.nombre.toLowerCase();
            const tipo = card.dataset.tipo.toLowerCase();
            const grupo = card.dataset.grupo.toLowerCase();

            const coincide = nombre.includes(termino) || tipo.includes(termino) || grupo.includes(termino);
            card.style.display = coincide ? "block" : "none";
        });
    });
});