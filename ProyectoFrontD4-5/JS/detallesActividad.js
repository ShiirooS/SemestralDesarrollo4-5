
const params = new URLSearchParams(window.location.search);
const idActividad = params.get("idActividad");

const infoActividadDiv = document.getElementById("info-actividad");
const usuariosInscritosUl = document.getElementById("usuarios-inscritos");
async function cargarDetallesActividad() {
    try {
        const response = await fetch(`http://localhost:5041/api/Activid/${idActividad}/detalles-completos`);
        if (!response.ok) throw new Error("Error al cargar los detalles de la actividad.");

        const data = await response.json();
        console.log("Respuesta del servidor:", data);
        if (!data.infoActividad) {
            throw new Error("El objeto infoActividad no está definido en la respuesta.");
        }
        const { Nombre, Imagen, Descripcion, Fecha, Hora, CuposMaximos, CuposOcupados,Costo,Requisitos, Estado } = data.infoActividad;
        infoActividadDiv.innerHTML = `
            <div class="Contenedor-DetallesDeActividad">
                <div class="DetallesAct-Top">
                    <p class="NameAct"><strong>Nombre:</strong> ${Nombre}</p>
                    <img src="${Imagen}" alt="${Nombre}" ">
                </div>
                <div class="Contenedor-DetallesAct-Derc-Izq">

                    <div class="DetallesAct-Derc">
                        <p><strong>Descripción:</strong> ${Descripcion}</p>
                        <p><strong>Fecha:</strong> ${Fecha}</p>
                        <p><strong>Hora:</strong> ${Hora}</p>
                        <p><strong>Cupos Máximos:</strong> ${CuposMaximos}</p>
                    </div>


                    <div class="DetallesAct-Izq">
                        <p><strong>Cupos Ocupados:</strong> ${CuposOcupados}</p>
                        <p><strong>Costo:</strong> ${Costo}</p>
                        <p><strong>Requisitos:</strong> ${Requisitos}</p>
                        <p><strong>Estado:</strong> ${Estado}</p>
                    </div>
                
                </div>
                
                
            
            </div>
            
        `;
        usuariosInscritosUl.innerHTML = data.usuarios.length
            ? data.usuarios.map(usuario => `<li>${usuario.nombre} (${usuario.correo})</li>`).join("")
            : "<li>No hay usuarios inscritos.</li>";
    } catch (error) {
        console.error(error);
        alert(error.message);
    }
}

function editarActividad() {
    window.location.href = `editarActividad.html?idActividad=${idActividad}`;
}
cargarDetallesActividad();
setInterval(cargarDetallesActividad, 50000); 