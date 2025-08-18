function toggleNotificaciones() {
    const zona = document.getElementById("zonaNotificaciones");
    if (!zona) return;

    if (zona.innerHTML.trim() === "") {
        fetch('/Notificacion/NoLeidas', {
            headers: { "X-Requested-With": "Fetch" }
        })
            .then(r => {
                if (r.status === 401) {
                    // Sesión expirada → redirigir al login
                    window.location.href = "/Home/Login";
                    return null;
                }
                return r.text();
            })
            .then(html => {
                if (!html) return;
                zona.innerHTML = html;

                const nuevoMenu = document.getElementById("menuNotificaciones");
                if (nuevoMenu) {
                    nuevoMenu.style.display = "block";
                }
            });
    } else {
        zona.innerHTML = "";
    }
}


    // llamada actualizar contador
    function actualizarContadorNotificaciones() {
        fetch('/Notificacion/NoLeidasCount')
            .then(r => r.json())
            .then(data => {
                document.getElementById("contadorNoti").innerText = data;
            });
    }

    document.addEventListener("DOMContentLoaded", actualizarContadorNotificaciones);

//para ocultar menu
document.addEventListener("click", function (event) {
    const zona = document.getElementById("zonaNotificaciones");
    const boton = document.getElementById("btnNotificaciones");

    if (!zona || !boton) return;

    
    if (!zona.contains(event.target) && !boton.contains(event.target)) {
        zona.innerHTML = "";
    }
});
//marcar como vista
function marcarComoLeida(id) {
    fetch(`/Notificacion/MarcarComoLeida/${id}`, { method: 'POST' })
        .then(() => {
            actualizarContadorNotificaciones();
            toggleNotificaciones(); // refresca
        });
}