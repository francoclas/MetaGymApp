$(document).ready(function () {
    $('#tablaUsuarios').DataTable({
        language: {
            search: "Buscar:",
            lengthMenu: "Mostrar _MENU_ registros",
            info: "Mostrando _START_ a _END_ de _TOTAL_ usuarios",
            paginate: {
                previous: "Anterior",
                next: "Siguiente"
            }
        }
    });
});