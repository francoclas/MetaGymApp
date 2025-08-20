let ejerciciosSeleccionados = [];
let clientesSeleccionados = [];


function agregarEjercicio(id, nombre, img) {
    if (ejerciciosSeleccionados.some(e => e.id === id)) {
        alert("⚠️ Ese ejercicio ya fue agregado");
        return;
    }
    ejerciciosSeleccionados.push({ id, nombre });

    const fila = `
        <tr data-id="${id}">
            <td><img src="${img}" class="img-thumbnail" style="max-width:80px" /></td>
            <td>${nombre}</td>
            <td><button type="button" class="btn btn-sm btn-danger" onclick="quitarEjercicio(${id})">❌ Quitar</button></td>
            <input type="hidden" name="IdsEjerciciosSeleccionados" value="${id}" />
        </tr>`;
    $("#tablaEjerciciosSeleccionados tbody").append(fila);
}

function quitarEjercicio(id) {
    ejerciciosSeleccionados = ejerciciosSeleccionados.filter(e => e.id !== id);
    $(`#tablaEjerciciosSeleccionados tr[data-id="${id}"]`).remove();
    $(`#tablaEjerciciosSeleccionados input[value="${id}"]`).remove();
}

function agregarCliente(id, nombre) {
    if (clientesSeleccionados.some(c => c.id === id)) {
        alert("⚠️ Ese cliente ya fue agregado");
        return;
    }
    clientesSeleccionados.push({ id, nombre });

    const fila = `
        <tr data-id="${id}">
            <td>${nombre}</td>
            <td><button type="button" class="btn btn-sm btn-danger" onclick="quitarCliente(${id})">❌ Quitar</button></td>
            <input type="hidden" name="IdsClientesAsignados" value="${id}" />
        </tr>`;
    $("#tablaClientesSeleccionados tbody").append(fila);
}

function quitarCliente(id) {
    clientesSeleccionados = clientesSeleccionados.filter(c => c.id !== id);
    $(`#tablaClientesSeleccionados tr[data-id="${id}"]`).remove();
    $(`#tablaClientesSeleccionados input[value="${id}"]`).remove();
}

$(document).ready(function () {
    $("#tablaMisEjercicios, #tablaEjerciciosSistema, #tablaClientes").DataTable({
        language: { url: "//cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json" },
        info: false
    });
    $("#tablaEjerciciosSeleccionados, #tablaClientesSeleccionados").DataTable({
        paging: false,
        searching: false,
        info: false
    });
});
