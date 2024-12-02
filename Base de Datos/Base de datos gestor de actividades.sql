CREATE DATABASE gestorActividades;
use gestorActividades;

CREATE TABLE categorias (
    id_categoria INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

CREATE TABLE subcategorias (
    id_subcategoria INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    id_categoria INT,
    FOREIGN KEY (id_categoria) REFERENCES categorias(id_categoria)
    
);

CREATE TABLE roles (
    id_rol INT AUTO_INCREMENT PRIMARY KEY,
    nombre_rol VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE usuarios (
    id_usuario INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    correo VARCHAR(100) UNIQUE NOT NULL,
    contrasena VARCHAR(255)default '123' NOT NULL,
    facultad VARCHAR(100),
    id_rol INT,
    FOREIGN KEY (id_rol) REFERENCES roles(id_rol)
);
CREATE TABLE tipo_estado (
    id_tipo_estado INT AUTO_INCREMENT PRIMARY KEY,
    nombre_tipo VARCHAR(50) UNIQUE NOT NULL
);

CREATE TABLE estado_actividades (
    id_estado INT AUTO_INCREMENT PRIMARY KEY,
    nombre_estado VARCHAR(50) UNIQUE NOT NULL,
    id_tipo_estado INT,
    FOREIGN KEY (id_tipo_estado) REFERENCES tipo_estado(id_tipo_estado)
);


CREATE TABLE actividades (
    id_actividad INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    descripcion TEXT,
    id_subcategoria INT,
    lugar VARCHAR(100),
    fecha DATE,
    hora TIME,
    limite_cupos INT,
    costo DECIMAL(10, 2),
    requisitos TEXT,
    imagen_url VARCHAR(300),
    id_estado INT DEFAULT 1,
    FOREIGN KEY (id_subcategoria) REFERENCES subcategorias(id_subcategoria),
    FOREIGN KEY (id_estado) REFERENCES estado_actividades(id_estado)
);
select * from actividades;

CREATE TABLE inscripciones (
    id_inscripcion INT AUTO_INCREMENT PRIMARY KEY,
    id_usuario INT,
    id_actividad INT,
    fecha_inscripcion DATETIME DEFAULT CURRENT_TIMESTAMP,
    id_estado INT DEFAULT 1,
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_actividad) REFERENCES actividades(id_actividad),
    FOREIGN KEY (id_estado) REFERENCES estado_actividades(id_estado)
);


CREATE TABLE propuestas_actividades (
    id_propuesta INT AUTO_INCREMENT PRIMARY KEY,
    id_usuario INT,
    nombre VARCHAR(100),
    descripcion TEXT,
    id_subcategoria INT,
    lugar VARCHAR(100),
    fecha DATE,
    hora TIME,
    limite_cupos INT,
    costo DECIMAL(10, 2),
    requisitos TEXT,
    imagen_url VARCHAR(300),
    id_estado INT DEFAULT 9,
    comentario_rechazo TEXT null,
    FOREIGN KEY (id_usuario) REFERENCES usuarios(id_usuario),
    FOREIGN KEY (id_subcategoria) REFERENCES subcategorias(id_subcategoria),
    FOREIGN KEY (id_estado) REFERENCES estado_actividades(id_estado)
);


INSERT INTO categorias (nombre)
VALUES
('Cultura y Arte'),
('Deportes'),
('Ciencia y Tecnología'),
('Jornadas Académicas');

INSERT INTO subcategorias (nombre, id_categoria)
VALUES
('Artesanía', 1), 
('Música', 1),
('Danza', 1),
('Literatura', 1),
('Pintura', 1),
('Teatro', 1),
('Baloncesto', 2),
('Fútbol', 2),
('Esgrima', 2),
('Atletismo', 2),
('Beisbol', 2),
('Ajedrez', 2),
('Robótica', 3), 
('Desarrollo de Software', 3), 
('Innovación Tecnológica', 3), 
('Inteligencia Artificial', 3),
('Talleres', 4),
('Presentaciones', 4);

INSERT INTO tipo_estado (nombre_tipo)
VALUES
('Actividad'),
('Propuesta');

INSERT INTO estado_actividades (nombre_estado, id_tipo_estado)
VALUES
('Activo', 1), 
('Finalizada', 1), 
('Cancelada', 1),  
('Pospuesta', 1),
('En revisión', 2), 
('Aprobada',2 ), 
('Rechazada', 2);  


INSERT INTO roles (nombre_rol)
VALUES
('Administrador'),
('Estudiantes');

-- Insertar algunos usuarios
INSERT INTO usuarios (nombre, correo, facultad, id_rol)
VALUES
('Carlos Martínez', 'carlos.martinez@example.com', 'Facultad de Ingeniería en Sistemas computacionales', 2),
('Ana Gómez', 'ana.gomez@example.com', 'Facultad de Ciencias Sociales', 2),
('Luis Pérez', 'luis.perez@example.com', 'Facultad de Ciencias', 1);

-- Insertar algunas actividades sin especificar el estado, ya que el valor por defecto es 1
INSERT INTO actividades (nombre, descripcion, id_subcategoria, lugar, fecha, hora, limite_cupos, costo, requisitos, imagen_url)
VALUES
('Conferencia sobre Inteligencia Artificial', 'Una conferencia sobre los avances en IA.', 15, 'Auditorio Central', '2024-11-30', '10:00:00', 100, 20.00, 'Ninguno', 'https://raw.githubusercontent.com/ShiirooS/SemestralDesarrollo4-5/refs/heads/main/ImagenesBD/ai.jpg'),
('Taller de Robótica', 'Taller práctico sobre la construcción de robots.', 13, 'Laboratorio de Robótica', '2024-11-27', '14:00:00', 30, 15.00, 'Conocimientos básicos de electrónica', 'https://raw.githubusercontent.com/ShiirooS/SemestralDesarrollo4-5/refs/heads/main/ImagenesBD/Taller-de-Robotica-1.jpg'),
('Torneo de Fútbol', 'Torneo de fútbol entre facultades.', 8, 'Cancha de la universidad', '2024-12-05', '08:00:00', 50, 10.00, 'Tener equipo propio', 'https://raw.githubusercontent.com/ShiirooS/SemestralDesarrollo4-5/refs/heads/main/ImagenesBD/IMG_9471-scaled.jpg');

-- Insertar algunas inscripciones

INSERT INTO actividades (nombre, descripcion, id_subcategoria, lugar, fecha, hora, limite_cupos, costo, requisitos, imagen_url)
VALUES
('Taller de Cerámica', 'Taller práctico para crear sus propias piezas de cerámica como actividad recreativa.', 1, 'Centro Cultural Universitario', '2024-11-29', '10:00:00', 25, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/ceramica.jpg'),
('Taller de Tejido Creativo', 'Taller de tejido en el que se aprenderán técnicas de tejido y bordado.', 1, 'Sala de Manualidades', '2024-11-29', '15:00:00', 20, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/tejido.avif'),
('Concurso de Artesanía Estudiantil', 'Concurso de manualidades donde los estudiantes y docentes podrán mostrar su creatividad utilizando materiales reciclables.', 1, 'Plaza Universitaria', '2024-12-25', '09:00:00', 30, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/concursOaRTESANIA.jpg'),
('Taller de Pintura en Madera', 'Taller de pintura para decorar muebles pequeños, una actividad para relajarse y liberar creatividad.', 1, 'Taller de Artes', '2024-11-30', '13:00:00', 15, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/pinturaMadera.jpg'),
('Taller de Escultura en Arcilla', 'Actividad recreativa en la que podrán crear esculturas en arcilla.', 1, 'Centro de Arte', '2024-12-10', '11:00:00', 25, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/arcilla.jpg'),
('Feria de Artesanía', 'Feria donde los estudiantes pueden vender o intercambiar productos artesanales creados por ellos.', 1, 'Pasillo Central', '2024-12-12', '09:00:00', 50, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/feriaArtesania.jpg'),
('Taller de Decoración Navideña', 'Taller de manualidades para crear decoraciones navideñas utilizando materiales reciclados.', 1, 'Sala de Eventos', '2024-12-15', '14:00:00', 30, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/navidad%7D.webp'),
('Taller de Cartonería', 'Taller donde aprenderán la técnica de cartonería para hacer figuras de papel maché.', 1, 'Taller de Manualidades', '2024-12-18', '13:00:00', 20, 0.00, 'Ninguno', 'https://raw.githubusercontent.com/Annie50P/PD4-5/refs/heads/main/ImagenesBD/cartoneria.jpeg');

DELIMITER //
CREATE PROCEDURE GetEstadisticasUsuario(IN idUsuario INT)
BEGIN
    SELECT 
        u.id_usuario,
        u.nombre AS nombre_usuario,
        -- Contar actividades inscritas
        (SELECT COUNT(DISTINCT i.id_inscripcion)
         FROM inscripciones i
         WHERE i.id_usuario = u.id_usuario) AS actividades_inscritas,
        -- Contar propuestas de actividades organizadas
        (SELECT COUNT(DISTINCT pa.id_propuesta)
         FROM propuestas_actividades pa
         WHERE pa.id_usuario = u.id_usuario) AS actividades_organizadas,
        -- Contar propuestas de actividades en espera
        (SELECT COUNT(*)
         FROM propuestas_actividades pa
         WHERE pa.id_usuario = u.id_usuario AND pa.id_estado = (SELECT id_estado FROM estado_actividades WHERE nombre_estado = 'En revisión')) AS actividades_en_espera,
        -- Contar propuestas de actividades aceptadas
        (SELECT COUNT(*)
         FROM propuestas_actividades pa
         WHERE pa.id_usuario = u.id_usuario AND pa.id_estado = (SELECT id_estado FROM estado_actividades WHERE nombre_estado = 'Aprobada')) AS actividades_aceptadas,
        -- Contar propuestas de actividades rechazadas
        (SELECT COUNT(*)
         FROM propuestas_actividades pa
         WHERE pa.id_usuario = u.id_usuario AND pa.id_estado = (SELECT id_estado FROM estado_actividades WHERE nombre_estado = 'Rechazada')) AS actividades_rechazadas
    FROM usuarios u
    WHERE u.id_usuario = idUsuario;
END //
DELIMITER ;


DELIMITER //
CREATE PROCEDURE GetActividadesCompletas()
BEGIN
    SELECT 
        a.id_actividad AS Id,
        a.nombre AS nombre_actividad,
        a.fecha,
        ea.nombre_estado AS estado,
        a.limite_cupos AS cupos_maximos,
        COUNT(i.id_inscripcion) AS cupos_ocupados
    FROM 
        actividades a
    LEFT JOIN 
        estado_actividades ea ON a.id_estado = ea.id_estado
    LEFT JOIN 
        inscripciones i ON a.id_actividad = i.id_actividad
    GROUP BY 
        a.id_actividad, a.nombre, a.fecha, ea.nombre_estado, a.limite_cupos;
END //
DELIMITER ;
call GetActividadesCompletas();


DELIMITER //
CREATE PROCEDURE GetActividadDetallesCompletos(IN idActividad INT)
BEGIN
    SELECT 
        a.id_actividad,
        a.imagen_url,
        a.nombre,
        a.descripcion,
        a.fecha,
        a.hora,
        a.limite_cupos,
        a.costo,
        a.requisitos,
        ea.nombre_estado AS estado,
        (SELECT COUNT(*) FROM inscripciones WHERE id_actividad = a.id_actividad) AS cupos_ocupados,
        u.nombre AS nombre_usuario,
        u.correo AS correo_usuario
    FROM 
        actividades a
    LEFT JOIN 
        estado_actividades ea ON a.id_estado = ea.id_estado
    LEFT JOIN 
        inscripciones i ON a.id_actividad = i.id_actividad
    LEFT JOIN 
        usuarios u ON i.id_usuario = u.id_usuario
    WHERE 
        a.id_actividad = idActividad;
END //
DELIMITER ;
call  GetActividadDetallesCompletos(2);


DELIMITER //
CREATE PROCEDURE ObtenerEstadosActividades()
BEGIN
    SELECT id_estado, nombre_estado
    FROM estado_actividades
    WHERE id_tipo_estado = 1; 
END //
DELIMITER ;
call ObtenerEstadosActividades();

select * from actividades;


SET GLOBAL event_scheduler = ON;

DELIMITER //
CREATE EVENT CambiarEstadoFinalizado
ON SCHEDULE EVERY 5 SECOND
DO
BEGIN
    UPDATE actividades
    SET id_estado = (
        SELECT id_estado FROM estado_actividades WHERE nombre_estado = 'Finalizada' LIMIT 1
    )
    WHERE CONCAT(fecha, ' ', hora) <= NOW() AND id_estado = (
        SELECT id_estado FROM estado_actividades WHERE nombre_estado = 'Activo' LIMIT 1
    );
END //
DELIMITER ;

-- Funcional
DELIMITER //
CREATE PROCEDURE EstadosInscripciones(IN IdUsuario INT, IN IdActividad INT)
BEGIN
    -- Cambiar inscripciones a "Activas" si la actividad está vigente (fecha no ha pasado)
    UPDATE inscripciones AS i
    JOIN actividades AS a ON i.id_actividad = a.id_actividad
    SET i.id_estado = 1 -- Activo
    WHERE i.id_estado = 5 
      AND a.fecha >= NOW();

    -- Cambiar inscripciones a "Finalizadas" si la actividad ya pasó
    UPDATE inscripciones AS i
    JOIN actividades AS a ON i.id_actividad = a.id_actividad
    SET i.id_estado = 2 -- Finalizada
    WHERE i.id_estado = 5 
      AND a.fecha < NOW();

    -- Actualizar inscripciones específicas a "Cancelada" usando variables de entrada
    UPDATE inscripciones
    SET id_estado = 3 -- Cancelada
    WHERE id_usuario = IdUsuario 
      AND id_actividad = IdActividad
      AND id_estado = 1; -- Solo si la inscripción está activa

END //
DELIMITER ;

DROP PROCEDURE EstadoInscripcionesUpdate
DELIMITER //
CREATE PROCEDURE EstadoInscripcionesUpdate(IN IdUsuario INT, IN IdActividad INT)
BEGIN
    -- Cambiar inscripciones a "Activas" si la actividad está vigente (fecha no ha pasado)
    UPDATE inscripciones AS i
    JOIN actividades AS a ON i.id_actividad = a.id_actividad
    SET i.id_estado = 1 -- Activo
    WHERE i.id_estado = 5 -- Inscripción pendiente o provisional
      AND a.fecha >= NOW();

    -- Cambiar inscripciones a "Finalizadas" si la actividad ya pasó
    UPDATE inscripciones AS i
    JOIN actividades AS a ON i.id_actividad = a.id_actividad
    SET i.id_estado = 2 -- Finalizada
    WHERE i.id_estado = 5 -- Inscripción pendiente o provisional
      AND a.fecha < NOW();

    -- Actualizar inscripciones específicas a "Cancelada" usando variables de entrada
    UPDATE inscripciones
    SET id_estado = 3 -- Cancelada
    WHERE id_usuario = IdUsuario AND id_actividad = IdActividad;
END //
DELIMITER ;
-- Funcionales las de incripciones

-- prueba 30000 funcional si se maneja por estado (eso no queremos) pero en filtro no sale nada :(
DROP PROCEDURE EstadosActualizadosPro
DELIMITER //
CREATE PROCEDURE EstadosActualizadosPro(IN IdUsuario INT, IN IdActividad INT, IN IdPropuesta INT)
BEGIN
    -- Cambiar propuestas aceptadas a "Activo" si la fecha es futura
    UPDATE propuestas_actividades
    SET id_estado = 1 -- Activo
    WHERE id_estado = 3 -- Aceptado
      AND fecha >= NOW();

    -- Cambiar propuestas aceptadas a "Finalizada" si la fecha ya pasó
    UPDATE propuestas_actividades
    SET id_estado = 2 -- Finalizada
    WHERE id_estado = 3 -- Aceptado
      AND fecha < NOW();

    -- Cambiar propuestas rechazadas a "No hace nada" (mantienen su estado)
    UPDATE propuestas_actividades
    SET id_estado = id_estado -- Sin cambios
    WHERE id_propuesta = IdPropuesta
      AND id_estado = 4; -- Rechazado

    -- Las propuestas "En revisión" no se actualizan automáticamente.
END //
DELIMITER ;


SELECT*FROM propuestas_actividades;
SELECT*FROM inscripciones;
SELECT*FROM actividades;

-- OBTENER PROPUESTAS COMPLETAASSS
DELIMITER //
CREATE PROCEDURE GetPropuestasCompletas()
BEGIN
    SELECT 
        pa.id_propuesta AS IdPropuesta,
        pa.nombre AS NombrePropuesta,
        ea.nombre_estado AS Estado,
        u.nombre AS NombreUsuario
    FROM 
        propuestas_actividades pa
    LEFT JOIN 
        estado_actividades ea ON pa.id_estado = ea.id_estado
    LEFT JOIN 
        usuarios u ON pa.id_usuario = u.id_usuario;
END //
DELIMITER ;

call  GetPropuestasCompletas();

SELECT * FROM estado_actividades;

-- -------------------------------OBTENER SUPER DETALLES PROPUESTAS COMPLETAASSS-----------------
DELIMITER //
CREATE PROCEDURE ObtenerPropuestaDetallesCompletos(IN idPropuesta INT)
BEGIN
    SELECT 
        pa.id_propuesta AS id_propuesta,
        pa.imagen_url AS imagen,
        pa.nombre AS nombre,
        pa.descripcion AS descripcion,
        pa.fecha AS fecha,
        pa.hora AS hora,
        pa.limite_cupos AS limite_cupos,
        pa.lugar AS lugar,
        pa.costo AS costo,
        pa.requisitos AS requisitos,
        ea.nombre_estado AS estado,
        u.nombre AS nombre_usuario,
        u.correo AS correo_usuario
    FROM 
        propuestas_actividades pa
    LEFT JOIN 
        estado_actividades ea ON pa.id_estado = ea.id_estado
    LEFT JOIN 
        usuarios u ON pa.id_usuario = u.id_usuario
    WHERE 
        pa.id_propuesta = idPropuesta;
END //
DELIMITER ;



SELECT*FROM propuestas_actividades;
SELECT*FROM inscripciones;
SELECT*FROM actividades;