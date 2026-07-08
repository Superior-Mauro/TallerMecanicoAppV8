# Mecha Prime - Sistema de Gestión para Taller Mecánico

**Mecha Prime** es una aplicación de escritorio robusta desarrollada en **.NET 8 Windows Forms** y **SQL Server**, diseñada bajo una arquitectura orientada a datos para optimizar los procesos de recepción de vehículos, asignación de órdenes de taller, costeo dinámico de servicios y control de evidencia multimedia en tiempo real.

---

## 🛠️ Arquitectura del Sistema

El sistema implementa un patrón de diseño desacoplado que separa de forma estricta la interfaz de usuario de la persistencia de datos:
*   **Capa de Presentación (UI):** Formularios interactivos (`Form1`, `Form2`, `FrmLogin`, `FrmNuevaCuenta`) con enlace de datos reactivo (`BindingList<T>`) para actualizar las grillas al instante sin parpadeos.
*   **Capa de Persistencia y Negocio (`TallerRepository.cs`):** Encapsula todas las transacciones SQL mediante **ADO.NET (SqlClient)** puro para una ejecución de alta velocidad.
*   **Capa de Inicialización (`TallerDbInitializer.cs`):** Al arrancar la aplicación, valida la conexión con SQL Server y, si la base de datos o las tablas no existen, inyecta los scripts DDL y triggers automáticamente en el motor.

---

## 📸 Evolución del Control Multimedia (Cambio de Ingeniería)

El sistema cuenta con dos flujos de control de imágenes independientes para cumplir con estándares de auditoría profesional:

1.  **Recepción General (`VehiculosImagenes`):** Las fotos tomadas en el `Form1` se asocian de manera global a la **Placa** del vehículo. Sirven como evidencia del estado físico en el que ingresa el auto al taller.
2.  **Evidencia de Taller (`TrabajosImagenes`):** *Migración Crítica de Lógica.* Las fotos tomadas dentro de la gestión de trabajos (`Form2`) se vinculan estrictamente al identificador numérico **`idTrabajo`**. 
    *   **Beneficio:** Si un vehículo regresa múltiples veces al taller (misma placa), la evidencia fotográfica de cada reparación no se mezcla, permitiendo auditorías independientes por cada orden de trabajo efectuada.

---

## 🧮 Lógica de Negocio y Fórmulas de Costeo

El sistema cuenta con un catálogo base (`CatalogoServicios`) que actualiza dinámicamente los totales en pantalla ante cualquier interacción con la UI mediante el método `RecalcularTotales()` en base a la siguiente estructura financiera:

| Servicio | Precio Base (S/) | Tiempo Base | Regla de Bujías Permitida |
| :--- | :--- | :--- | :--- |
| **Mantenimiento Regular** | S/ 150.00 | 2h 00m | Sí |
| **Mantenimiento Completo** | S/ 450.00 | 3h 40m | Sí |
| **Afinamiento** | S/ 280.00 | 2h 30m | Sí |
| **Otros (Personalizado)** | *Dinámico* | *Dinámico* | No |

### 💲 Fórmulas Matemáticas de Costeo

El total final a pagar ($Total_{Final}$) y el tiempo estimado de entrega ($Tiempo_{Total}$) se calculan en caliente utilizando variables acumulativas binarias según los adicionales seleccionados por el operario:

$$Total_{Final} = Precio_{Base} + (Refri \times 90) + (Frenos \times 50) + (Bujias \times 40)$$

$$Tiempo_{Total} = Tiempo_{Base} + (Refri \times 40\text{ min}) + (Frenos \times 30\text{ min}) + (Bujias \times 10\text{ min})$$

*Donde:*
*   $Refri$, $Frenos$, $Bujias$ $\in \{0, 1\}$ (representando el estado del `CheckBox`).
*   *Restricción:* El adicional de Bujías solo sumará si el servicio seleccionado es Regular, Completo o Afinamiento.

---

## 🗄️ Modelo del Schema de Base de Datos (SQL)

A continuación se detalla la estructura física de las tablas del sistema:

### 1. `dbo.Usuarios` (Credenciales de Acceso)
*   `Id` (INT, PK, IDENTITY): Identificador único de usuario.
*   `Nombres` (NVARCHAR(150)): Nombre completo del personal.
*   `NombreUsuario` (NVARCHAR(50), UNIQUE): Identificador de login.
*   `Contrasena` (NVARCHAR(100)): Contraseña de acceso en texto plano.
*   `FechaRegistro` (DATETIME2): Seteado por defecto con `SYSUTCDATETIME()`.

