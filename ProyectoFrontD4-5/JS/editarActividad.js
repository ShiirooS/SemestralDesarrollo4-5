const params = new URLSearchParams(window.location.search);
const idActividad = params.get("idActividad");

const form = document.getElementById("editarActividadForm");
const nombreInput = document.getElementById("nombre");
const descripcionTextarea = document.getElementById("descripcion");
const fechaInput = document.getElementById("fecha");
const horaInput = document.getElementById("hora");
const cuposMaximosInput = document.getElementById("cuposMaximos");
const estadoSelect = document.getElementById("estado");
const costoInput = document.getElementById("costo");
const requisitosInput = document.getElementById("requisitos");

async function cargarActividad() {
    try {
        const response = await fetch(`http://localhost:5041/api/Activid/${idActividad}/detalles-completos`);
        if (!response.ok) throw new Error("Error al cargar los detalles de la actividad.");

        const { infoActividad } = await response.json();

        nombreInput.value = infoActividad.Nombre;
        descripcionTextarea.value = infoActividad.Descripcion;
        fechaInput.value = new Date(infoActividad.Fecha).toISOString().split('T')[0];
        horaInput.value = infoActividad.Hora.slice(0, 5);
        cuposMaximosInput.value = infoActividad.CuposMaximos;
        estadoSelect.value = infoActividad.Estado;
        costoInput.value = infoActividad.Costo;
        requisitosInput.value = infoActividad.Requisitos;
        
    } catch (error) {
        manejarError("Error al cargar los detalles de la actividad.", error);
    }
}

async function cargarEstados() {
    try {
        const response = await fetch("http://localhost:5041/api/Activid/estados-actividades");
        if (!response.ok) throw new Error("Error al cargar los estados de las actividades.");

        const estados = await response.json();
        estados.forEach(estado => {
            const option = document.createElement("option");
            option.value = estado.idEstado;
            option.textContent = estado.nombreEstado;
            estadoSelect.appendChild(option);
        });
    } catch (error) {
        manejarError("Error al cargar los estados.", error);
    }
}

async function guardarCambios(event) {
    event.preventDefault();

    const actividadActualizada = {
        IdActividad: parseInt(idActividad), 
        Nombre: nombreInput.value,
        Descripcion: descripcionTextarea.value,
        Fecha: fechaInput.value, 
        Hora: `${horaInput.value}:00`, 
        LimiteCupos: parseInt(cuposMaximosInput.value),
        Costo: parseFloat(costoInput.value),
        Requisitos: requisitosInput.value,
        IdEstado: parseInt(estadoSelect.value)
    };

    try {
        const response = await fetch(`http://localhost:5041/api/Activid/${idActividad}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(actividadActualizada)
        });

        if (!response.ok) throw new Error("Error al guardar los cambios de la actividad.");

        alert("Cambios guardados exitosamente.");
        window.location.href = `/detallesactAdmin.html?idActividad=${idActividad}`;
    } catch (error) {
        manejarError("Error al guardar los cambios.", error);
    }
}

function manejarError(mensaje, error) {
    console.error(error);
    alert(mensaje);
}

document.addEventListener("DOMContentLoaded", async () => {
    await cargarEstados();
    await cargarActividad();
});

form.addEventListener("submit", guardarCambios);