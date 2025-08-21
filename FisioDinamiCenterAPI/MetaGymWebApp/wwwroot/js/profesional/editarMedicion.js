let contadorMediciones = document.querySelectorAll("#contenedorMediciones .card").length || 0;
document.addEventListener("DOMContentLoaded", () => {
    const botonEliminar = document.getElementById("btnEliminarEjercicio");
    if (botonEliminar) {
        botonEliminar.addEventListener("click", (e) => {
            const confirmado = confirm("⚠️ ¿Seguro que querés eliminar este ejercicio? Esta acción no se puede deshacer.");
            if (!confirmado) {
                e.preventDefault();
            }
        });
    }
});
function agregarMedicion() {
    const contenedor = document.getElementById("contenedorMediciones");

    const div = document.createElement("div");
    div.className = "card p-3 mb-3 bg-secondary text-white position-relative";
    div.id = `medicion-${contadorMediciones}`;

    div.innerHTML = `
        <input type="hidden" name="Mediciones[${contadorMediciones}].Id" value="0" />
        <div class="mb-2">
            <label class="form-label">¿Qué se va a medir?</label>
            <input type="text" name="Mediciones[${contadorMediciones}].Nombre" class="form-control" required />
        </div>
        <div class="mb-2">
            <label class="form-label">¿En qué unidad?</label>
            <input type="text" name="Mediciones[${contadorMediciones}].Unidad" class="form-control" required />
        </div>
        <div class="mb-2">
            <label class="form-label">Instrucción para el cliente</label>
            <input type="text" name="Mediciones[${contadorMediciones}].Descripcion" class="form-control" required />
        </div>
        <div class="form-check mb-2">
            <input type="checkbox" name="Mediciones[${contadorMediciones}].EsObligatoria" class="form-check-input" />
            <label class="form-check-label">¿Es obligatoria?</label>
        </div>
        <button type="button" class="btn btn-sm btn-danger position-absolute top-0 end-0 m-2" onclick="eliminarMedicion('medicion-${contadorMediciones}')">✖</button>
    `;

    contenedor.appendChild(div);
    contadorMediciones++;
}

function eliminarMedicion(idElemento) {
    const elemento = document.getElementById(idElemento);
    if (elemento) {
        elemento.remove();
    }
}

// Para eliminar archivos multimedia (reutilizado desde editar publicación)
function eliminarMedia(mediaId, ejercicioId) {
    if (!confirm("¿Estás seguro de que querés eliminar este archivo?")) return;

    const form = document.createElement('form');
    form.method = 'POST';
    form.action = '/Profesional/EliminarMedia';

    const input1 = document.createElement('input');
    input1.type = 'hidden';
    input1.name = 'mediaId';
    input1.value = mediaId;

    const input2 = document.createElement('input');
    input2.type = 'hidden';
    input2.name = 'EjercicioId';
    input2.value = ejercicioId;

    form.appendChild(input1);
    form.appendChild(input2);
    document.body.appendChild(form);
    form.submit();
}
