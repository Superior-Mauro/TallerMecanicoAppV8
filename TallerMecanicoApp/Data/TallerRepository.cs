using Microsoft.Data.SqlClient;
using TallerMecanicoApp.Models;

namespace TallerMecanicoApp.Data;

public sealed class TallerRepository
{
    // ==========================================
    // MÉTODOS DE USUARIOS (LOGIN Y REGISTRO)
    // ==========================================

    public bool ValidarUsuario(string usuario, string contrasena)
    {
        const string sql =
            """
            SELECT COUNT(1)
            FROM dbo.Usuarios
            WHERE NombreUsuario = @usuario 
              AND Contrasena = @contrasena;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@usuario", System.Data.SqlDbType.NVarChar).Value = usuario.Trim();
        comando.Parameters.Add("@contrasena", System.Data.SqlDbType.NVarChar).Value = contrasena.Trim();

        int resultado = Convert.ToInt32(comando.ExecuteScalar());
        return resultado > 0;
    }

    public bool ExisteUsuario(string usuario)
    {
        const string sql =
            """
            SELECT 1
            FROM dbo.Usuarios
            WHERE NombreUsuario = @usuario;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@usuario", usuario);
        return comando.ExecuteScalar() is not null;
    }

    public void RegistrarUsuario(string nombres, string usuario, string contrasena)
    {
        const string sql =
            """
            INSERT INTO dbo.Usuarios (Nombres, NombreUsuario, Contrasena)
            VALUES (@nombres, @usuario, @contrasena);
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@nombres", nombres);
        comando.Parameters.AddWithValue("@usuario", usuario);
        comando.Parameters.AddWithValue("@contrasena", contrasena);
        comando.ExecuteNonQuery();
    }

    // ==========================================
    // MÉTODOS DE VEHÍCULOS
    // ==========================================

    public IReadOnlyList<Vehiculo> ObtenerVehiculos()
    {
        // Esta consulta trae todos los datos del vehículo más el estado de su TRABAJO MÁS RECIENTE
        const string sql =
            """
        SELECT 
            v.idVehiculo, 
            v.Placa, 
            v.Cliente, 
            v.Telefono, 
            v.Modelo, 
            v.Dni, 
            v.FechaRegistro,
            ISNULL(t.Estado, 'Sin Trabajos') AS Estado
        FROM dbo.Vehiculos v
        OUTER APPLY (
            SELECT TOP 1 t.Estado
            FROM dbo.Trabajos t
            WHERE t.idVehiculo = v.idVehiculo
            ORDER BY t.FechaRegistro DESC, t.idTrabajo DESC
        ) t
        ORDER BY v.FechaRegistro DESC, v.idVehiculo DESC;
        """;

        var vehiculos = new List<Vehiculo>();

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            vehiculos.Add(new Vehiculo(
                lector.GetInt32(0),      // 1. idVehiculo (¡Esto solucionará el ID vacío!)
                lector.GetString(1),     // 2. Placa
                lector.GetString(2),     // 3. Cliente
                lector.GetString(4),     // 4. Modelo (Índice 4 en el SELECT)
                lector.GetString(5),     // 5. Dni (Índice 5 en el SELECT)
                lector.GetString(3),     // 6. Telefono (Índice 3 en el SELECT)
                lector.GetDateTime(6),   // 7. FechaRegistro (Índice 6 en el SELECT)
                lector.GetString(7)      // 8. Estado (Índice 7 en el SELECT)
            ));
        }

        return vehiculos;
    }

    public bool ExistePlaca(string placa)
    {
        const string sql =
            """
            SELECT 1
            FROM dbo.Vehiculos
            WHERE Placa = @placa;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@placa", placa);
        return comando.ExecuteScalar() is not null;
    }

    public void RegistrarVehiculo(Vehiculo vehiculo)
    {
        const string sql =
            """
            INSERT INTO dbo.Vehiculos (Placa, Cliente, Telefono, Modelo, Dni, FechaRegistro)
            VALUES (@placa, @cliente, @telefono, @modelo, @dni, @fechaRegistro);
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@placa", vehiculo.Placa);
        comando.Parameters.AddWithValue("@cliente", vehiculo.Cliente);
        comando.Parameters.AddWithValue("@telefono", vehiculo.Telefono);
        comando.Parameters.AddWithValue("@modelo", vehiculo.Modelo);
        comando.Parameters.AddWithValue("@dni", vehiculo.Dni);
        comando.Parameters.AddWithValue("@fechaRegistro", vehiculo.FechaRegistro);
        comando.ExecuteNonQuery();
    }

    public void ActualizarVehiculo(Vehiculo vehiculo)
    {
        const string sql =
            """
            UPDATE dbo.Vehiculos
            SET Cliente = @cliente,
                Telefono = @telefono,
                Modelo = @modelo,
                Dni = @dni,
                FechaRegistro = SYSDATETIME() -- Actualiza a la hora local actual de Lima
            WHERE idVehiculo = @idVehiculo;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idVehiculo", vehiculo.idVehiculo);
        comando.Parameters.AddWithValue("@cliente", vehiculo.Cliente);
        comando.Parameters.AddWithValue("@telefono", vehiculo.Telefono);
        comando.Parameters.AddWithValue("@modelo", vehiculo.Modelo);
        comando.Parameters.AddWithValue("@dni", vehiculo.Dni);

        comando.ExecuteNonQuery();
    }

