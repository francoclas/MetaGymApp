document.addEventListener("DOMContentLoaded", function () {
    // Tabs
    const tabs = document.querySelectorAll("#tabFiltro .nav-link");
    const secciones = document.querySelectorAll(".tab-publicaciones");

    tabs.forEach(tab => {
        tab.addEventListener("click", function (e) {
            e.preventDefault();

            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            secciones.forEach(sec => sec.classList.add("d-none"));
            const target = document.getElementById("tabla-" + tab.dataset.tab);
            if (target) target.classList.remove("d-none");
        });
    });

    // Inicializar DataTables en cada tabla de publicaciones
    ["#tablaPendientes", "#tablaAprobadas", "#tablaRechazadas"].forEach(id => {
        if ($(id).length) {
            $(id).DataTable({
                paging: true,
                pageLength: 20,
                lengthChange: false,
                ordering: true,
                order: [[2, "desc"]], // ordenar por fecha por defecto
                language: {
                    url: "//cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
                },
                columnDefs: [
                    { orderable: false, targets: -1 } // Acciones no ordenables
                ]
            });
        }
    });
});
