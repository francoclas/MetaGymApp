document.getElementById("buscador").addEventListener("input", function () {
    const valor = this.value.toLowerCase();
    const rutinas = document.querySelectorAll(".rutina-item");

    rutinas.forEach(card => {
        const nombre = card.querySelector(".nombre").textContent.toLowerCase();
        const tipo = card.querySelector(".tipo").textContent.toLowerCase();

        if (nombre.includes(valor) || tipo.includes(valor)) {
            card.style.display = "block";
        } else {
            card.style.display = "none";
        }
    });
});