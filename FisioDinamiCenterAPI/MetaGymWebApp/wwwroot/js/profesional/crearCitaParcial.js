function inicializarCrearCitaParcial() {
    console.log("Inicializando CrearCitaParcial...");

    // === Buscador de clientes ===
    const buscador = document.getElementById("buscadorClienteParcial");
    const tabla = document.getElementById("tablaClientesParcial");
    if (buscador && tabla) {
        const filas = tabla.querySelectorAll("tbody tr");

        buscador.addEventListener("keyup", function () {
            const filtro = buscador.value.toLowerCase();
            filas.forEach(fila => {
                const celdas = fila.querySelectorAll("td");
                const match = Array.from(celdas).some((c, i) => i < 2 && c.textContent.toLowerCase().includes(filtro));
                fila.style.display = match ? "" : "none";
            });
        });

        tabla.addEventListener("click", function (e) {
            if (e.target.classList.contains("seleccionar-cliente-parcial")) {
                const id = e.target.dataset.id;
                const nombre = e.target.dataset.nombre;

                document.getElementById("clienteIdParcial").value = id;
                document.getElementById("clienteNombreParcial").value = nombre;
            }
        });
    }

    // === Especialidad → TipoAtencion ===
    const selectEspecialidad = document.getElementById("selectEspecialidadParcial");
    const selectTipoAtencion = document.getElementById("selectTipoAtencionParcial");
    const descripcionTipo = document.getElementById("descripcionTipoParcial");

    if (selectEspecialidad && selectTipoAtencion) {
        selectEspecialidad.addEventListener("change", function () {
            const especialidadId = selectEspecialidad.value;
            selectTipoAtencion.innerHTML = "<option value=''>Seleccione un tipo</option>";
            if (descripcionTipo) descripcionTipo.innerText = "";

            if (especialidadId) {
                const especialidad = especialidades.find(e => e.id == especialidadId || e.Id == especialidadId);
                if (especialidad && (especialidad.tipoAtenciones || especialidad.TipoAtenciones)) {
                    const tipos = especialidad.tipoAtenciones || especialidad.TipoAtenciones;
                    tipos.forEach(tipo => {
                        const opt = document.createElement("option");
                        opt.value = tipo.id || tipo.Id;
                        opt.textContent = tipo.nombre || tipo.Nombre;
                        opt.setAttribute("data-desc", tipo.desc || tipo.Desc || tipo.Descripcion);
                        selectTipoAtencion.appendChild(opt);
                    });
                }
            }
        });

        selectTipoAtencion.addEventListener("change", function () {
            const desc = selectTipoAtencion.selectedOptions[0]?.getAttribute("data-desc");
            if (descripcionTipo) descripcionTipo.innerText = desc || "";
        });
    }
}
