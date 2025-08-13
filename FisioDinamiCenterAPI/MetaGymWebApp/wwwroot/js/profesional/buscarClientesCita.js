function filtrarClientes() {
    const campo = document.getElementById("filtroCampo").value.toLowerCase();
    const texto = document.getElementById("buscadorInput").value.toLowerCase();
    const filas = document.querySelectorAll("#tablaClientes tbody tr");

    filas.forEach(fila => {
        const valor = fila.querySelector(`td:nth-child(${campo === 'NombreCompleto' ? 1 : campo === 'Ci' ? 2 : 3})`).innerText.toLowerCase();
        fila.style.display = valor.includes(texto) ? "" : "none";
    });
}