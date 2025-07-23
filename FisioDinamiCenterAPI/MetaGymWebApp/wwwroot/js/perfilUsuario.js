document.addEventListener("DOMContentLoaded", function () {
    const filtro = document.getElementById("filtroTipo");
    const orden = document.getElementById("ordenFecha");
    const lista = document.getElementById("listaNotificaciones");

    function filtrarYOrdenar() {
        const tipo = filtro.value;
        const ordenVal = orden.value;
        const items = Array.from(lista.querySelectorAll(".noti-item"));

        items.forEach(i => i.style.display = "block");

        if (tipo !== "todos") {
            items.forEach(i => {
                if (i.dataset.tipo !== tipo)
                    i.style.display = "none";
            });
        }

        const visibles = items.filter(i => i.style.display !== "none");
        visibles.sort((a, b) => {
            const t1 = parseInt(a.dataset.fecha);
            const t2 = parseInt(b.dataset.fecha);
            return ordenVal === "asc" ? t1 - t2 : t2 - t1;
        });

        visibles.forEach(i => lista.appendChild(i));
    }

    filtro.addEventListener("change", filtrarYOrdenar);
    orden.addEventListener("change", filtrarYOrdenar);
});