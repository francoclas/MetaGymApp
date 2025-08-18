document.addEventListener("DOMContentLoaded", () => {
    const tabla = $("#tablaHistorial").DataTable({
        pageLength: 20,
        lengthMenu: [10, 20, 50, 100],
        order: [[3, "desc"]],
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
        }
    });

    // Conectar el buscador manual al filtro de DataTables
    document.getElementById("buscadorHistorial").addEventListener("keyup", function () {
        tabla.search(this.value).draw();
    });
});
