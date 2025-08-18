document.addEventListener("DOMContentLoaded", () => {
    const tabla = $("#tablaHistorialEntrenamiento").DataTable({
        pageLength: 20,
        lengthMenu: [10, 20, 50, 100],
        order: [[2, "desc"]], // ordena por fecha descendente
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
        }
    });

    // Conectar el buscador manual con DataTables
    document.getElementById("buscadorHistorialEntrenamiento")
        .addEventListener("keyup", function () {
            tabla.search(this.value).draw();
        });
});
