document.addEventListener("DOMContentLoaded", () => {
    const campos = {
        ci: { selector: "#ci", error: "#error-ci", validador: v => /^\d{8}$/.test(v), mensaje: "El CI debe tener 8 dígitos." },
        usuario: { selector: "#usuario", error: "#error-usuario", validador: v => v.length >= 4, mensaje: "Mínimo 4 caracteres." },
        nombre: { selector: "#nombre", error: "#error-nombre", validador: v => v.trim() !== "", mensaje: "Campo obligatorio." },
        correo: { selector: "#correo", error: "#error-correo", validador: v => /^\S+@\S+\.\S+$/.test(v), mensaje: "Correo no válido." },
        telefono: { selector: "#telefono", error: "#error-telefono", validador: v => /^\d{8,}$/.test(v), mensaje: "Al menos 8 dígitos." },
        pass: { selector: "#pass", error: "#error-pass", validador: v => v.length >= 6, mensaje: "Mínimo 6 caracteres." },
        confpass: {
            selector: "#confpass",
            error: "#error-confpass",
            validador: () => document.querySelector("#pass").value === document.querySelector("#confpass").value,
            mensaje: "Las contraseñas no coinciden."
        }
    };

    const btnSubmit = document.querySelector("#btn-submit");

    function validarCampo(campo) {
        const input = document.querySelector(campo.selector);
        const error = document.querySelector(campo.error);
        const valor = input.value.trim();

        const valido = campo.validador(valor);
        error.textContent = valido ? "" : campo.mensaje;
        return valido;
    }

    function validarTodo() {
        let esValido = true;
        for (let key in campos) {
            const valido = validarCampo(campos[key]);
            if (!valido) esValido = false;
        }
        btnSubmit.disabled = !esValido;
    }

    // Asignar eventos en tiempo real
    for (let key in campos) {
        const input = document.querySelector(campos[key].selector);
        input.addEventListener("input", validarTodo);
    }

    document.getElementById("registroForm").addEventListener("submit", function (e) {
        if (btnSubmit.disabled) {
            e.preventDefault();
        }
    });

    validarTodo(); // Validar al cargar
});
