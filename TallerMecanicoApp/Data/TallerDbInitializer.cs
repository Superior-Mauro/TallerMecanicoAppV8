using Microsoft.Data.SqlClient;

namespace TallerMecanicoApp.Data;

public static class TallerDbInitializer
{
    public static void Inicializar()
    {
        var cadenaCompleta = DatabaseConnection.Obtener();
        var builder = new SqlConnectionStringBuilder(cadenaCompleta);
        var nombreBase = builder.InitialCatalog;

        if (string.IsNullOrWhiteSpace(nombreBase))
        {
            throw new InvalidOperationException(
                "La cadena de conexión debe incluir Database=TallerMecanicoDb (o el nombre de tu base).");
        }

        builder.InitialCatalog = "master";
        CrearBaseSiNoExiste(builder.ConnectionString, nombreBase);

        using var conexion = new SqlConnection(cadenaCompleta);
        conexion.Open();

        // Ejecución de scripts automáticos
        EjecutarScript(conexion, ScriptUsuarios); // <-- SE AGREGA LA TABLA DE LOGINS AQUÍ
        EjecutarScript(conexion, ScriptVehiculos);
        EjecutarScript(conexion, ScriptTrabajos);
        EjecutarScript(conexion, ScriptColumnasAdicionales);
        EjecutarScript(conexion, ScriptTablaHistorial);
        EjecutarScript(conexion, ScriptTriggerFinalizados);
        EjecutarScript(conexion, ScriptTablaImagenes);

    }

    private static void CrearBaseSiNoExiste(string cadenaMaster, string nombreBase)
    {
        using var conexion = new SqlConnection(cadenaMaster);
        conexion.Open();

        var nombreSeguro = nombreBase.Replace("]", "]]");
        using var comando = conexion.CreateCommand();
        comando.CommandText = $"""
            IF DB_ID(N'{nombreBase.Replace("'", "''")}') IS NULL
            BEGIN
                CREATE DATABASE [{nombreSeguro}];
            END
            """;
        comando.ExecuteNonQuery();
    }

    private static void EjecutarScript(SqlConnection conexion, string script)
    {
        using var comando = conexion.CreateCommand();
        comando.CommandText = script;
        comando.ExecuteNonQuery();
    }

    // ==========================================
    // SCRIPT NUEVO: TABLA DE USUARIOS
    // ==========================================
    private const string ScriptUsuarios =
        """
        IF OBJECT_ID(N'dbo.Usuarios', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Usuarios
            (
                Id            INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
                Nombres       NVARCHAR(150)      NOT NULL,
                NombreUsuario NVARCHAR(50)       NOT NULL,
                Contrasena    NVARCHAR(100)      NOT NULL,
                FechaRegistro DATETIME2(0)       NOT NULL CONSTRAINT DF_Usuarios_FechaRegistro DEFAULT (SYSUTCDATETIME()),
                CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE (NombreUsuario)
            );
        END
        """;

    // ==========================================
    // SCRIPTS EXISTENTES
    // ==========================================
    private const string ScriptVehiculos =
            """
        IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Vehiculos
            (
                idVehiculo    INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Vehiculos PRIMARY KEY,
                Placa         NVARCHAR(15)       NOT NULL,
                Cliente       NVARCHAR(120)      NOT NULL,
                Telefono      NVARCHAR(20)       NOT NULL CONSTRAINT DF_Vehiculos_Telefono DEFAULT (N''),
                Modelo        NVARCHAR(80)       NOT NULL,
                Dni           NVARCHAR(15)       NOT NULL CONSTRAINT DF_Vehiculos_Dni DEFAULT (N''),
                Estado NVARCHAR(50) DEFAULT 'Pendiente',
                FechaRegistro DATETIME2(0)       NOT NULL CONSTRAINT DF_Vehiculos_FechaRegistro DEFAULT (SYSUTCDATETIME()),
                CONSTRAINT UQ_Vehiculos_Placa UNIQUE (Placa)
            );
        END
        """;

    private const string ScriptImagenesVehiculos =
        """
        IF OBJECT_ID(N'dbo.ImagenesVehiculos', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.ImagenesVehiculos (
                Id INT IDENTITY(1,1) PRIMARY KEY,
                Placa VARCHAR(15) FOREIGN KEY REFERENCES dbo.Vehiculos(Placa) ON DELETE CASCADE,
                Foto VARBINARY(MAX) NOT NULL,
                FechaCarga DATETIME DEFAULT GETDATE()
            );
        END
        """;

    private const string ScriptTrabajos =
            """
        IF OBJECT_ID(N'dbo.Trabajos', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Trabajos
            (
                idTrabajo             INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_Trabajos PRIMARY KEY,
                idVehiculo            INT                NOT NULL,
                Placa                 NVARCHAR(15)       NOT NULL,
                Mecanico              NVARCHAR(120)      NOT NULL,
                Descripcion           NVARCHAR(500)      NOT NULL,
                Estado                NVARCHAR(30)       NOT NULL,
                ServicioNombre        NVARCHAR(80)       NOT NULL,
                PrecioBase            DECIMAL(10, 2)     NOT NULL,
                TiempoBaseMinutos     INT                NOT NULL,
                CambioRefrigerante    BIT                NOT NULL,
                CambioLiquidoFrenos   BIT                NOT NULL,
                CambioBujias          BIT                NOT NULL,
                TotalPagar            DECIMAL(10, 2)     NOT NULL,
                TiempoEstimadoMinutos INT                NOT NULL,
                FechaRegistro         DATETIME2(0)       NOT NULL CONSTRAINT DF_Trabajos_FechaRegistro DEFAULT (SYSDATETIME()),
                
                CONSTRAINT FK_Trabajos_Vehiculos FOREIGN KEY (idVehiculo) REFERENCES dbo.Vehiculos (idVehiculo) ON DELETE CASCADE            );
        END
        """;

