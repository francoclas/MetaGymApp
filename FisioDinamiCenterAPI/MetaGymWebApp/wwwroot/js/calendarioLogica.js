function inicializarCalendario(urlEventos, businessHoursConfig) {
    var calendarEl = document.getElementById('calendario');
    console.log("BusinessHoursConfig:", businessHoursConfig);

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        locale: 'es',
        allDaySlot: false,
        slotDuration: '00:30:00',
        businessHours: businessHoursConfig,
        nowIndicator: true,

        events: urlEventos,
        editable: true,
        selectable: true,

        // Click en un evento
        eventClick: function (info) {
            const estado = info.event.extendedProps.estado;

            let url = '';
            switch (estado) {
                case "Finalizada":
                    url = '/Profesional/VerCitaParcial';
                    break;
                case "EnEspera":
                    url = '/Profesional/VerCitaParcial';
                case "Aceptada":
                    url = '/Profesional/EditarCitaParcial';
                    break;
                case "Cancelada":
                    url = '/Profesional/VerCitaParcial';
                case "Rechazada":
                    url = '/Profesional/VerCitaParcial'; 
                    break;
                default:
                    url = '/Profesional/EditarCitaParcial';
                    break;
            }

            $.get(url, { citaId: info.event.id }, function (html) {
                $('#panel-derecho').html(html);
            });
        },

        // Drag para mover cita
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
        },

        // Selección de un rango/hora
        select: function (info) {
            let fecha = info.startStr;
            $("#fecha-seleccionada").text("Fecha seleccionada: " + new Date(fecha).toLocaleString());
            $("#acciones-cita").removeClass("d-none");

            $("#btn-crear-cita").off("click").on("click", function () {
                $.get('/Profesional/CrearCitaParcial', { fecha: fecha }, function (html) {
                    $('#panel-derecho').html(html);
                });
            });

            $("#btn-cancelar-seleccion").off("click").on("click", function () {
                $("#acciones-cita").addClass("d-none");
                calendar.unselect();
            });
        }
    });

    calendar.render();
}

function cerrarPanelDerecho() {
    $("#panel-derecho").html(""); // vacía el panel
}
