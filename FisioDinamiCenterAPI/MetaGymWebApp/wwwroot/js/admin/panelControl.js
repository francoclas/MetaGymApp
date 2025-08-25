document.addEventListener("DOMContentLoaded", () => {
    document.querySelectorAll(".datatable").forEach((tablaEl, index) => {
        const tabla = $(tablaEl).DataTable({
            pageLength: 10,
            lengthMenu: [10, 20, 50, 100],
            order: [],
            language: {
                url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
            }
        });

        const buscador = document.createElement("input");
        buscador.type = "text";
        buscador.placeholder = "Buscar...";
        buscador.classList.add("form-control", "mb-3");

        tablaEl.parentNode.insertBefore(buscador, tablaEl);

        buscador.addEventListener("keyup", function () {
            tabla.search(this.value).draw();
        });
    });
});
