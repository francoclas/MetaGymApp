const filtro = document.getElementById("filtroCitas");
const citas = document.querySelectorAll(".cita-card");

filtro.addEventListener("input", function () {
    const texto = this.value.toLowerCase();
    citas.forEach(cita => {
        const contenido = cita.textContent.toLowerCase();
        cita.style.display = contenido.includes(texto) ? "block" : "none";
    });
});

function ordenarCitas(direccion) {
    const contenedor = citas[0].parentElement;
    const citasArray = Array.from(citas);

    citasArray.sort((a, b) => {
        const fechaA = new Date(a.dataset.fecha);
        const fechaB = new Date(b.dataset.fecha);
        return direccion === "asc" ? fechaA - fechaB : fechaB - fechaA;
    });

    citasArray.forEach(c => contenedor.appendChild(c));
}
document.addEventListener("DOMContentLoaded", function () {
    const tabs = document.querySelectorAll(".nav-link[data-tab]");
    const contents = document.querySelectorAll(".tab-content");
    const filtro = document.getElementById("filtroCitas");

    tabs.forEach(tab => {
        tab.addEventListener("click", function () {
            tabs.forEach(t => t.classList.remove("active"));
            contents.forEach(c => c.classList.add("d-none"));

            const target = document.getElementById("tab-" + this.dataset.tab);
            this.classList.add("active");
            target.classList.remove("d-none");
        });
    });

    filtro.addEventListener("input", function () {
        const texto = this.value.toLowerCase();
        document.querySelectorAll(".tab-content:not(.d-none) .cita-card").forEach(card => {
            const contenido = card.innerText.toLowerCase();
            card.style.display = contenido.includes(texto) ? "" : "none";
        });
    });
});

function ordenarCitas(direccion) {
    const activa = document.querySelector(".tab-content:not(.d-none)");
    const cards = Array.from(activa.querySelectorAll(".cita-card"));

    cards.sort((a, b) => {
        const fa = new Date(a.dataset.fecha);
        const fb = new Date(b.dataset.fecha);
        return direccion === "asc" ? fa - fb : fb - fa;
    });

    cards.forEach(card => activa.appendChild(card));
}
