// Función para obtener parámetros de la URL
function obtenerParametroURL(parametro) {
    const urlParams = new URLSearchParams(window.location.search);
    return urlParams.get(parametro);
}

async function obtenerDetallesPropuesta(idPropuesta) {
    try {
        const response = await fetch(`http://localhost:5041/api/PropuestasActividades/${idPropuesta}/detalles-completos`);
        if (!response.ok) throw new Error("No se pudieron obtener los detalles de la propuesta.");

        const propuesta = await response.json();
        mostrarDetallesPropuesta(propuesta);
    } catch (error) {
        console.error(error);
        alert("Hubo un problema al cargar los detalles de la propuesta.");
    }
}
// Función para enviar los detalles en el HTML
function mostrarDetallesPropuesta(propuesta) {
    const contenedorDetalles = document.getElementById("detalles-propuesta");
    contenedorDetalles.innerHTML = `
        <div class="detalle-propuesta">
            <div class="titulo-DetallesPropuesta">
                <h2>${propuesta.nombre}</h2>
                <img src="${propuesta.imagen}" class="imagen-propuesta"/>
            </div>
            
            <div class="Contenedor-Parraf-DetallesPropuestas">
                <div class="parraf-izq-DetallesPropuestas">
                    <p><strong>Descripción:</strong> ${propuesta.descripcion}</p>
                    <p><strong>Fecha:</strong> ${new Date(propuesta.fecha).toLocaleDateString()}</p>
                    <p><strong>Hora:</strong> ${propuesta.hora}</p>
                    <p><strong>Lugar:</strong> ${propuesta.lugar}</p>
                    <p><strong>Cupos Máximos:</strong> ${propuesta.limiteCupos}</p>
                </div>
                
                <div class="Parraf-derech-DetallesPropuesta">
                    
                    
                    <p><strong>Costo:</strong> $${propuesta.costo}</p>
                    <p><strong>Requisitos:</strong> ${propuesta.requisitos}</p>
                    <p><strong>Estado:</strong> ${propuesta.estado}</p>
                    <p><strong>Propuesta por:</strong> ${propuesta.nombreUsuario} (${propuesta.correoUsuario})</p>
                    
                </div>
            </div>
            

        </div>
    `;
}
// ----------------------------------Aprobar Propuesta------------------------------------
async function aprobarPropuesta(idPropuesta) {
    if (!confirm("¿Estás seguro de aprobar esta propuesta?")) return;

    try {
        // Obtener detalles de la propuesta para verificar el estado
        const responseDetalles = await fetch(`http://localhost:5041/api/PropuestasActividades/${idPropuesta}/detalles-completos`);
        if (!responseDetalles.ok) throw new Error("No se pudieron obtener los detalles de la propuesta.");
        
        const propuesta = await responseDetalles.json();

        // Validar si la propuesta ya fue rechazada
        if (propuesta.estado === "Rechazada") {
            alert("No se puede aprobar una propuesta que ya fue rechazada.");
            return;
        }

        // Realizar la solicitud de aprobación
        const response = await fetch(`http://localhost:5041/api/PropuestasActividades/${idPropuesta}/aprobar`, {
            method: "POST"
        });

        if (!response.ok) {
            const error = await response.json();
            if (error.message === "La propuesta ya ha sido aprobada.") {
                alert("Esta propuesta ya fue aprobada.");
                return;
            }
            throw new Error(error.message || "Hubo un problema al aprobar la propuesta.");
        }

        const nuevaActividad = await response.json();
        alert("La propuesta ha sido aprobada y convertida en una actividad.");
        actualizarEstadoPropuesta(idPropuesta, "Aprobada");
        agregarActividad(nuevaActividad);
    } catch (error) {
        console.error(error);
        alert(error.message);
    }
}

// ----------------------------------Rechazar Propuesta-----------------------------------
async function rechazarPropuesta(idPropuesta) {
    const comentarioRechazo = prompt("Ingrese un comentario para el rechazo de la propuesta:");
    if (!comentarioRechazo || comentarioRechazo.trim() === "") {
        alert("Debe ingresar un comentario para rechazar la propuesta.");
        return;
    }
    try {
        const response = await fetch(`http://localhost:5041/api/PropuestasActividades/${idPropuesta}/rechazar`, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(comentarioRechazo)
        });

        if (!response.ok) {
            const error = await response.json();
            if(error.message==="La propuesta ya ha sido rechazada."){
                alert("Esta propuesta ya fue rechazada.");
                return;
            }
            throw new Error(error.message || "Hubo un problema al rechazar la propuesta.");
        }

        alert("La propuesta ha sido rechazada.");
        actualizarEstadoPropuesta(idPropuesta, "Rechazada");
    } catch (error) {
        console.error(error);
        alert("Hubo un error al rechazar la propuesta.");
    }
}

// ----------------------------------Actualizar Estado Propuesta-------------------------
function actualizarEstadoPropuesta(idPropuesta, estado) {
    const propuestaElemento = document.querySelector(`#propuesta-${idPropuesta}`);
    if (propuestaElemento) propuestaElemento.querySelector(".estado").innerText = estado;
}
// ----------------------------------Agregar Nueva Actividad-----------------------------
function agregarActividad(nuevaActividad) {
    const actividadesContenedor = document.getElementById("actividades-completas-admin");
    if (!actividadesContenedor) {
        console.error("El contenedor de actividades no existe.");
        return;
    }
    const actividadHTML = `
        <div class="actividadesCompleta">
            <h3>${nuevaActividad.Nombre}</h3>
            <p>Fecha: ${new Date(nuevaActividad.Fecha).toLocaleDateString()}</p>
            <p>Estado: Activo</p>
            <p>Cupos máximos: ${nuevaActividad.LimiteCupos}</p>
            <p>Cupos ocupados: 0</p>
            <button type="button" onclick="eliminarActividad(${nuevaActividad.IdActividad})" class="eliminar-actividad">❌</button>
            <button type="button" onclick="window.location.href='detallesactAdmin.html?idActividad=${nuevaActividad.IdActividad}'">Detalles</button>
        </div>
    `;
    actividadesContenedor.insertAdjacentHTML("beforeend", actividadHTML);
}
// ----------------------------------Inicializar Eventos----------------------------------
document.addEventListener("DOMContentLoaded", () => {
    const idPropuesta = obtenerParametroURL("idPropuesta");
    if (idPropuesta) {
        obtenerDetallesPropuesta(idPropuesta);
    } else {
        alert("No se encontró el ID de la propuesta en la URL.");
    }

    const aprobarBtn = document.getElementById("aprobar-btn");
    const rechazarBtn = document.getElementById("rechazar-btn");

    if (aprobarBtn) aprobarBtn.addEventListener("click", () => aprobarPropuesta(idPropuesta));
    if (rechazarBtn) rechazarBtn.addEventListener("click", () => rechazarPropuesta(idPropuesta));
});
