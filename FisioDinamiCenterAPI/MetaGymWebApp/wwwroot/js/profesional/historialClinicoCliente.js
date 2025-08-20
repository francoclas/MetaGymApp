document.addEventListener("DOMContentLoaded", () => {
    if ($("#tablaHistorial").length) {
        const tabla = $("#tablaHistorial").DataTable({
            pageLength: 20,
            lengthMenu: [10, 20, 50, 100],
            order: [[3, "desc"]],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
            }
        });

        const buscador = document.getElementById("buscadorHistorial");
        if (buscador) {
            buscador.addEventListener("keyup", function () {
                tabla.search(this.value).draw();
            });
        }
    }
});
