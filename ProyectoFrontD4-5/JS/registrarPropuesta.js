document.addEventListener("DOMContentLoaded", async () => {
    const formPropuesta = document.getElementById("formPropuesta");
    const categoriaSelect = document.getElementById("categoria");
    const subcategoriaSelect = document.getElementById("subcategoria");

    function verificarSesion() {
        const userSession = JSON.parse(localStorage.getItem("userSession"));
        if (!userSession || !userSession.idUsuario) {
            alert("Debes iniciar sesión para registrar una propuesta.");
            window.location.href = "perfil.html";
            return null;
        }
        return userSession;
    }

    function validarHora(hora) {
        const regex = /^([01]\d|2[0-3]):([0-5]\d):([0-5]\d)$/;
        return regex.test(hora);
    }

    // Función para cargar categorías al iniciar
    async function cargarCategorias() {
        try {
            const response = await fetch("http://localhost:5041/api/Categorias");
            if (!response.ok) throw new Error("No se pudieron cargar las categorías.");

            const categorias = await response.json();
            categorias.forEach((categoria) => {
                const option = document.createElement("option");
                option.value = categoria.idCategoria;
                option.textContent = categoria.nombre;
                categoriaSelect.appendChild(option);
            });
        } catch (error) {
            console.error("Error al cargar categorías:", error);
            alert("Hubo un problema al cargar las categorías.");
        }
    }

    categoriaSelect.addEventListener("change", async () => {
        const categoriaId = categoriaSelect.value;
        subcategoriaSelect.innerHTML = '<option value="">Seleccione una subcategoría</option>';

        if (!categoriaId) return;

        try {
            const response = await fetch(`http://localhost:5041/api/Categorias/${categoriaId}/subcategorias`);
            if (!response.ok) throw new Error("No se pudieron cargar las subcategorías.");

            const subcategorias = await response.json();
            subcategorias.forEach((subcategoria) => {
                const option = document.createElement("option");
                option.value = subcategoria.idSubcategoria;
                option.textContent = subcategoria.nombre;
                subcategoriaSelect.appendChild(option);
            });
        } catch (error) {
            console.error("Error al cargar subcategorías:", error);
            alert("Hubo un problema al cargar las subcategorías.");
        }
    });

    function validarFormulario() {
        const nombre = document.getElementById("nombreActividad").value;
        const descripcion = document.getElementById("descripcion").value;
        const categoria = document.getElementById("categoria").value;
        const subcategoria = document.getElementById("subcategoria").value;
        const lugar = document.getElementById("lugar").value;
        const fecha = document.getElementById("fecha").value;
        const hora = document.getElementById("hora").value;
        const limiteCupos = document.getElementById("limiteCupos").value;
        const requisitos = document.getElementById("requisitos").value;
        if (!nombre || !categoria || !subcategoria || !lugar || !fecha || !hora || !limiteCupos || !requisitos) {
            alert("Por favor, complete todos los campos obligatorios.");
            return false;
        }

        return true;
    }
    formPropuesta.addEventListener("submit", async (event) => {
        event.preventDefault();

        const fechaSeleccionada = new Date(document.getElementById("fecha").value);
        const fechaActual = new Date();
        if (fechaSeleccionada < fechaActual) {
            alert("La fecha de la propuesta no puede ser anterior a la fecha actual.");
            return;
        }
        const hora = document.getElementById("hora").value;
        if (!validarHora(hora)) {
            alert("Por favor, ingrese una hora válida en formato HH:MM:SS.");
            return;
        }
        const userSession = verificarSesion();
        if (!userSession) return;

        if (!validarFormulario()) {
            return; 
        }

        const propuesta = {
            idUsuario: userSession.idUsuario,
            nombre: document.getElementById("nombreActividad").value,
            descripcion: document.getElementById("descripcion").value || null,
            idSubcategoria: parseInt(document.getElementById("subcategoria").value),
            lugar: document.getElementById("lugar").value,
            fecha: new Date(document.getElementById("fecha").value).toISOString().split("T")[0],
            hora: hora,
            limiteCupos: parseInt(document.getElementById("limiteCupos").value),
            costo: parseFloat(document.getElementById("precio").value) || 0,
            requisitos: document.getElementById("requisitos").value,
            imagenUrl: document.getElementById("imagenUrl").value || null,
        };

        try {
            const response = await fetch("http://localhost:5041/api/PropuestasActividades", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(propuesta),
            });

            if (!response.ok) {
                const error = await response.json().catch(() => null);
                if (response.status === 409) {
                    if (error.field) {
                        alert(`El campo "${error.field}" ya tiene una propuesta registrada. ${error.message}`);
                    } else {
                        alert("Ya existe una propuesta similar. Por favor, verifica los datos.");
                    }
                } else {
                    console.error("Error del servidor:", error || response.statusText);
                    alert(`Error: ${error?.message || "Ocurrió un problema."}`);
                }
                return;
            }

            alert("¡Propuesta registrada con éxito!");
            formPropuesta.reset();
        } catch (error) {
            console.error("Error al registrar la propuesta:", error);
            alert("Hubo un problema al intentar registrar la propuesta.");
        }
    });

    const userSession = verificarSesion();
    if (userSession) {
        await cargarCategorias();
    }
});
