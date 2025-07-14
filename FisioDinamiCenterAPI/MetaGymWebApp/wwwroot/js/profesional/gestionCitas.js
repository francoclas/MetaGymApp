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