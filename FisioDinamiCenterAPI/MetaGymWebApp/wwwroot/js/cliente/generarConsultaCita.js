document.addEventListener("DOMContentLoaded", function () {
    const selectEspecialidad = document.getElementById("selectEspecialidad");
    const selectTipoAtencion = document.getElementById("selectTipoAtencion");
    const descripcionTipo = document.getElementById("descripcionTipo");
    const selectEstablecimiento = document.getElementById("selectEstablecimiento");
    const iframeMapa = document.getElementById("iframeMapa");
    const contenedorMapa = document.getElementById("contenedorMapa");

    const bloqueTipoAtencion = document.getElementById("bloqueTipoAtencion");
    const bloqueEstablecimiento = document.getElementById("bloqueEstablecimiento");
    const bloqueFecha = document.getElementById("bloqueFecha");
    const bloqueDescripcion = document.getElementById("bloqueDescripcion");
    const botonEnviar = document.getElementById("botonEnviar");

    // Cargar tipos según especialidad seleccionada
    function cargarTiposPorEspecialidad() {
        const especialidadId = selectEspecialidad.value;
        selectTipoAtencion.innerHTML = "<option value=''>Seleccione un tipo</option>";
        descripcionTipo.innerText = "";
        ocultarBloquesDesde(bloqueTipoAtencion);

        if (especialidadId) {
            const especialidad = especialidades.find(e => e.id == especialidadId || e.Id == especialidadId);
            if (especialidad && (especialidad.tipoAtenciones || especialidad.TipoAtenciones)) {
                const tipos = especialidad.tipoAtenciones || especialidad.TipoAtenciones;
                tipos.forEach(tipo => {
                    const opt = document.createElement("option");
                    opt.value = tipo.id || tipo.Id;
                    opt.textContent = tipo.nombre || tipo.Nombre;
                    opt.setAttribute("data-desc", tipo.desc || tipo.Desc);
                    selectTipoAtencion.appendChild(opt);
                });
                bloqueTipoAtencion.style.display = "block";
            }
        }
    }

    // Mostrar descripción del tipo de atención
    function mostrarDescripcionTipo() {
        const desc = selectTipoAtencion.selectedOptions[0]?.getAttribute("data-desc");
        descripcionTipo.innerText = desc || "";

        if (selectTipoAtencion.value) {
            bloqueEstablecimiento.style.display = "block";
        } else {
            ocultarBloquesDesde(bloqueEstablecimiento);
        }
    }

    // Mostrar mapa al elegir establecimiento
    function mostrarMapaEstablecimiento() {
        const opcion = selectEstablecimiento.selectedOptions[0];
        const lat = opcion?.getAttribute("data-lat");
        const lon = opcion?.getAttribute("data-lon");

        if (lat && lon) {
            iframeMapa.src = `https://maps.google.com/maps?q=${lat},${lon}&z=15&output=embed`;
            contenedorMapa.style.display = "block";
        } else {
            contenedorMapa.style.display = "none";
            iframeMapa.src = "";
        }

        if (selectEstablecimiento.value) {
            bloqueFecha.style.display = "block";
            bloqueDescripcion.style.display = "block";
            botonEnviar.style.display = "inline-block";
        } else {
            ocultarBloquesDesde(bloqueFecha);
        }
    }

    // Ocultar bloques desde uno en adelante
    function ocultarBloquesDesde(bloque) {
        bloque.style.display = "none";
        if (bloque === bloqueTipoAtencion) {
            bloqueEstablecimiento.style.display = "none";
            bloqueFecha.style.display = "none";
            bloqueDescripcion.style.display = "none";
            botonEnviar.style.display = "none";
        } else if (bloque === bloqueEstablecimiento) {
            bloqueFecha.style.display = "none";
            bloqueDescripcion.style.display = "none";
            botonEnviar.style.display = "none";
        } else if (bloque === bloqueFecha) {
            bloqueDescripcion.style.display = "none";
            botonEnviar.style.display = "none";
        }
    }

    // Eventos
    selectEspecialidad.addEventListener("change", cargarTiposPorEspecialidad);
    selectTipoAtencion.addEventListener("change", mostrarDescripcionTipo);
    selectEstablecimiento.addEventListener("change", mostrarMapaEstablecimiento);
});
