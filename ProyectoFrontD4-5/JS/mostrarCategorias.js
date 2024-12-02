let categoriaSeleccionada = null;
let activiadadSeleccionada= null;


async function obtenerCategorias() {
    try {
        const response = await fetch("http://localhost:5041/api/Categorias");
        if (!response.ok) throw new Error("No se pudo obtener las categorías");

        const categorias = await response.json();
        const categoriasContenedor = document.getElementById("categorias");
        categoriasContenedor.innerHTML = '';

        categorias.forEach(categoria => {
            const categoriaElemento = crearElementoCategoria(categoria);
            categoriasContenedor.appendChild(categoriaElemento);
        });
    } catch (error) {
        console.error(error);
    }
}

function crearElementoCategoria(categoria) {
    const categoriaElemento = document.createElement("div");
    categoriaElemento.classList.add("categoria");
    categoriaElemento.innerHTML = `<p>${categoria.nombre}</p>`;
    categoriaElemento.onclick = (event) => {
        event.stopPropagation();
        verSubcategorias(categoria.idCategoria);
    };
    return categoriaElemento;
}

// ------------------------------------MUESTRA LAS SUBCATEGORIASSS WUJUUU----------------------------------------------------
async function verSubcategorias(idCategoria) {
    const subcategoriasContenedor = document.getElementById("subcategorias-lista");
    if (categoriaSeleccionada === idCategoria) {
        subcategoriasContenedor.innerHTML = '';
        document.getElementById("subcategorias").style.display = "none";
        categoriaSeleccionada = null;
        return;
    }

    try {
        const response = await fetch(`http://localhost:5041/api/Categorias/${idCategoria}/subcategorias`);
        if (!response.ok) throw new Error(`No se encontraron subcategorías para la categoría con ID ${idCategoria}`);

        const subcategorias = await response.json();
        subcategoriasContenedor.innerHTML = subcategorias.length
            ? subcategorias.map(crearElementoSubcategoria).join('')
            : "<p>No hay subcategorías disponibles.</p>";

        document.getElementById("subcategorias").style.display = "block";
        categoriaSeleccionada = idCategoria;
    } catch (error) {
        console.error(error);
    }
}

function crearElementoSubcategoria(subcategoria) {
    return `
        <div class="subcategoria" onclick="irASubcategoria(${subcategoria.idSubcategoria})">
            <img src="/IMG/subcategorias/${subcategoria.nombre.toLowerCase().replace(/\s/g, '-')}.jpg" alt="${subcategoria.nombre}">
            <p>${subcategoria.nombre}</p>
        </div>`;
}

document.addEventListener("click", (event) => {
    const subcategoriasContenedor = document.getElementById("subcategorias");
    if (categoriaSeleccionada && !subcategoriasContenedor.contains(event.target)) {
        subcategoriasContenedor.style.display = "none";
        categoriaSeleccionada = null;
    }
});

function obtenerParametroURL(nombre) {
    return new URLSearchParams(window.location.search).get(nombre);
}

const idSubcategoria = obtenerParametroURL("idSubcategoria");
if (idSubcategoria) mostrarActividadesPorSubcategoria(idSubcategoria);

// ---------------------------------------MUESTRA LAS ACTIVIDADES WUJUUUUUUUUU--------------------------------------------------
async function mostrarActividadesPorSubcategoria(idSubcategoria) {
    try {
        const response = await fetch(`http://localhost:5041/api/Activid/subcategoria/${idSubcategoria}`);
        if (!response.ok) throw new Error(`No se encontraron actividades para la subcategoría con ID ${idSubcategoria}`);

        const actividades = await response.json();
        const actividadesContenedor = document.getElementById("actividades-lista");
        actividadesContenedor.innerHTML = actividades.length
            ? actividades.map(crearElementoActividad).join('')
            : "<p>No hay actividades disponibles.</p>";

        document.getElementById("actividades").style.display = "block";
    } catch (error) {
        console.error(error);
    }
}

function crearElementoActividad(actividad) {
    return `
        <div class="actividad">
            <div class="imagenContenedorAct"><img src="${actividad.imagenUrl}" alt="${actividad.nombre}" class="actividad-imagen"></div>
            <h3>${actividad.nombre}</h3>
            <p>Cupos disponibles: ${actividad.limiteCupos}</p>
            <button type="button" onclick="window.location.href='inscripcion.html?idActividad=${actividad.idActividad}'">Ver detalles</button>
        </div>`;
}


function irASubcategoria(idSubcategoria) {
    window.location.href = `actividadesPorSubcategoria.html?idSubcategoria=${idSubcategoria}`;
    mostrarActividadesPorSubcategoria(idSubcategoria);
}


// ----------------------------------------------------------------------------------


// --------------------------------------------------------

document.getElementById("scroll-left").addEventListener("click", () => {
    document.getElementById("subcategorias-lista").scrollBy({ left: -200, behavior: 'smooth' });
});

document.getElementById("scroll-right").addEventListener("click", () => {
    document.getElementById("subcategorias-lista").scrollBy({ left: 200, behavior: 'smooth' });
});

obtenerCategorias();
