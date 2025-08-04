document.addEventListener("DOMContentLoaded", function () {
    const filtro = document.getElementById("filtroTipo");
    const orden = document.getElementById("ordenFecha");
    const buscador = document.getElementById("buscadorTexto");
    const lista = document.getElementById("listaNotificaciones");

    function filtrarYOrdenar() {
        const tipo = filtro.value;
        const ordenVal = orden.value;
        const texto = buscador.value.toLowerCase();
        const items = Array.from(lista.querySelectorAll(".noti-item"));

        items.forEach(i => {
            const coincideTipo = (tipo === "todos" || i.dataset.tipo === tipo);
            const coincideTexto = i.dataset.texto.includes(texto);
            i.style.display = (coincideTipo && coincideTexto) ? "block" : "none";
        });

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
    buscador.addEventListener("input", filtrarYOrdenar);
});
