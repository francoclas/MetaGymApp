document.addEventListener("DOMContentLoaded", () => {
    // Extender DataTables con filtro de fechas
    $.fn.dataTable.ext.search.push(function (settings, data) {
        const desde = document.getElementById("fechaDesde").value;
        const hasta = document.getElementById("fechaHasta").value;
        const fecha = data[3]; // columna "Fecha de solicitud"

        if (!fecha) return true;

        const fechaRow = new Date(fecha);
        const fechaDesde = desde ? new Date(desde) : null;
        const fechaHasta = hasta ? new Date(hasta) : null;

        if (fechaDesde && fechaRow < fechaDesde) return false;
        if (fechaHasta && fechaRow > fechaHasta) return false;

        return true;
    });

    // Inicializar tabla
    const tabla = $("#tablaPublicaciones").DataTable({
        pageLength: 20,
        lengthMenu: [10, 20, 50, 100],
        order: [[3, "desc"]],
        language: {
            url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
        }
    });

    // Redibujar cuando cambien los filtros
    document.getElementById("fechaDesde").addEventListener("change", () => tabla.draw());
    document.getElementById("fechaHasta").addEventListener("change", () => tabla.draw());
});
