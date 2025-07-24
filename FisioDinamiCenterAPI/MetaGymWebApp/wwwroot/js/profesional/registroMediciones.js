let contadorMediciones = 0;

function agregarMedicion() {
    const contenedor = document.getElementById("contenedorMediciones");
    const id = `medicion-${contadorMediciones}`;
    const html = `
        <div class="card p-3 mb-3 bg-secondary text-white position-relative" id="${id}">
            <input type="hidden" name="Mediciones[${contadorMediciones}].Id" value="0" />
            <div class="mb-2">
                <label class="form-label">¿Qué se va a medir?</label>
                <input type="text" name="Mediciones[${contadorMediciones}].Nombre" class="form-control" placeholder="Ej: Peso levantado, duración..." required />
            </div>
            <div class="mb-2">
                <label class="form-label">¿En qué unidad?</label>
                <input type="text" name="Mediciones[${contadorMediciones}].Unidad" class="form-control" placeholder="Ej: kg, seg, reps..." required />
            </div>
            <div class="mb-2">
                <label class="form-label">Instrucción para el cliente</label>
                <input type="text" name="Mediciones[${contadorMediciones}].Descripcion" class="form-control" placeholder="Ej: Ingresá el peso en cada serie" required />
            </div>
            <div class="form-check mb-2">
                <input type="checkbox" name="Mediciones[${contadorMediciones}].EsObligatoria" class="form-check-input" />
                <label class="form-check-label">¿Es obligatoria esta medición?</label>
            </div>
            <button type="button" class="btn btn-sm btn-danger position-absolute top-0 end-0 m-2" onclick="eliminarMedicion('${id}')">✖</button>
        </div>`;

    contenedor.insertAdjacentHTML("beforeend", html);
    contadorMediciones++;
}

function eliminarMedicion(id) {
    const elemento = document.getElementById(id);
    if (elemento) {
        elemento.remove();
    }
}