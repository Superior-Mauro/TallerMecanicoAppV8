namespace TallerMecanicoApp.Models;

public class Vehiculo
{
    public Vehiculo(string placa, string cliente, string modelo, string dni, string telefono = "")
        : this(0, placa, cliente, modelo, dni, telefono, DateTime.Now, "Sin Trabajos")
    {
    }

    public Vehiculo(int idVeh, string placa, string cliente, string modelo, string dni, string telefono, DateTime fechaRegistro, string estado)
    {
        idVehiculo = idVeh;
        Placa = placa;
        Cliente = cliente;
        Modelo = modelo;
        Dni = dni; // <-- CAMBIADO
        Telefono = telefono;
        FechaRegistro = fechaRegistro; // <-- NUEVA PROPIEDAD ASIGNADA
        Estado = string.IsNullOrWhiteSpace(estado) ? "Sin Trabajos" : estado; // Por si es nuevo
    }

    public int idVehiculo { get; set; }
    public string Placa { get; set; }
    public string Cliente { get; set; }
    public string Modelo { get; set; }
    public string Dni { get; set; } // <-- CAMBIADO
    public string Telefono { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string Estado { get; set; }
}
