// ==============================
// INICIALIZACIÓN DE DATATABLES
// ==============================
$(document).ready(function () {
    $("#tablaMisEjercicios, #tablaEjerciciosSistema, #tablaClientes").DataTable({
        language: { url: "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json" },
        info: false
    });

    $("#tablaEjerciciosSeleccionados, #tablaClientesSeleccionados").DataTable({
        paging: false,
        searching: false,
        info: false,
        language: { url: "https://cdn.datatables.net/plug-ins/1.10.25/i18n/Spanish.json" }
    });
});

// ==============================
// FUNCIONES EJERCICIOS
// ==============================
function agregarEjercicio(id, nombre, img) {
    // Evito duplicados
    if ($(`#tablaEjerciciosSeleccionados tr[data-id='${id}']`).length > 0) {
        alert("⚠️ El ejercicio ya está en la rutina.");
        return;
    }

    let fila = `
        <tr data-id="${id}">
            <td><img src="${img}" alt="img" class="rounded" style="width:60px;height:60px;object-fit:cover;" /></td>
            <td>${nombre}</td>
            <td>
                <button type="button" class="btn btn-sm btn-danger" onclick="quitarEjercicio(${id}, this)">❌ Quitar</button>
                <input type="hidden" name="IdsEjerciciosSeleccionados" value="${id}" />
            </td>
        </tr>`;

    $("#tablaEjerciciosSeleccionados tbody").append(fila);
}

function quitarEjercicio(id, btn) {
    $(btn).closest("tr").remove();
}

// ==============================
// FUNCIONES CLIENTES
// ==============================
function agregarCliente(id, nombre) {
    // Evito duplicados
    if ($(`#tablaClientesSeleccionados tr[data-id='${id}']`).length > 0) {
        alert("⚠️ El cliente ya está asignado.");
        return;
    }

    let fila = `
        <tr data-id="${id}">
            <td>${nombre}</td>
            <td>
                <button type="button" class="btn btn-sm btn-danger" onclick="quitarCliente(${id}, this)">❌ Quitar</button>
                <input type="hidden" name="IdsClientesAsignados" value="${id}" />
            </td>
        </tr>`;

    $("#tablaClientesSeleccionados tbody").append(fila);
}

function quitarCliente(id, btn) {
    $(btn).closest("tr").remove();
}
