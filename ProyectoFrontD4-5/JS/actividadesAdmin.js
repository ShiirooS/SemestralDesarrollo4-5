let actividades = [];

const barrabuscador = document.getElementById("busqueda");
barrabuscador.addEventListener("keyup", (e) => {
    const searchString = e.target.value.toLowerCase();
    const filtroActividad = actividades.filter((actividad) =>
        actividad.nombreActividad.toLowerCase().includes(searchString)
    );
    mostrarActividades(filtroActividad);
});

async function ActividadesAdmin() {
    try {
        const response = await fetch("http://localhost:5041/api/Activid/actividades-completas");
        if (!response.ok) throw new Error("No se pudo mostrar la información");

        actividades = await response.json();
        mostrarActividades(actividades);
        document.getElementById("actividades-completas-admin").style.display = "block";
    } catch (error) {
        console.error("Error al cargar actividades:", error);
    }
}

function mostrarActividades(actividades) {
    const actividadesContenedor = document.getElementById("actividades-completas-admin");
    actividadesContenedor.innerHTML = actividades.length
        ? actividades.map(crearActividadCompleta).join("")
        : "<p>No hay actividades disponibles.</p>";
}

function crearActividadCompleta(actividad) {
    return `
        <div class="actividadesCompleta">
            <h3>${actividad.nombreActividad}</h3>
            <p>Día del Evento : ${new Date(actividad.fecha).toLocaleDateString()}</p>
            <p>Estado: ${actividad.estado}</p>
            <p>Cupos máximos: ${actividad.cuposMaximos}</p>
            <p>Cupos ocupados: ${actividad.cuposOcupados}</p>
            <button type="button" onclick="eliminarActividad(${actividad.idActividad})" class="eliminar-actividad">❌</button>
            <button type="button" onclick="window.location.href='detallesactAdmin.html?idActividad=${actividad.idActividad}'">Detalles</button>
        </div>`;
}

async function cargarCategorias() {
    const categoriaSelect = document.getElementById("categoria");
    try {
        const response = await fetch("http://localhost:5041/api/Categorias");
        if (!response.ok) throw new Error("No se pudieron cargar las categorías.");

        const categorias = await response.json();
        categoriaSelect.innerHTML = `<option value="">Todas las categorías</option>`;
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

document.getElementById("categoria").addEventListener("change", async (event) => {
    const categoriaId = event.target.value;
    try {
        const response = await fetch(
            categoriaId
                ? `http://localhost:5041/api/Activid/categoria/${categoriaId}`
                : "http://localhost:5041/api/Activid/actividades-completas"
        );
        if (!response.ok) throw new Error("Error al obtener actividades.");

        actividades = await response.json();
        mostrarActividades(actividades);
    } catch (error) {
        console.error("Error al filtrar actividades:", error);
        alert("Hubo un problema al obtener las actividades.");
    }
});
const estadoSelect = document.getElementById("estado");
    async function cargarEstados() {
    try {
        const response = await fetch("http://localhost:5041/api/EstadoActividad");
        if (!response.ok) throw new Error("No se pudieron cargar los estados.");
        const estados = await response.json();
        // Recorrer estadosActividades
        estados.estadosActividades.forEach((estado) => {
            const option = document.createElement("option");
            option.value = estado.idEstado;
            option.textContent = estado.nombreEstado;
            estadoSelect.appendChild(option);
        });
        } catch (error) {
        console.error("Error al cargar estados:", error);
        alert("Hubo un problema al cargar los estados.");
        }
    }
    estadoSelect.addEventListener("change",async (event) => {
        const idEstado =event.target.value;
        try {
            let response;
            if (!idEstado){
                response = await fetch("http://localhost:5041/api/Activid/actividades-completas");
            }else{
                response=await fetch(`http://localhost:5041/api/EstadoActividad/estado/${idEstado}`);
            }
            if (!response.ok) {
                throw new Error("Error al obtener actividades.")
            }
            const actividadesFiltradas = await response.json();
            actividades = actividadesFiltradas; 
            mostrarActividades(actividadesFiltradas);
        } catch (error) {
            console.error("Error al filtrar estados:", error);
            alert("Hubo un problema al obtener los estados.");
        }
    });

async function eliminarActividad(idActividad) {
    if (!confirm("¿Estás seguro de que deseas eliminar esta actividad?")) return;

    try {
        const response = await fetch(`http://localhost:5041/api/Activid/${idActividad}`, {
            method: "DELETE",
        });
        if (!response.ok) throw new Error(await response.text() || "Error al eliminar la actividad.");

        alert("Actividad eliminada exitosamente.");
        ActividadesAdmin();
    } catch (error) {
        console.error("Error al eliminar la actividad:", error);
        alert(`Hubo un problema al eliminar la actividad: ${error.message}`);
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    await cargarCategorias();
    await ActividadesAdmin();
    await cargarEstados();
});
