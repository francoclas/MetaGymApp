$(document).ready(function () {
    const table = $('#tablaClientes').DataTable({
        paging: true,
        pageLength: 20,
        lengthChange: false,
        language: {
            url: '//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json'
        },
        columnDefs: [
            { orderable: false, targets: 4 } // Acciones sin ordenamiento
        ]
    });

    // filtro específico
    $('#buscadorInput').on('keyup', function () {
        const colIndex = $('#filtroCampo').val();
        table.column(colIndex).search(this.value).draw();
    });

    // limpiar búsqueda cuando cambia el campo
    $('#filtroCampo').on('change', function () {
        $('#buscadorInput').val('');
        table.columns().search(''); // limpiar todas
        table.draw();
    });
});
