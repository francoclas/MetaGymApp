function agregar(tipo, id, nombre, btn) {
    const lista = document.getElementById(`lista-${tipo}s`);

    const li = document.createElement("li");
    li.className = "list-group-item bg-dark text-light d-flex justify-content-between align-items-center";
    li.dataset.id = id;

    if (tipo === "ejercicio") {
        const card = btn.closest(".card-ejercicio");
        li.dataset.origen = card.closest("#misEjerciciosDisponibles") ? "mis" : "sistema";
    } else {
        li.dataset.origen = "cliente";
    }

    li.innerHTML = `${nombre} 
        <button type="button" class="btn btn-sm btn-danger" onclick="quitar('${tipo}', ${id}, this)">X</button>
        <input type="hidden" name="Ids${capitalizar(tipo)}sSeleccionados" value="${id}" />`;

    lista.appendChild(li);

    if (tipo === "ejercicio") btn.closest(".card").style.display = "none";
    else btn.closest("tr").style.display = "none";
}

function quitar(tipo, id, btn) {
    const li = btn.closest("li");
    const origen = li.dataset.origen;
    li.remove();

    if (tipo === "ejercicio") {
        if (origen === "mis") {
            const card = document.querySelector(`#misEjerciciosDisponibles .card-ejercicio[data-id="${id}"]`);
            if (card) card.style.display = "block";
        } else if (origen === "sistema") {
            const card = document.querySelector(`#ejerciciosSistemaDisponibles .card-ejercicio[data-id="${id}"]`);
            if (card) card.style.display = "block";
        }
    } else if (tipo === "cliente") {
        const row = document.querySelector(`tr[data-id="${id}"]`);
        if (row) row.style.display = "";
    }
}

function capitalizar(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}

// Buscadores
document.getElementById("buscadorMisEjercicios").addEventListener("input", function () {
    const filtro = this.value.toLowerCase();
    document.querySelectorAll("#misEjerciciosDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre || "";
        card.style.display = nombre.includes(filtro) ? "block" : "none";
    });
});

document.getElementById("buscadorEjerciciosSistema").addEventListener("input", function () {
    const filtro = this.value.toLowerCase();
    document.querySelectorAll("#ejerciciosSistemaDisponibles .card-ejercicio").forEach(card => {
        const nombre = card.dataset.nombre || "";
        card.style.display = nombre.includes(filtro) ? "block" : "none";
    });
});