    // ----------------------------------------------------------
    // METODO INYECTADO: ELIMINAR POR PLACA
    // ----------------------------------------------------------
    public void EliminarVehiculo(int idVehiculo)
    {
        const string sql =
            """
            DELETE FROM dbo.Vehiculos
            WHERE idVehiculo = @idVehiculo;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idVehiculo", idVehiculo);

        comando.ExecuteNonQuery();
    }

    // ==========================================
    // MÉTODOS DE TRABAJOS
    // ==========================================

    public IReadOnlyList<Trabajo> ObtenerTrabajos()
    {
        const string sql =
            """
            SELECT 
                t.idTrabajo, 
                t.idVehiculo, 
                v.Placa, 
                t.Mecanico, 
                t.Descripcion, 
                t.Estado, 
                t.ServicioNombre, 
                t.PrecioBase, 
                t.TiempoBaseMinutos,
                t.CambioRefrigerante, 
                t.CambioLiquidoFrenos, 
                t.CambioBujias, 
                t.TotalPagar, 
                t.TiempoEstimadoMinutos
            FROM dbo.Trabajos t
            INNER JOIN dbo.Vehiculos v ON t.idVehiculo = v.idVehiculo
            ORDER BY t.FechaRegistro DESC, t.idTrabajo DESC;
            """;

        var trabajos = new List<Trabajo>();

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        using var lector = comando.ExecuteReader();

        while (lector.Read())
        {
            trabajos.Add(new Trabajo
            {
                idTrabajo = lector.GetInt32(0),          // idTrabajo
                idVehiculo = lector.GetInt32(1),  // FK idVehiculo
                Placa = lector.GetString(2),       // Placa recuperada mediante JOIN relacional
                Mecanico = lector.GetString(3),
                Descripcion = lector.GetString(4),
                Estado = lector.GetString(5),
                ServicioNombre = lector.GetString(6),
                PrecioBase = lector.GetDecimal(7),
                TiempoBase = TimeSpan.FromMinutes(lector.GetInt32(8)),
                CambioRefrigerante = lector.GetBoolean(9),
                CambioLiquidoFrenos = lector.GetBoolean(10),
                CambioBujias = lector.GetBoolean(11),
                TotalPagar = lector.GetDecimal(12),
                TiempoEstimado = TimeSpan.FromMinutes(lector.GetInt32(13))
            });
        }

        return trabajos;
    }

    public void RegistrarTrabajo(Trabajo trabajo)
    {
        const string sql =
            """
            INSERT INTO dbo.Trabajos
            (
                idVehiculo, Placa, Mecanico, Descripcion, Estado, ServicioNombre, PrecioBase, TiempoBaseMinutos,
                CambioRefrigerante, CambioLiquidoFrenos, CambioBujias, TotalPagar, TiempoEstimadoMinutos
            )
            VALUES
            (
                @idVehiculo, @placa, @mecanico, @descripcion, @estado, @servicioNombre, @precioBase, @tiempoBaseMinutos,
                @cambioRefrigerante, @cambioLiquidoFrenos, @cambioBujias, @totalPagar, @tiempoEstimadoMinutos
            );
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idVehiculo", trabajo.idVehiculo);
        comando.Parameters.AddWithValue("@placa", trabajo.Placa);
        comando.Parameters.AddWithValue("@mecanico", trabajo.Mecanico);
        comando.Parameters.AddWithValue("@descripcion", trabajo.Descripcion);
        comando.Parameters.AddWithValue("@estado", trabajo.Estado);
        comando.Parameters.AddWithValue("@servicioNombre", trabajo.ServicioNombre);
        comando.Parameters.AddWithValue("@precioBase", trabajo.PrecioBase);
        comando.Parameters.AddWithValue("@tiempoBaseMinutos", (int)trabajo.TiempoBase.TotalMinutes);
        comando.Parameters.AddWithValue("@cambioRefrigerante", trabajo.CambioRefrigerante);
        comando.Parameters.AddWithValue("@cambioLiquidoFrenos", trabajo.CambioLiquidoFrenos);
        comando.Parameters.AddWithValue("@cambioBujias", trabajo.CambioBujias);
        comando.Parameters.AddWithValue("@totalPagar", trabajo.TotalPagar);
        comando.Parameters.AddWithValue("@tiempoEstimadoMinutos", (int)trabajo.TiempoEstimado.TotalMinutes);
        comando.ExecuteNonQuery();
    }

