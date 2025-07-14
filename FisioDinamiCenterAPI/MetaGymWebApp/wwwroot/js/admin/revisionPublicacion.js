document.addEventListener("DOMContentLoaded", function () {
    const btnAceptar = document.getElementById("btn-aceptar");
    const btnRechazar = document.getElementById("btn-rechazar");
    const btnConfirmar = document.getElementById("btn-confirmar");

    const accionInput = document.getElementById("accion");
    const motivoInput = document.getElementById("motivoOculto");
    const motivoTextarea = document.getElementById("motivoRechazo");
    const motivoContainer = document.getElementById("motivo-rechazo-container");

    btnAceptar.addEventListener("click", function () {
        accionInput.value = "Aceptar";
        motivoContainer.classList.add("d-none");
        btnConfirmar.classList.remove("d-none");
    });

    btnRechazar.addEventListener("click", function () {
        accionInput.value = "Rechazar";
        motivoContainer.classList.remove("d-none");
        btnConfirmar.classList.remove("d-none");
    });

    // Al enviar el formulario, si es rechazo, pasamos el motivo
    btnConfirmar.closest("form").addEventListener("submit", function () {
        if (accionInput.value === "Rechazar") {
            motivoInput.value = motivoTextarea.value;
        }
    });
});
