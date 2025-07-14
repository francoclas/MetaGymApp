const filtro = document.getElementById("filtroCitas");
const citas = document.querySelectorAll(".cita-card");
const mensajeEstado = document.getElementById("mensajeEstado");
const sinResultados = document.getElementById("sinResultados");

filtro.addEventListener("input", () => {
    filtrarEstado(estadoActual);
});

let estadoActual = "";

function filtrarEstado(estado) {
    estadoActual = estado;
    const texto = filtro.value.toLowerCase();
    let hayResultados = false;

    citas.forEach(cita => {
        const estadoCita = cita.dataset.estado;
        const contenido = cita.textContent.toLowerCase();
        const visible = (estado === "" || estadoCita === estado) && contenido.includes(texto);

        cita.style.display = visible ? "block" : "none";
        if (visible) hayResultados = true;
    });

    // Mensaje segun estado
    let mensaje = "Mostrando todas las citas";
    if (estado === "EnEspera") mensaje = "Estas solicitudes están en espera de revisión";
    else if (estado === "Aceptada") mensaje = "Citas próximas a realizar asistencia";
    else if (estado === "FinalizadaCancelada") mensaje = "Historial de citas finalizadas o canceladas";

    mensajeEstado.textContent = mensaje;
    sinResultados.style.display = hayResultados ? "none" : "block";
}

function ordenarCitas(direccion) {
    const contenedor = document.getElementById("contenedor-citas");
    const citasArray = Array.from(citas);

    citasArray.sort((a, b) => {
        const fechaA = new Date(a.dataset.fecha);
        const fechaB = new Date(b.dataset.fecha);
        return direccion === "asc" ? fechaA - fechaB : fechaB - fechaA;
    });

    citasArray.forEach(c => contenedor.appendChild(c));
}

// Inicial: mostrar todo
filtrarEstado("");