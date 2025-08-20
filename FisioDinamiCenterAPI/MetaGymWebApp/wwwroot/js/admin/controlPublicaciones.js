let minDate, maxDate;

document.addEventListener("DOMContentLoaded", () => {
    // Definir los inputs de fecha usando el plugin DateTime
    minDate = new DateTime('#fechaDesde', {
        format: 'YYYY-MM-DD'
    });
    maxDate = new DateTime('#fechaHasta', {
        format: 'YYYY-MM-DD'
    });

    // Filtro personalizado de DataTables para fechas
    $.fn.dataTable.ext.search.push(function (settings, data) {
        const min = minDate.val();
        const max = maxDate.val();
        const fechaStr = data[3]; // Columna de la fecha (índice 3 en tu tabla)
        const fecha = fechaStr ? new Date(fechaStr) : null;

        if (!fecha) return true;

        if (
            (min === null && max === null) ||
            (min === null && fecha <= max) ||
            (min <= fecha && max === null) ||
            (min <= fecha && fecha <= max)
        ) {
            return true;
        }
        return false;
    });

    // Inicializar la tabla (evita warning si ya estaba)
    if ($.fn.DataTable.isDataTable("#tablaPublicaciones")) {
        $("#tablaPublicaciones").DataTable().destroy();
    }

    const tabla = $("#tablaPublicaciones").DataTable({
        pageLength: 10,
        lengthMenu: [10, 20, 50, 100],
        order: [[3, "desc"]],
        language: {
            url: "/js/datatables/es-ES.json" // archivo local de idioma
        },
        info: false
    });

    // Redibujar cuando cambien los filtros
    document.querySelectorAll("#fechaDesde, #fechaHasta").forEach(el => {
        el.addEventListener("change", () => tabla.draw());
    });
});
