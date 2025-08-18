$(document).on("submit", "#formEditarCita", function (e) {
    e.preventDefault();
    $.post($(this).attr("action"), $(this).serialize())
        .done(function (res) {
            if (res.success) {
                alert(res.message);

                // Si usás FullCalendar v6 (como vi en tu código), refresca con:
                var calendarEl = document.getElementById('calendario');
                var calendar = FullCalendar.getCalendar(calendarEl);
                calendar.refetchEvents();

            } else {
                alert("Error: " + res.message);
            }
        })
        .fail(() => alert("Error en la comunicación con el servidor."));
});