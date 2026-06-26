namespace TallerMecanicoApp.Models;

public class Trabajo
{
    public int idTrabajo { get; set; }
    public int idVehiculo { get; set; } // Nueva FK que conecta con Vehiculos.Id //borrar si falla
    public string Placa { get; set; } = string.Empty;
    public string Mecanico { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Estado { get; set; } = "Pendiente";
    public string ServicioNombre { get; set; } = string.Empty;
    public decimal PrecioBase { get; set; }
    public TimeSpan TiempoBase { get; set; }
    public bool CambioRefrigerante { get; set; }
    public bool CambioLiquidoFrenos { get; set; }
    public bool CambioBujias { get; set; }
    public decimal TotalPagar { get; set; }
    public TimeSpan TiempoEstimado { get; set; }

    public string TiempoEstimadoTexto =>
        $"{(int)TiempoEstimado.TotalHours}h {TiempoEstimado.Minutes}m";

    public string AdicionalesTexto
    {
        get
        {
            var adicionales = new List<string>();

            if (CambioRefrigerante)
            {
                adicionales.Add("Refrigerante");
            }

            if (CambioLiquidoFrenos)
            {
                adicionales.Add("Líquido frenos");
            }

            if (CambioBujias)
            {
                adicionales.Add("Bujías");
            }

            return adicionales.Count == 0 ? "Ninguno" : string.Join(", ", adicionales);
        }
    }
}
