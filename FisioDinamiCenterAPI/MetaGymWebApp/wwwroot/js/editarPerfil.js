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