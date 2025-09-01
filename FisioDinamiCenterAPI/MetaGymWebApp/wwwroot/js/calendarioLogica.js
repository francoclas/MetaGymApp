
function inicializarCalendario(urlEventos, businessHoursConfig) {
    var calendarEl = document.getElementById('calendario');
    console.log("BusinessHoursConfig:", businessHoursConfig);

    var calendar = new FullCalendar.Calendar(calendarEl, {
        initialView: 'timeGridWeek',
        locale: 'es',
        timeZone: 'local',
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
                    url = '/Profesional/VerCitaParcial';
                    break;
            }

            $.get(url, { citaId: info.event.id }, function (html) {
                $('#panel-derecho').html(html);
            });
        },

        // Drag para mover cita
        eventDrop: function (info) {
            if (confirm("¿Desea reprogramar esta cita a la nueva fecha y hora?")) {
                let fecha = info.event.start;
                let nuevaFecha = fecha.getFullYear() + "-" +
                    String(fecha.getMonth() + 1).padStart(2, '0') + "-" +
                    String(fecha.getDate()).padStart(2, '0') + "T" +
                    String(fecha.getHours()).padStart(2, '0') + ":" +
                    String(fecha.getMinutes()).padStart(2, '0') + ":" +
                    String(fecha.getSeconds()).padStart(2, '0');
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
                    if (typeof inicializarCrearCitaParcial === "function") {
                        inicializarCrearCitaParcial();
                    }
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

