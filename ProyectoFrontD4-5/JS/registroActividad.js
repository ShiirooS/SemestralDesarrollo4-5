// Función para obtener las actividades desde la API con filtro 
async function obtenerRegistro() {
    try {
        const filtroEstado = document.getElementById("filtro-estado").value;
        const userSession = JSON.parse(localStorage.getItem("userSession"));

        if (!userSession || !userSession.idUsuario) {
            alert("No se pudo obtener la información del usuario. Por favor, inicie sesión nuevamente.");
            window.location.href = "perfil.html";
            return;
        }

        const idUsuario = userSession.idUsuario;
            const response = await fetch(`http://localhost:5041/api/RegistroActividades?estado=${filtroEstado}&idUsuario=${idUsuario}`);
            
            if (!response.ok) {
                throw new Error(`Error en la API: ${response.statusText}`);
            }
        const actividades = await response.json();
        if (actividades.length === 0) {
            alert("No se encontraron actividades para mostrar.");
        } else {
            mostrarActividades(actividades);
        }
    } catch (error) {
        console.error("Error al obtener los registros de las actividades:", error);
        alert("Hubo un error al cargar los registros de las actividades.");
    }
}

function mostrarActividades(actividades) {
    const contenedor = document.getElementById("contenedor-actividades");
    contenedor.innerHTML = ""; 

    actividades.forEach((actividad) => {
        const actividadElement = document.createElement("div");
        actividadElement.className = "actividad";
        actividadElement.style.cursor = "pointer"; 
        actividadElement.onclick = () => {
            if (actividad.idActividad) {
                window.location.href = `inscripcion.html?idActividad=${actividad.idActividad}`;
            } else {
                alert("Esta Propuesta no tiene detalles disponibles.");
            }
        };

        actividadElement.innerHTML = `
            <div class="Contenedor-SectioActividad">
            <h3>${actividad.nombre}</h3>
            <p><strong>Estado:</strong> ${actividad.estado}</p>
            <p>${actividad.descripcion}</p>
            ${
                actividad.fechaInscripcion
                ? `<p><strong>Fecha de Inscripción:</strong> ${new Date(actividad.fechaInscripcion).toLocaleDateString()}</p>`
                : ''
            }
             ${
                actividad.comentarioRechazo
                    ? `<p><strong>Comentario de Rechazo:</strong> ${actividad.comentarioRechazo}</p>`
                    : ''
             }
            </div>
            
        `;

        contenedor.appendChild(actividadElement);
    });
}

document.getElementById("filtro-estado").addEventListener("change", obtenerRegistro);
document.addEventListener("DOMContentLoaded", obtenerRegistro);
