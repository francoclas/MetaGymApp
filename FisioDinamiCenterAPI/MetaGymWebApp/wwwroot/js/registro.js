document.getElementById("registroForm").addEventListener("submit", function (e) {
    let errores = [];

    const ci = document.getElementById("ci").value.trim();
    const usuario = document.getElementById("usuario").value.trim();
    const nombre = document.getElementById("nombre").value.trim();
    const correo = document.getElementById("correo").value.trim();
    const telefono = document.getElementById("telefono").value.trim();
    const pass = document.getElementById("pass").value.trim();
    const confpass = document.getElementById("confpass").value.trim();

    // Validaciones
    if (!/^\d{8}$/.test(ci)) {
        errores.push("El CI debe tener exactamente 8 dígitos.");
    }

    if (usuario.length < 4) {
        errores.push("El nombre de usuario debe tener al menos 4 caracteres.");
    }

    if (nombre === "") {
        errores.push("El nombre completo es obligatorio.");
    }

    if (!/^\S+@\S+\.\S+$/.test(correo)) {
        errores.push("Ingrese un correo válido.");
    }

    if (!/^\d{8,}$/.test(telefono)) {
        errores.push("El teléfono debe tener al menos 8 dígitos numéricos.");
    }

    if (pass.length < 6) {
        errores.push("La contraseña debe tener al menos 6 caracteres.");
    }

    if (pass !== confpass) {
        errores.push("Las contraseñas no coinciden.");
    }

    // Mostrar errores
    const errorBox = document.getElementById("errorBox");
    errorBox.innerHTML = "";
    if (errores.length > 0) {
        e.preventDefault(); // cancela envío
        errores.forEach(err => {
            const div = document.createElement("div");
            div.className = "text-danger text-center mb-1";
            div.textContent = err;
            errorBox.appendChild(div);
        });
    }
});