document.querySelector("form[asp-action='GuardarCambiosPerfil']").addEventListener("submit", function (e) {
    const nuevaPass = document.getElementById("nuevaPass").value;
    const confirmarPass = document.getElementById("confirmarPass").value;
    const errorSpan = document.getElementById("errorPass");

    if (nuevaPass !== confirmarPass) {
        e.preventDefault();
        errorSpan.style.display = "block";
    } else {
        errorSpan.style.display = "none";
    }
});
// Validación del formulario de carga de imagen
const formFoto = document.querySelector("form[asp-action='ActualizarFotoPerfil']");
if (formFoto) {
    formFoto.addEventListener("submit", function (e) {
        const archivoInput = formFoto.querySelector("input[type='file']");
        if (!archivoInput.files || archivoInput.files.length === 0) {
            e.preventDefault();
            alert("Debes seleccionar una imagen antes de subirla.");
        }
    });
}