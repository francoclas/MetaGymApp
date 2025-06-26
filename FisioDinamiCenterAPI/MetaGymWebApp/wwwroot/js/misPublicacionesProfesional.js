        document.querySelectorAll('#tabFiltro .nav-link').forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();

                // Activar la pestaña
                document.querySelectorAll('#tabFiltro .nav-link').forEach(tab => tab.classList.remove('active'));
                this.classList.add('active');

                // Filtrar filas
                const estado = this.dataset.estado;
                document.querySelectorAll('#tablaPublicaciones tbody tr').forEach(row => {
                    if (row.dataset.estado === estado) {
                        row.style.display = '';
                    } else {
                        row.style.display = 'none';
                    }
                });
            });
        });

        // Activar filtro inicial (mostrar solo Pendientes)
        window.addEventListener('DOMContentLoaded', () => {
            document.querySelector('#tabFiltro .nav-link.active').click();
        });