### 2. `dbo.Vehiculos` (Entidades Físicas)
*   `idVehiculo` (INT, PK, IDENTITY): Identificador único interno.
*   `Placa` (NVARCHAR(15), UNIQUE): Matrícula vehicular única.
*   `Cliente` (NVARCHAR(120)): Nombre del propietario.
*   `Telefono` (NVARCHAR(20)): Teléfono de contacto.
*   `Modelo` (NVARCHAR(80)): Marca y modelo del vehículo.
*   `Dni` (NVARCHAR(15)): Documento de identidad del cliente.
*   `Estado` (NVARCHAR(50)): Estado global del vehículo (actualizado dinámicamente).
*   `FechaRegistro` (DATETIME2): Fecha y hora de ingreso al sistema.

### 3. `dbo.Trabajos` (Órdenes de Servicio)
*   `idTrabajo` (INT, PK, IDENTITY): Número de orden autoincremental (Formato `D2`).
*   `idVehiculo` (INT, FK -> `Vehiculos.idVehiculo` ON DELETE CASCADE).
*   `Placa` (NVARCHAR(15)): Copia de control vehicular.
*   `Mecanico` (NVARCHAR(120)): Nombre del mecánico asignado (`CatalogoMecanicos`).
*   `Descripcion` (NVARCHAR(500)): Tareas específicas a realizar.
*   `Estado` (NVARCHAR(30)): `[Pendiente, En proceso, Finalizado]`.
*   `ServicioNombre` (NVARCHAR(80)): Nombre del servicio principal aplicado.
*   `PrecioBase` / `TotalPagar` (DECIMAL(10,2)): Registros financieros de la transacción.
*   `TiempoBaseMinutos` / `TiempoEstimadoMinutos` (INT): Duración del trabajo expresada en minutos planos para cálculos matemáticos precisos en código.
*   `CambioRefrigerante` / `CambioLiquidoFrenos` / `CambioBujias` (BIT): Estados lógicos de adicionales.

### 4. `dbo.TrabajosImagenes` (Galería Relacional de Evidencia)
*   `idImagen` (INT, PK, IDENTITY): Identificador único de archivo.
*   `idTrabajo` (INT, FK -> `Trabajos.idTrabajo` ON DELETE CASCADE): Vinculación directa al proceso.
*   `datosImagen` (VARBINARY(MAX)): Almacenamiento binario binario (`byte[]`) puro de la fotografía cargada.

### 5. `dbo.HistorialServiciosFinalizados` (Tabla de Auditoría Pasiva)
Tabla espejo que funciona de forma automatizada mediante un disparador para guardar el histórico inalterable de los servicios completados con éxito en el taller.

---

## 📂 Estructura de la Solución (Arquitectura limpia)

El proyecto está organizado de forma modular para garantizar un mantenimiento eficiente y una clara separación de responsabilidades:

* **`Data/`**: Contiene la infraestructura de persistencia.
    * `DatabaseConnection.cs`: Gestiona la lectura segura de la cadena de conexión desde el archivo de configuración.
    * `TallerDbInitializer.cs`: Inicializa la base de datos y la estructura de tablas.
    * `TallerRepository.cs`: Centraliza todas las consultas transaccionales (`INSERT`, `SELECT`, `UPDATE`, `DELETE`) usando ADO.NET puro.
* **`Helpers/`**: Funciones utilitarias del sistema.
    * `PlacaValidator.cs`: Lógica de negocio encargada de validar y normalizar la sintaxis de las matrículas vehiculares.
* **`Models/`**: Entidades del dominio del taller.
    * `Vehiculo.cs` / `Trabajo.cs` / `Mecanico.cs` / `Cliente.cs` / `Servicio.cs`
    * `CatalogoServicios.cs` / `CatalogoMecanicos.cs`: Enumeradores y repositorios en memoria para la parametrización de la UI.
* **Formularios Principales (UI)**:
    * `FrmLogin.cs`: Control de acceso y seguridad perimetral.
    * `FrmNuevaCuenta.cs`: Módulo para el registro de nuevos operadores.
    * `Form1.cs`: Módulo de Recepción (Registro, actualización y búsqueda de vehículos).
    * `Form2.cs`: Módulo de Taller (Gestión de órdenes de trabajo, control multimedia y costeo).

---

## 🚀 Requisitos e Instalación

1.  **Entorno:** Contar con el SDK de .NET 8 o superior instalado.
2.  **Base de Datos:** Instancia activa de SQL Server (Express, LocalDB o Enterprise).
3.  **Configuración de Cadena:** Modificar el archivo `appsettings.json` ubicado en la raíz del proyecto para direccionar el motor local:
    ```json
    {
      "ConnectionStrings": {
        "TallerMecanico": "Server=TU_SERVIDOR_SQL;Database=TallerMecanicoDb;Trusted_Connection=True;TrustServerCertificate=True;"
      }
    }
    ```
4.  **Despliegue de Esquemas:** Al ejecutar la solución por primera vez, el sistema autodetectará la ausencia de las entidades y creará automáticamente todo el modelo físico relacional en el servidor SQL de forma transparente.
