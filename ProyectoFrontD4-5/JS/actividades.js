let actividades = [];

const barrabuscador = document.getElementById("busqueda");
barrabuscador.addEventListener('keyup', (e) => {
    const searchString = e.target.value.toLowerCase(); 
    const filtroActividad = actividades.filter((actividad) => {
        return actividad.nombre.toLowerCase().includes(searchString); 
    });
    mostrarActividades(filtroActividad); 
});

async function Actividades() {
    try {
        const response = await fetch("http://localhost:5041/api/Activid");
        if (!response.ok) {
            throw new Error("No pudo mostrar la información");
        }
        actividades = await response.json();
        mostrarActividades(actividades);
        document.getElementById("actividades").style.display = "block";
    } catch (Error) {
        console.log(Error);
    }
}

function mostrarActividades(actividades) {
    const actividadesContenedor = document.getElementById("actividades-completas");
    actividadesContenedor.innerHTML = actividades.length
        ? actividades.map(crearActividadCompleta).join('')
        : "<p>No hay actividades disponibles.</p>";
}

function crearActividadCompleta(actividad) {
    return ` 
        <div class="actividadesCompleta">
             <div class="imagenContenedorAct"><img src="${actividad.imagenUrl}" alt="${actividad.nombre}" class="actividad-imagen"></img></div>
             <h3>${actividad.nombre}</h3>
             <p>Cupos disponibles: ${actividad.limiteCupos}</p>
              <button type="button" onclick="window.location.href='inscripcion.html?idActividad=${actividad.idActividad}'">Ver detalles</button>
         </div>`;
}
Actividades(); 

