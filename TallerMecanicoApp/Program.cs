using TallerMecanicoApp.Data;

namespace TallerMecanicoApp;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            TallerDbInitializer.Inicializar();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo inicializar la base de datos SQL Server.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
                "Base de datos",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            return;
        }

        Application.Run(new Form1());
    }
}