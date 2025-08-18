window.ejerciciosSeleccionados = [];
window.clientesSeleccionados = [];

window.initDatosIniciales = function () {
    document.querySelectorAll("#listaEjerciciosSeleccionados li").forEach(li => {
        window.ejerciciosSeleccionados.push({
            id: parseInt(li.dataset.id),
            nombre: li.dataset.nombre
        });
    });

    document.querySelectorAll("#listaClientesSeleccionados li").forEach(li => {
        window.clientesSeleccionados.push({
            id: parseInt(li.dataset.id),
            nombre: li.dataset.nombre
        });
    });
};

window.agregarEjercicio = function (id, nombre) {
    if (window.ejerciciosSeleccionados.some(e => e.id === id)) return;
    window.ejerciciosSeleccionados.push({ id, nombre });
    actualizarListaEjercicios();
    document.querySelectorAll(`.card-ejercicio[data-id="${id}"]`).forEach(c => c.style.display = "none");
};

window.quitarEjercicio = function (id) {
    window.ejerciciosSeleccionados = window.ejerciciosSeleccionados.filter(e => e.id !== id);
    actualizarListaEjercicios();
    document.querySelectorAll(`.card-ejercicio[data-id="${id}"]`).forEach(c => c.style.display = "block");
};

function actualizarListaEjercicios() {
    const lista = document.getElementById("listaEjerciciosSeleccionados");
    const contenedorInputs = document.getElementById("contenedorEjerciciosInputs");

    lista.innerHTML = "";
    contenedorInputs.innerHTML = "";

    window.ejerciciosSeleccionados.forEach(e => {
        const li = document.createElement("li");
        li.dataset.id = e.id;
        li.dataset.nombre = e.nombre;
        li.className = "list-group-item bg-dark text-light";
        li.textContent = e.nombre + " ";

        const btnQuitar = document.createElement("button");
        btnQuitar.type = "button";
        btnQuitar.className = "btn btn-sm btn-danger ms-2";
        btnQuitar.textContent = "X";
        btnQuitar.onclick = () => window.quitarEjercicio(e.id);

        li.appendChild(btnQuitar);
        lista.appendChild(li);

        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "IdsEjerciciosSeleccionados";
        input.value = e.id;
        contenedorInputs.appendChild(input);
    });
}

window.agregarCliente = function (id, nombre) {
    if (window.clientesSeleccionados.some(c => c.id === id)) return;
    window.clientesSeleccionados.push({ id, nombre });
    actualizarListaClientes();
    document.querySelector(`tr[data-id="${id}"]`).style.display = "none";
};

window.quitarCliente = function (id) {
    window.clientesSeleccionados = window.clientesSeleccionados.filter(c => c.id !== id);
    actualizarListaClientes();
    document.querySelector(`tr[data-id="${id}"]`).style.display = "";
};

function actualizarListaClientes() {
    const lista = document.getElementById("listaClientesSeleccionados");
    const contenedorInputs = document.getElementById("contenedorClientesInputs");

    lista.innerHTML = "";
    contenedorInputs.innerHTML = "";

    window.clientesSeleccionados.forEach(c => {
        const li = document.createElement("li");
        li.dataset.id = c.id;
        li.dataset.nombre = c.nombre;
        li.className = "list-group-item bg-dark text-light";
        li.textContent = c.nombre + " ";

        const btnQuitar = document.createElement("button");
        btnQuitar.type = "button";
        btnQuitar.className = "btn btn-sm btn-danger ms-2";
        btnQuitar.textContent = "X";
        btnQuitar.onclick = () => window.quitarCliente(c.id);

        li.appendChild(btnQuitar);
        lista.appendChild(li);

        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "IdsClientesAsignados";
        input.value = c.id;
        contenedorInputs.appendChild(input);
    });
}

document.addEventListener("DOMContentLoaded", () => {
    window.initDatosIniciales();
});
// Buscador en "Mis Ejercicios"
document.getElementById("buscadorMisEjercicios").addEventListener("input", function () {
    const filtro = this.value.toLowerCase();
    document.querySelectorAll("#misEjerciciosDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre || "";
        card.style.display = nombre.includes(filtro) ? "block" : "none";
    });
});

// Buscador en "Ejercicios del Sistema"
document.getElementById("buscadorEjerciciosSistema").addEventListener("input", function () {
    const filtro = this.value.toLowerCase();
    document.querySelectorAll("#ejerciciosSistemaDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre || "";
        card.style.display = nombre.includes(filtro) ? "block" : "none";
    });
});