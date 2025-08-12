function agregar(tipo, id, nombre, btn) {
    const lista = document.getElementById(`lista-${tipo}s`);

    // Crear item
    const li = document.createElement("li");
    li.className = "list-group-item bg-dark text-light d-flex justify-content-between align-items-center";
    li.dataset.id = id;
    li.innerHTML = `${nombre} 
        <button type="button" class="btn btn-sm btn-danger" onclick="quitar('${tipo}', ${id}, this)">X</button>
        <input type="hidden" name="Ids${capitalizar(tipo)}sSeleccionados" value="${id}" />`;

    lista.appendChild(li);

    // Ocultar origen
    if (tipo === "ejercicio") btn.closest(".card").style.display = "none";
    else btn.closest("tr").style.display = "none";
}

function quitar(tipo, id, btn) {
    const li = btn.closest("li");
    li.remove();

    // Mostrar origen
    if (tipo === "ejercicio") {
        const card = document.querySelector(`.card[data-id="${id}"]`);
        if (card) card.style.display = "block";
    } else {
        const row = document.querySelector(`tr[data-id="${id}"]`);
        if (row) row.style.display = "";
    }
}

function capitalizar(str) {
    return str.charAt(0).toUpperCase() + str.slice(1);
}
