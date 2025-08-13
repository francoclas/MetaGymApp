function filtrarHistorial() {
    const texto = document.getElementById("buscadorHistorial").value.toLowerCase();
    const filas = document.querySelectorAll("#tablaHistorial tbody tr");

    filas.forEach(fila => {
        const especialidad = fila.children[1].innerText.toLowerCase();
        const tipoAtencion = fila.children[2].innerText.toLowerCase();
        const coincide = especialidad.includes(texto) || tipoAtencion.includes(texto);
        fila.style.display = coincide ? "" : "none";
    });
}
