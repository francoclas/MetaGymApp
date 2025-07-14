document.addEventListener("DOMContentLoaded", function () {
    const texto = document.getElementById("buscadorTexto");
    const desde = document.getElementById("fechaDesde");
    const hasta = document.getElementById("fechaHasta");
    const tarjetas = document.querySelectorAll(".tarjeta-publicacion");

    function filtrar() {
        const valTexto = texto.value.toLowerCase();
        const valDesde = desde.value;
        const valHasta = hasta.value;

        tarjetas.forEach(card => {
            const solicitante = card.dataset.solicitante;
            const titulo = card.dataset.titulo;
            const fecha = card.dataset.fecha;

            const coincideTexto = solicitante.includes(valTexto) || titulo.includes(valTexto);
            const enRango =
                (!valDesde || fecha >= valDesde) &&
                (!valHasta || fecha <= valHasta);

            card.style.display = (coincideTexto && enRango) ? "block" : "none";
        });
    }

    texto.addEventListener("input", filtrar);
    desde.addEventListener("change", filtrar);
    hasta.addEventListener("change", filtrar);
});

function ordenarPorFecha(direccion) {
    const contenedor = document.getElementById("contenedorPublicaciones");
    const tarjetas = Array.from(contenedor.querySelectorAll(".tarjeta-publicacion"));

    tarjetas.sort((a, b) => {
        const fechaA = a.dataset.fecha;
        const fechaB = b.dataset.fecha;
        return direccion === "asc"
            ? fechaA.localeCompare(fechaB)
            : fechaB.localeCompare(fechaA);
    });

    tarjetas.forEach(t => contenedor.appendChild(t));
}
