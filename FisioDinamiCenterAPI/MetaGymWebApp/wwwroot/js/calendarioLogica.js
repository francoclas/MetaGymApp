function inicializarCalendario(urlEventos, businessHoursConfig) {
    document.addEventListener('DOMContentLoaded', function () {
        var calendarEl = document.getElementById('calendario');

        var calendar = new FullCalendar.Calendar(calendarEl, {
            initialView: 'timeGridWeek',
            locale: 'es',
            allDaySlot: false,
            slotDuration: '00:30:00',
            businessHours: businessHoursConfig,
            events: urlEventos,
            editable: true,

            eventClick: function (info) {
                if (info.event.extendedProps.estado === "Finalizada") {
                    // Muestra solo vista
                    $.get('/Profesional/VerCitaParcial', { citaId: info.event.id }, function (html) {
                        $('#panel-derecho').html(html);
                    });
                } else {
                    // Vista editable
                    $.get('/Profesional/EditarCitaParcial', { citaId: info.event.id }, function (html) {
                        $('#panel-derecho').html(html);
                    });
                }
            },

            eventDrop: function (info) {
                if (confirm("¿Desea reprogramar esta cita a la nueva fecha y hora?")) {
                    var nuevaFecha = info.event.start.toISOString();
                    $.post('/Profesional/ReprogramarCita', { citaId: info.event.id, nuevaFecha: nuevaFecha })
                        .done(() => alert("Cita reprogramada"))
                        .fail(() => {
                            alert("Error al mover la cita");
                            info.revert();
                        });
                } else {
                    info.revert();
                }
            }
        });

        calendar.render();
    });
}
