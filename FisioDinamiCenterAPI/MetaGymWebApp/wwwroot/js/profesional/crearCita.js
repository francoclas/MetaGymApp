document.getElementById("especialidadSelect").addEventListener("change", function () {
    const espId = this.value;
    if (!espId) return;

    fetch(`/Extra/ObtenerTiposPorEspecialidad?especialidadId=${espId}`)
        .then(res => res.json())
        .then(data => {
            const tipoSelect = document.getElementById("tipoAtencionSelect");
            tipoSelect.innerHTML = '<option value="">-- Seleccione --</option>';
            data.forEach(t => {
                tipoSelect.innerHTML += `<option value="${t.id}">${t.nombre}</option>`;
            });
        });
});