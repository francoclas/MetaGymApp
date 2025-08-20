document.addEventListener("DOMContentLoaded", () => {
    $(".datatable").each(function () {
        if (!$.fn.DataTable.isDataTable(this)) { 
            $(this).DataTable({
                pageLength: 20,
                lengthMenu: [10, 20, 50, 100],
                order: [[0, "desc"]],
                language: {
                    url: "https://cdn.datatables.net/plug-ins/1.13.6/i18n/es-ES.json"
                }
            });
        }
    });
});