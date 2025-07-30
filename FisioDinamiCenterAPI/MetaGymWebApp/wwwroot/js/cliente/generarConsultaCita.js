const establecimientos = @Html.Raw(ViewBag.EstablecimientosJson);
document.getElementById("EstablecimientoId").addEventListener("change", function () {
    const seleccion = parseInt(this.value);
    const est = establecimientos.find(e => e.id === seleccion);

    const infoDiv = document.getElementById("infoEstablecimiento");
    const direccion = document.getElementById("direccionEstablecimiento");
    const imagen = document.getElementById("imagenEstablecimiento");

    if (est) {
        direccion.textContent = est.direccion;
        infoDiv.style.display = "block";

        if (est.urlMedia) {
            imagen.src = est.urlMedia;
            imagen.style.display = "block";
        } else {
            imagen.style.display = "none";
        }
    } else {
        infoDiv.style.display = "none";
    }
});