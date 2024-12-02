async function PropuestasAdmin() {
    try {
        const response = await fetch("http://localhost:5041/api/PropuestasActividades/propuestas-completas");
        if (!response.ok) {
            throw new Error("No se pudo mostrar la información de las propuestas.");
        }
        const propuestas = await response.json();
        mostrarPropuestas(propuestas);
    } catch (error) {
        console.error(error);
    }
}

function mostrarPropuestas(propuestas) {
    const propuestasContenedor = document.getElementById("propuestas-completas-admin");
    propuestasContenedor.innerHTML = propuestas.length
        ? propuestas.map(crearPropuestaCompleta).join('')
        : "<p>No hay propuestas disponibles.</p>";
}

function crearPropuestaCompleta(propuesta) {
    return `
        <div class="propuestasCompleta">
            <h3>${propuesta.nombrePropuesta}</h3>
            <p>Estado: ${propuesta.estado}</p>
            <p>Propuesta por: ${propuesta.nombreUsuario}</p>
            <button type="button" onclick="window.location.href='detallesPropuesta.html?idPropuesta=${propuesta.idPropuesta}'">Detalles</button>
        </div>`;
}

PropuestasAdmin();