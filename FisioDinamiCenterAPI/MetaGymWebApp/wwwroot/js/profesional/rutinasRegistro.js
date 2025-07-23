let ejerciciosSeleccionados = [];
let clientesSeleccionados = [];
const listaEjercicios = new Set();
function agregarEjercicio(id, nombre) {
    if (listaEjercicios.has(id)) return;

    listaEjercicios.add(id);

    const ul = document.getElementById("listaEjerciciosSeleccionados");
    const li = document.createElement("li");
    li.innerText = nombre;
    li.dataset.id = id;

    const btnQuitar = document.createElement("button");
    btnQuitar.innerText = "Quitar";
    btnQuitar.type = "button";
    btnQuitar.className = "btn btn-sm btn-danger ms-2";
    btnQuitar.onclick = function () {
        listaEjercicios.delete(id);
        li.remove();
        document.getElementById(`input-ej-${id}`)?.remove();
    };

    li.appendChild(btnQuitar);
    ul.appendChild(li);

    // Crear input oculto
    const input = document.createElement("input");
    input.type = "hidden";
    input.name = "IdsEjerciciosSeleccionados";
    input.value = id;
    input.id = `input-ej-${id}`;
    document.getElementById("contenedorEjerciciosInputs").appendChild(input);
}
function quitarEjercicio(id) {
    ejerciciosSeleccionados = ejerciciosSeleccionados.filter(e => e !== id);
    actualizarListaEjercicios();
    document.querySelector(`.card-ejercicio[data-id="${id}"]`).style.display = "block";
}
function actualizarListaEjercicios() {
    const lista = document.getElementById("listaEjerciciosSeleccionados");
    lista.innerHTML = "";

    const contenedorInputs = document.getElementById("contenedorEjerciciosInputs");
    contenedorInputs.innerHTML = "";

    ejerciciosSeleccionados.forEach(id => {
        const nombre = document.querySelector(`.card-ejercicio[data-id="${id}"] strong`).textContent;

        // Mostrar en lista
        lista.innerHTML += `
            <li data-id="${id}">
                ${nombre} 
                <button type="button" onclick="quitarEjercicio(${id})">X</button>
            </li>`;

        // Agregar input oculto
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "IdsEjerciciosSeleccionados";
        input.value = id;
        contenedorInputs.appendChild(input);
    });
}

function agregarCliente(id, nombre) {
    if (!clientesSeleccionados.includes(id)) {
        clientesSeleccionados.push(id);
        actualizarListaClientes();
        document.querySelector(`tr[data-id="${id}"]`).style.display = "none";
    }
}

function quitarCliente(id) {
    clientesSeleccionados = clientesSeleccionados.filter(c => c !== id);
    actualizarListaClientes();
    document.querySelector(`tr[data-id="${id}"]`).style.display = "";
}

function actualizarListaClientes() {
    const lista = document.getElementById("listaClientesSeleccionados");
    lista.innerHTML = "";

    const contenedorInputs = document.getElementById("contenedorClientesInputs");
    contenedorInputs.innerHTML = "";

    clientesSeleccionados.forEach(id => {
        const nombre = document.querySelector(`tr[data-id="${id}"] td:first-child`).textContent;

        lista.innerHTML += `
            <li data-id="${id}">
                ${nombre}
                <button type="button" onclick="quitarCliente(${id})">X</button>
            </li>`;

        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "IdsClientesAsignados";
        input.value = id;
        contenedorInputs.appendChild(input);
    });
}

// Buscador de ejercicios
document.getElementById("buscadorEjercicios").addEventListener("input", function () {
    const valor = this.value.toLowerCase();
    document.querySelectorAll("#ejerciciosDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre.toLowerCase();
        const tipo = card.dataset.tipo.toLowerCase();
        card.style.display = (nombre.includes(valor) || tipo.includes(valor)) ? "block" : "none";
    });
});

// Buscador de clientes
document.getElementById("buscadorClientes").addEventListener("input", function () {
    const valor = this.value.toLowerCase();
    document.querySelectorAll(".tabla-clientes tbody tr").forEach(row => {
        const nombre = row.dataset.nombre.toLowerCase();
        const usuario = row.dataset.usuario.toLowerCase();
        const ci = row.dataset.ci.toLowerCase();
        row.style.display = (nombre.includes(valor) || usuario.includes(valor) || ci.includes(valor)) ? "" : "none";
    });
});
function aplicarFiltro(inputId, contenedorId) {
    const input = document.getElementById(inputId);
    const contenedor = document.getElementById(contenedorId);
    const cards = contenedor.querySelectorAll(".card-ejercicio");

    input.addEventListener("input", () => {
        const texto = input.value.toLowerCase();
        cards.forEach(card => {
            const nombre = card.dataset.nombre;
            const tipo = card.dataset.tipo;
            const visible = nombre.includes(texto) || tipo.includes(texto);
            card.style.display = visible ? "block" : "none";
        });
    });
}

document.addEventListener("DOMContentLoaded", () => {
    aplicarFiltro("buscadorMisEjercicios", "misEjerciciosDisponibles");
    aplicarFiltro("buscadorEjerciciosSistema", "ejerciciosSistemaDisponibles");
});