document.addEventListener("DOMContentLoaded", () => {
    console.log("DOM completamente cargado.");
    obtenerRoles();
    const selectedRole = JSON.parse(localStorage.getItem("selectedRole"));
    console.log("Rol seleccionado recuperado:", selectedRole);

    if (selectedRole && selectedRole.idRol) {
        const rolInput = document.getElementById("rol");
        if (rolInput) {
            rolInput.value = selectedRole.idRol;
            console.log("ID del rol asignado al input oculto:", selectedRole.idRol);
        }
    } else {
        console.log("No se ha seleccionado un rol aún.");
    }

    const loginFormulario= document.getElementById("loginFormulario");
    if (loginFormulario) {
        loginFormulario.addEventListener("submit", async (event) => {
            event.preventDefault();
            const correo = document.getElementById("correo").value;
            const contrasena = document.getElementById("contrasena").value;
            const rol = document.getElementById("rol").value;

            console.log("Datos enviados al login:", { correo, contrasena, rol });
            await login(correo, contrasena, rol);
        });
    } else {
        console.error("Formulario con ID 'loginFormulario' no encontrado.");
    }
});

async function obtenerRoles() {
    try {
        console.log("Solicitando roles desde la API...");
        const response = await fetch("http://localhost:5041/api/Roles");
        console.log("Respuesta de la API recibida:", response);

        if (!response.ok) throw new Error("No se pudieron obtener los roles");

        const roles = await response.json();
        console.log("Roles obtenidos de la API:", roles);

        const rolesContenedor = document.getElementById("roles");
        if (!rolesContenedor) {
            console.error("El contenedor con ID 'roles' no existe en el DOM.");
            return;
        }

        rolesContenedor.innerHTML = ""; 
        roles.forEach((role) => {
            const roleElemento = crearElementoRole(role);
            rolesContenedor.appendChild(roleElemento);
        });

        console.log("Roles renderizados en el DOM.");
    } catch (error) {
        console.error("Error al obtener los roles:", error);
        alert("Hubo un problema al cargar los roles. Intenta nuevamente.");
    }
}

function crearElementoRole(role) {
    console.log("Creando elemento para el rol:", role);

    const roleElemento = document.createElement("div");
    roleElemento.classList.add("role");

    let imagenSrc = "";
    if (role.nombreRol === "Estudiantes") {
        imagenSrc = "IMG/Student.png";
    } else if (role.nombreRol === "Administrador") {
        imagenSrc = "IMG/Admin.png";
    } else {
        console.warn(`El rol "${role.nombreRol}" no tiene una imagen asociada.`);
    }

    roleElemento.innerHTML = `
        <img src="${imagenSrc}" alt="${role.nombreRol}">
        <p>${role.nombreRol}</p>
        <button onclick="seleccionarRole('${role.nombreRol}', ${role.idRol})">
            Iniciar sesión como ${role.nombreRol}
        </button>
    `;

    return roleElemento;
}

function seleccionarRole(nombreRole, idRol) {
    console.log("Rol seleccionado:", { nombreRole, idRol });
    localStorage.setItem("selectedRole", JSON.stringify({ nombreRole, idRol }));
    console.log("Rol guardado en localStorage:", { nombreRole, idRol });
    window.location.href = "login.html";
}

async function login(correo, contrasena, rol) {
    try {
        console.log("Intentando iniciar sesión con:", { correo, contrasena, rol });

        const response = await fetch("http://localhost:5041/api/roles/validar-login", {
            method: "POST",
            headers: {
                "Content-Type": "application/json",
            },
            body: JSON.stringify({
                correo: correo,
                contrasena: contrasena,
                rolId: parseInt(rol),
            }),
        });

        const data = await response.json();

        if (!response.ok) {
            console.error("Error al iniciar sesión:", data.message);
            alert(data.message);
        } else {
            console.log("Inicio de sesión exitoso:", data.message);
            alert(data.message);
            localStorage.setItem("userSession", JSON.stringify({ correo, rol, idUsuario: data.idUsuario,nombre: data.nombre}));
            if(parseInt(rol)==2){
                window.location.href = "indexEstudiante.html";    
            }else{
                window.location.href = "indexAdministrativo.html";  
            }
        }
    } catch (error) {
        console.error("Error en la conexión:", error);
        alert("Ocurrió un error al intentar iniciar sesión.");
    }
}

// Botón de cierre de sesión
const logoutButton = document.getElementById("logoutButton");
logoutButton.addEventListener("click", () => {
    console.log("Cerrando sesión...");
    // Eliminar datos del usuario y del rol del almacenamiento local
    localStorage.removeItem("userSession");
    localStorage.removeItem("selectedRole");
    window.location.href = "perfil.html";
});

document.addEventListener("DOMContentLoaded", () => {
    const userSession = JSON.parse(localStorage.getItem("userSession"));
    if (!userSession) {
        alert("Tu sesión ha expirado. Por favor, inicia sesión nuevamente.");
        window.location.href = "perfil.html";
        return;
    }
    window.idUsuario = userSession.idUsuario;
    cargarEstadisticas();
});