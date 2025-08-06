document.addEventListener("DOMContentLoaded", function () {
    console.log("JS cargado");

    const especialidadSelect = document.getElementById("especialidadSelect");
    const tipoAtencionSelect = document.getElementById("tipoAtencionSelect");
    const descTipo = document.getElementById("descTipo");

    const establecimientoSelect = document.getElementById("establecimientoSelect");
    const iframeMapa = document.getElementById("iframeMapa");
    const mapaDiv = document.getElementById("mapaEstablecimiento");

    function filtrarTiposAtencionPorEspecialidad() {
        const especialidadId = especialidadSelect.value;

        const opciones = tipoAtencionSelect.querySelectorAll("option");
        opciones.forEach(op => {
            const espId = op.getAttribute("data-especialidad");
            if (!espId || espId === especialidadId || op.value === "") {
                op.style.display = "block";
            } else {
                op.style.display = "none";
            }
        });

        tipoAtencionSelect.value = "";
        descTipo.innerText = "";
    }

    function mostrarDescripcionTipoAtencion() {
        const desc = tipoAtencionSelect.selectedOptions[0]?.getAttribute("data-desc");
        descTipo.innerText = desc || "";
    }

    function mostrarMapaEstablecimiento() {
        const option = establecimientoSelect.selectedOptions[0];
        const lat = option.getAttribute("data-lat");
        const lon = option.getAttribute("data-lon");

        if (lat && lon) {
            iframeMapa.src = `https://maps.google.com/maps?q=${lat},${lon}&z=15&output=embed`;
            mapaDiv.style.display = "block";
        } else {
            mapaDiv.style.display = "none";
            iframeMapa.src = "";
        }
    }

    especialidadSelect.addEventListener("change", filtrarTiposAtencionPorEspecialidad);
    tipoAtencionSelect.addEventListener("change", mostrarDescripcionTipoAtencion);
    establecimientoSelect.addEventListener("change", mostrarMapaEstablecimiento);
});