async function cargarEstadisticas() {
    try {
        const userSession = JSON.parse(localStorage.getItem("userSession"));
        if (!userSession || !userSession.idUsuario) {
            throw new Error("Usuario no autenticado.");
        }

        const response = await fetch(`http://localhost:5041/api/Estadisticas/estadisticas`, {
            headers: { "idUsuario": userSession.idUsuario },
        });

        if (!response.ok) {
            throw new Error("No se pudieron cargar las estadísticas.");
        }

        const estadisticas = await response.json();
        mostrarEstadisticas(estadisticas);
        document.querySelector(".estadisticas").style.display = "block";
    } catch (error) {
        console.error("Error:", error);
    }
}

function mostrarEstadisticas(data) {
    const contenedoresEstadisticas = document.querySelectorAll(".item .valor");

    const valores = [
        data.actividadesInscritas ?? 0,
        data.actividadesOrganizadas ?? 0,
        data.actividadesEnEspera ?? 0,
        data.actividadesAceptadas ?? 0,
        data.actividadesRechazadas ?? 0,
    ];
    contenedoresEstadisticas.forEach((element, index) => {
        element.textContent = valores[index];
    });
}