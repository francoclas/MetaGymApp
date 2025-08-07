document.addEventListener("DOMContentLoaded", function () {
    const lat = document.getElementById("latitud")?.value;
    const lon = document.getElementById("longitud")?.value;

    if (lat && lon) {
        const url = `https://maps.google.com/maps?q=${lat},${lon}&hl=es&z=16&output=embed`;
        const iframe = document.createElement("iframe");
        iframe.src = url;
        iframe.width = "100%";
        iframe.height = "100%";
        iframe.style.border = "0";
        iframe.allowFullscreen = true;
        iframe.loading = "lazy";

        const contenedor = document.getElementById("mapaIframe");
        if (contenedor) contenedor.appendChild(iframe);
    }
});
