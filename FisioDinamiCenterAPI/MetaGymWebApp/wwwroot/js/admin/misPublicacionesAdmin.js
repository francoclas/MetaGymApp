document.addEventListener("DOMContentLoaded", () => {
    const tabs = document.querySelectorAll("[data-tab]");
    const contents = document.querySelectorAll(".tab-content");

    tabs.forEach(tab => {
        tab.addEventListener("click", () => {
            tabs.forEach(t => t.classList.remove("active"));
            tab.classList.add("active");

            contents.forEach(c => c.classList.add("d-none"));
            document.getElementById(tab.dataset.tab).classList.remove("d-none");
        });
    });

    const configDT = {
        pageLength: 20,
        lengthMenu: [10, 20, 50, 100],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
        },
        order: [[0, "desc"]]
    };

    // Inicializar solo si la tabla existe
    if ($("#tablaCreadas").length) {
        $("#tablaCreadas").DataTable(configDT);
    }
    if ($("#tablaAprobadas").length) {
        $("#tablaAprobadas").DataTable(configDT);
    }
    if ($("#tablaRechazadas").length) {
        $("#tablaRechazadas").DataTable(configDT);
    }
});
