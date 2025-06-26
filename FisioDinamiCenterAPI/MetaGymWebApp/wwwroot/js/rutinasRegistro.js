let ejerciciosSeleccionados = [];
let clientesSeleccionados = [];

function agregarEjercicio(id, nombre) {
    if (!ejerciciosSeleccionados.includes(id)) {
        ejerciciosSeleccionados.push(id);
        document.getElementById("listaEjerciciosSeleccionados").innerHTML += `<li data-id="${id}">${nombre}</li>`;
        document.getElementById("IdsEjerciciosSeleccionados").value = ejerciciosSeleccionados.join(",");
    }
}

function agregarCliente(id, nombre) {
    if (!clientesSeleccionados.includes(id)) {
        clientesSeleccionados.push(id);
        document.getElementById("listaClientesSeleccionados").innerHTML += `<li data-id="${id}">${nombre}</li>`;
        document.getElementById("IdsClientesAsignados").value = clientesSeleccionados.join(",");
    }
}

// Buscador en ejercicios
document.getElementById("buscadorEjercicios").addEventListener("input", function () {
    const valor = this.value.toLowerCase();
    document.querySelectorAll("#ejerciciosDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre.toLowerCase();
        const tipo = card.dataset.tipo.toLowerCase();
        card.style.display = (nombre.includes(valor) || tipo.includes(valor)) ? "block" : "none";
    });
});