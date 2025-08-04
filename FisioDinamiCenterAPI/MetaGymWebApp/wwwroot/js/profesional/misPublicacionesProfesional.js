document.querySelectorAll('#tabFiltro .nav-link').forEach(link => {
    link.addEventListener('click', function (e) {
        e.preventDefault();

        document.querySelectorAll('#tabFiltro .nav-link').forEach(tab => tab.classList.remove('active'));
        this.classList.add('active');

        const tab = this.dataset.tab;
        document.querySelectorAll('.tab-publicaciones').forEach(div => div.classList.add('d-none'));
        document.querySelector(`#tabla-${tab}`).classList.remove('d-none');
    });
});

window.addEventListener('DOMContentLoaded', () => {
    document.querySelector('#tabFiltro .nav-link.active').click();
});