    private const string ScriptColumnasAdicionales =
        """
        IF OBJECT_ID(N'dbo.Vehiculos', N'U') IS NOT NULL
           AND COL_LENGTH(N'dbo.Vehiculos', N'Telefono') IS NULL
        BEGIN
            ALTER TABLE dbo.Vehiculos
            ADD Telefono NVARCHAR(20) NOT NULL CONSTRAINT DF_Vehiculos_Telefono_Alt DEFAULT (N'') WITH VALUES;
        END

        IF OBJECT_ID(N'dbo.Trabajos', N'U') IS NOT NULL
           AND COL_LENGTH(N'dbo.Trabajos', N'Mecanico') IS NULL
        BEGIN
            ALTER TABLE dbo.Trabajos
            ADD Mecanico NVARCHAR(120) NOT NULL CONSTRAINT DF_Trabajos_Mecanico DEFAULT (N'Sin asignar') WITH VALUES;
        END
        """;


    // ============================================================
    // SCRIPT NUEVO: TABLA AUDITORÍA / HISTORIAL DE FINALIZADOS
    // ============================================================
    private const string ScriptTablaHistorial =
        """
        IF OBJECT_ID(N'dbo.HistorialServiciosFinalizados', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.HistorialServiciosFinalizados
            (
                IdHistorial      INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_HistorialServicios PRIMARY KEY,
                Placa            NVARCHAR(15)       NOT NULL,
                Estado           NVARCHAR(30)       NOT NULL,
                Dni              NVARCHAR(15)       NOT NULL,
                Telefono         NVARCHAR(20)       NOT NULL,
                Cliente          NVARCHAR(120)      NOT NULL,
                Modelo           NVARCHAR(80)       NOT NULL,
                Mecanico         NVARCHAR(120)      NOT NULL,
                HoraDeFinalizado DATETIME2(0)       NOT NULL CONSTRAINT DF_Historial_HoraDeFinalizado DEFAULT (SYSDATETIME())
            );
        END
        """;

    // ============================================================
    // SCRIPT NUEVO: TRIGGER AUTOMÁTICO EN C# (Mapeado a tus tablas)
    // ============================================================
    private const string ScriptTriggerFinalizados =
        """
        IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'TR_TrabajoFinalizado')
        BEGIN
            EXEC('
                CREATE TRIGGER dbo.TR_TrabajoFinalizado
                ON dbo.Trabajos
                AFTER UPDATE
                AS
                BEGIN
                    SET NOCOUNT ON;

                    -- Evaluamos si cambió el estado y su nuevo valor es Finalizado
                    IF UPDATE(Estado)
                    BEGIN
                        INSERT INTO dbo.HistorialServiciosFinalizados 
                        (
                            Placa, Estado, Dni, Telefono, Cliente, Modelo, Mecanico, HoraDeFinalizado
                        )
                        SELECT 
                            i.Placa,
                            i.Estado,
                            v.Dni,
                            v.Telefono,
                            v.Cliente,
                            v.Modelo,
                            i.Mecanico,
                            SYSDATETIME()
                        FROM inserted i
                        INNER JOIN deleted d ON i.idTrabajo = d.idTrabajo
                        INNER JOIN dbo.Vehiculos v ON i.idVehiculo = v.idVehiculo
                        WHERE i.Estado = ''Finalizado'' 
                          AND (d.Estado IS NULL OR d.Estado <> ''Finalizado'');
                    END
                END
            ');
        END
        """;

    // SCRIPT NUEVO: TABLA RELACIONAL PARA IMÁGENES MÚLTIPLES POR PLACA
    private const string ScriptTablaImagenes =
        """
        IF OBJECT_ID(N'dbo.VehiculosImagenes', N'U') IS NULL
        BEGIN
            CREATE TABLE dbo.VehiculosImagenes
            (
                Id            INT IDENTITY(1, 1) NOT NULL CONSTRAINT PK_VehiculosImagenes PRIMARY KEY,
                Placa         NVARCHAR(15)       NOT NULL,
                DatosImagen   VARBINARY(MAX)     NOT NULL, -- Aloja los bytes reales comprimidos de la foto
                FechaRegistro DATETIME2(0)       NOT NULL CONSTRAINT DF_VehiculosImagenes_Fecha DEFAULT (SYSUTCDATETIME()),
                
                -- Si eliminas el vehículo con el botón Delete, se limpian sus fotos en cascada automáticamente
                CONSTRAINT FK_Imagenes_Vehiculos FOREIGN KEY (Placa) REFERENCES dbo.Vehiculos (Placa) ON DELETE CASCADE
            );
        END
        """;

}