$(document).ready(function () {
    $('.datatable').DataTable({
        language: {
            search: "Buscar:",
            lengthMenu: "Mostrar _MENU_ registros por página",
            info: "Mostrando _START_ a _END_ de _TOTAL_ registros",
            paginate: {
                previous: "Anterior",
                next: "Siguiente"
            },
            zeroRecords: "No se encontraron resultados"
        }
    });
});
function normalizarTexto(data) {
    return !data
        ? ''
        : data
            .toString()
            .normalize("NFD")                 
            .replace(/[\u0300-\u036f]/g, "")   
            .toLowerCase();
}

if ($.fn && $.fn.dataTable) {
    $.fn.dataTable.ext.type.search.string = function (data) {
        return normalizarTexto(data);
    };
}