    public void ActualizarTrabajo(Trabajo trabajo)
    {
        const string sql =
            """
            UPDATE dbo.Trabajos
            SET idVehiculo = @idVehiculo,
                Placa = @placa,
                Mecanico = @mecanico,
                Descripcion = @descripcion,
                Estado = @estado,
                ServicioNombre = @servicioNombre,
                PrecioBase = @precioBase,
                TiempoBaseMinutos = @tiempoBaseMinutos,
                CambioRefrigerante = @cambioRefrigerante,
                CambioLiquidoFrenos = @cambioLiquidoFrenos,
                CambioBujias = @cambioBujias,
                TotalPagar = @totalPagar,
                TiempoEstimadoMinutos = @tiempoEstimadoMinutos
            WHERE idTrabajo = @idTrabajo;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idTrabajo", trabajo.idTrabajo);
        comando.Parameters.AddWithValue("@idVehiculo", trabajo.idVehiculo);
        comando.Parameters.AddWithValue("@placa", trabajo.Placa);
        comando.Parameters.AddWithValue("@mecanico", trabajo.Mecanico);
        comando.Parameters.AddWithValue("@descripcion", trabajo.Descripcion);
        comando.Parameters.AddWithValue("@estado", trabajo.Estado);
        comando.Parameters.AddWithValue("@servicioNombre", trabajo.ServicioNombre);
        comando.Parameters.AddWithValue("@precioBase", trabajo.PrecioBase);
        comando.Parameters.AddWithValue("@tiempoBaseMinutos", (int)trabajo.TiempoBase.TotalMinutes);
        comando.Parameters.AddWithValue("@cambioRefrigerante", trabajo.CambioRefrigerante);
        comando.Parameters.AddWithValue("@cambioLiquidoFrenos", trabajo.CambioLiquidoFrenos);
        comando.Parameters.AddWithValue("@cambioBujias", trabajo.CambioBujias);
        comando.Parameters.AddWithValue("@totalPagar", trabajo.TotalPagar);
        comando.Parameters.AddWithValue("@tiempoEstimadoMinutos", (int)trabajo.TiempoEstimado.TotalMinutes);

        comando.ExecuteNonQuery();
    }

    public void EliminarTrabajo(int idTrabajo)
    {
        const string sql =
            """
            DELETE FROM dbo.Trabajos
            WHERE idTrabajo = @idTrabajo;
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.AddWithValue("@idTrabajo", idTrabajo);

        comando.ExecuteNonQuery();
    }

    // ============================================================
    // MÉTODOS PARA GALERÍA DE IMÁGENES MÚLTIPLES
    // ============================================================

    public void GuardarImagenVehiculo(string placa, byte[] imagenBytes)
    {
        const string sql =
            """
            INSERT INTO dbo.VehiculosImagenes (Placa, datosImagen, FechaRegistro)
            VALUES (@placa, @datosImagen, SYSDATETIME());
            """;

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);

        comando.Parameters.Add("@placa", System.Data.SqlDbType.NVarChar, 15).Value = placa;
        comando.Parameters.Add("@datosImagen", System.Data.SqlDbType.VarBinary, -1).Value = imagenBytes;

        comando.ExecuteNonQuery();
    }

    public IReadOnlyList<byte[]> ObtenerImagenesPorPlaca(string placa)
    {
        const string sql =
            """
            SELECT datosImagen
            FROM dbo.VehiculosImagenes
            WHERE Placa = @placa
            ORDER BY FechaRegistro ASC;
            """;

        var imagenes = new List<byte[]>();

        using var conexion = AbrirConexion();
        using var comando = new SqlCommand(sql, conexion);
        comando.Parameters.Add("@placa", System.Data.SqlDbType.NVarChar, 15).Value = placa;

        using var lector = comando.ExecuteReader();
        while (lector.Read())
        {
            byte[] buffer = (byte[])lector["datosImagen"];
            imagenes.Add(buffer);
        }

        return imagenes;
    }

    // ==========================================
    // CONEXIÓN CENTRALIZADA
    // ==========================================

    private static SqlConnection AbrirConexion()
    {
        var conexion = new SqlConnection(DatabaseConnection.Obtener());
        conexion.Open();
        return conexion;
    }
}