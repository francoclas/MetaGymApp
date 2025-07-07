document.addEventListener("DOMContentLoaded", () => {
    const inputTexto = document.getElementById("buscadorRutina");
    const fechaDesde = document.getElementById("fechaDesde");
    const fechaHasta = document.getElementById("fechaHasta");

    const sesiones = document.querySelectorAll(".card-sesion");

    function filtrar() {
        const texto = inputTexto.value.toLowerCase();
        const desde = fechaDesde.value;
        const hasta = fechaHasta.value;

        sesiones.forEach(card => {
            const nombre = (card.dataset.nombre || "").toLowerCase();
            const fecha = card.dataset.fecha;

            const coincideNombre = nombre.includes(texto);
            const coincideFecha = (!desde || fecha >= desde) && (!hasta || fecha <= hasta);

            card.style.display = (coincideNombre && coincideFecha) ? "block" : "none";
        });
    }

    inputTexto.addEventListener("input", filtrar);
    fechaDesde.addEventListener("change", filtrar);
    fechaHasta.addEventListener("change", filtrar);
});
