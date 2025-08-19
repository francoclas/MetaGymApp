let ejerciciosSeleccionados = [];
let clientesSeleccionados = [];

function agregarEjercicio(id, nombre) {
    if (ejerciciosSeleccionados.some(e => e.id === id)) {
        alert("Ese ejercicio ya fue agregado");
        return;
    }
    ejerciciosSeleccionados.push({ id, nombre });

    const fila = `
        <tr data-id="${id}">
            <td><img src="/img/default.png" class="img-thumbnail" style="max-width:80px" /></td>
            <td>${nombre}</td>
            <td><button type="button" class="btn btn-sm btn-danger" onclick="quitarEjercicio(${id})">Quitar</button></td>
        </tr>`;
    $("#tablaEjerciciosSeleccionados tbody").append(fila);
    $("#contenedorEjerciciosInputs").append(`<input type="hidden" name="IdsEjerciciosSeleccionados" value="${id}" />`);
}

function quitarEjercicio(id) {
    ejerciciosSeleccionados = ejerciciosSeleccionados.filter(e => e.id !== id);
    $(`#tablaEjerciciosSeleccionados tr[data-id="${id}"]`).remove();
    $(`#contenedorEjerciciosInputs input[value="${id}"]`).remove();
}

function agregarCliente(id, nombre) {
    if (clientesSeleccionados.some(c => c.id === id)) {
        alert("Ese cliente ya fue agregado");
        return;
    }
    clientesSeleccionados.push({ id, nombre });

    const fila = `
        <tr data-id="${id}">
            <td>${nombre}</td>
            <td><button type="button" class="btn btn-sm btn-danger" onclick="quitarCliente(${id})">Quitar</button></td>
        </tr>`;
    $("#tablaClientesSeleccionados tbody").append(fila);
    $("#contenedorClientesInputs").append(`<input type="hidden" name="IdsClientesAsignados" value="${id}" />`);
}

function quitarCliente(id) {
    clientesSeleccionados = clientesSeleccionados.filter(c => c.id !== id);
    $(`#tablaClientesSeleccionados tr[data-id="${id}"]`).remove();
    $(`#contenedorClientesInputs input[value="${id}"]`).remove();
}

// Inicializar datatables
$(document).ready(function () {
    $("#tablaMisEjercicios, #tablaEjerciciosSistema, #tablaClientes").DataTable({
        language: { url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json" },
        info: false
    });
    $("#tablaEjerciciosSeleccionados, #tablaClientesSeleccionados").DataTable({
        language: { url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json" },
        paging: false,
        searching: false,
        info: false
    });
});
