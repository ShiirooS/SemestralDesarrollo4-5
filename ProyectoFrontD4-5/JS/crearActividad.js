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
        const regex = /^([01]\d|2[0-3]):([0-5]\d)(:[0-5]\d)?$/; 
        return regex.test(hora);
    }
    async function cargarCategorias() {
        try {
            categoriaSelect.innerHTML = '<option value="">Seleccione una categoría</option>';

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
    formPropuesta.addEventListener("submit", async (event) => {
        event.preventDefault();
        const hora = document.getElementById("hora").value;
        if (!validarHora(hora)) {
            alert("Por favor, ingrese una hora válida en formato HH:MM o HH:MM:SS.");
            return;
        }
        const fechaIngresada = new Date(document.getElementById("fecha").value);
        const fechaActual = new Date();
        if (fechaIngresada.setHours(0, 0, 0, 0) < fechaActual.setHours(0, 0, 0, 0)) {
            alert("La fecha de la actividad no puede ser anterior a la fecha actual.");
            return;
        }
        const userSession = verificarSesion();
        if (!userSession) return;
        const gruposCheckboxes = document.querySelectorAll('input[name="grupos"]:checked');
        const gruposDestinatarios = Array.from(gruposCheckboxes)
            .map((checkbox) => checkbox.value)
            .join(", ");
    
        const actividad = {
            nombre: document.getElementById("nombreActividad").value,
            descripcion: document.getElementById("descripcion").value || null,
            idSubcategoria: parseInt(document.getElementById("subcategoria").value),
            lugar: document.getElementById("lugar").value,
            fecha: document.getElementById("fecha").value,
            hora: hora,
            limiteCupos: parseInt(document.getElementById("limiteCupos").value),
            costo: parseFloat(document.getElementById("precio").value) || 0,
            requisitos: document.getElementById("requisitos").value,
            imagenUrl: document.getElementById("imagenUrl").value || null,
            gruposDestinatarios: gruposDestinatarios,
        };
    
        try {
            const response = await fetch("http://localhost:5041/api/Activid/crear", {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(actividad),
            });
    
            if (!response.ok) {
                const error = await response.json().catch(() => null);
                console.error("Error del servidor:", error || response.statusText);
                alert(`Error: ${error?.message || "Ocurrió un problema al crear la actividad."}`);
                return;
            }
    
            alert("¡Actividad creada con éxito!");
            formPropuesta.reset();
        } catch (error) {
            console.error("Error al crear la actividad:", error);
            alert("Hubo un problema al intentar crear la actividad.");
        }
    });
    if (verificarSesion()) {
        await cargarCategorias();
    }
});
document.addEventListener("DOMContentLoaded", () => {
    const cancelButton = document.getElementById("cancelButton");

    cancelButton.addEventListener("click", () => {
        window.location.href = "/indexAdministrativo.html";
    });
});
