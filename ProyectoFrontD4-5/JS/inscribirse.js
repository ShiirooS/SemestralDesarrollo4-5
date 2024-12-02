document.addEventListener("DOMContentLoaded", async () => {
    const idActividad = new URLSearchParams(window.location.search).get("idActividad");
    const userSession = JSON.parse(localStorage.getItem("userSession"));

    if (idActividad) {
        try {
            const actividadResponse = await fetch(`http://localhost:5041/api/Activid/${idActividad}`);
            if (!actividadResponse.ok) throw new Error("Actividad no encontrada.");
            const actividad = await actividadResponse.json();
            document.getElementById("informacion-lista").innerHTML = `
                <div class="container-incripcion">
                    <div class="header-cont">
                        <h1>${actividad.nombre}</h1>
                    </div>
                    <div class="content">
                        <div class="info-incrip">
                            <div class="description">
                                <h1>Descripción</h1>
                                <p>${actividad.descripcion}</p>
                            </div>
                            <div class="details-incrip">
                                <p><strong>Lugar:</strong> ${actividad.lugar}</p>
                                <p><strong>Fecha y Hora:</strong> <span class="hora">${actividad.hora}</span><span class="fecha">${actividad.fecha}</span></p>
                                <p><strong>Costo: </strong> ${actividad.costo}</p>
                                <p><strong>Requisitos: </strong> ${actividad.requisitos}</p>
                            </div>
                        </div>
                        <div class="image-incrip">
                            <img src="${actividad.imagenUrl}" alt="${actividad.nombre}" class="actividad-imagen"></img>
                        </div>
                    </div>
                </div>
            `;

            // Verificar estado de inscripción del usuario
            if (userSession) {
                const estadoResponse = await fetch(`http://localhost:5041/api/Inscripciones/estado?idActividad=${idActividad}&correo=${userSession.correo}`);
                if (!estadoResponse.ok) throw new Error("No se pudo verificar el estado de inscripción.");
                const { inscrito } = await estadoResponse.json();

                if (inscrito) {
                    agregarBotonCancelar(userSession.correo, idActividad);
                } else {
                    agregarBotonInscribirse();
                }
            }

            document.getElementById("infoActividades").style.display = "block";
        } catch (error) {
            console.error(error);
            document.getElementById("informacion-lista").innerHTML = "<p>No se pudo cargar la información de la actividad.</p>";
        }
    } else {
        console.error("No se proporcionó un idActividad válido.");
    }
});

// Funciones pa los botones
function agregarBotonInscribirse() {
    const inscribirseButton = document.createElement("button");
    inscribirseButton.className = "btn";
    inscribirseButton.textContent = "Inscribirse";
    inscribirseButton.onclick = inscribirse;
    document.querySelector(".image-incrip").appendChild(inscribirseButton);
}

function agregarBotonCancelar(correo, idActividad) {
    const cancelButton = document.createElement("button");
    cancelButton.className = "btn cancelar";
    cancelButton.textContent = "Cancelar Inscripción";
    cancelButton.onclick = () => cancelarInscripcion(idActividad, correo);
    document.querySelector(".image-incrip").appendChild(cancelButton);
}

// Función para inscribirse
async function inscribirse() {
    const userSession = JSON.parse(localStorage.getItem("userSession"));
    const idActividad = new URLSearchParams(window.location.search).get("idActividad");

    if (!userSession) {
        alert("Debes iniciar sesión para inscribirte.");
        window.location.href = "login.html";
        return;
    }

    if (!idActividad) {
        alert("No se proporcionó un ID de actividad válido.");
        return;
    }

    try {
        const response = await fetch("http://localhost:5041/api/Inscripciones/registrar", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ IdActividad: parseInt(idActividad), Correo: userSession.correo }),
        });

        if (response.ok) {
            const data = await response.json();
            alert(data.message);
            document.querySelector(".btn").remove();
            agregarBotonCancelar(userSession.correo, idActividad);
        } else {
            const error = await response.json();
            alert(`Error: ${error.message}`);
        }
    } catch (error) {
        console.error("Error al inscribirse:", error);
        alert("Hubo un problema al intentar inscribirte.");
    }
}

// Función para cancelar inscripción
async function cancelarInscripcion(idActividad, correo) {
    try {
        const response = await fetch("http://localhost:5041/api/Inscripciones/cancelar", {
            method: "DELETE",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ IdActividad: parseInt(idActividad), Correo: correo }),
        });

        if (response.ok) {
            alert("Inscripción cancelada con éxito.");
            // Actualizar la UI
            document.querySelector(".btn.cancelar").remove(); 
            agregarBotonInscribirse();
        } else {
            const error = await response.json();
            alert(`Error: ${error.message}`);
        }
    } catch (error) {
        console.error("Error al cancelar inscripción:", error);
        alert("Hubo un problema al intentar cancelar la inscripción.");
    }
}

