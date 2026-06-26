using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TallerMecanicoApp.Data;
using TallerMecanicoApp.Helpers;
using TallerMecanicoApp.Models;

namespace TallerMecanicoApp;

// Formulario Principal: Controla el registro, actualización, eliminación y filtrado de vehículos (Módulo de Recepción).
public partial class Form1 : Form
{
    private readonly TallerRepository _repositorio = new();
    private readonly BindingList<Vehiculo> _vehiculos; // Lista reactiva enlazada directamente al DataGridView

    // Código nativo de Windows para activar la marca de agua gris
    private const int EM_SETCUEBANNER = 0x1501;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

    public Form1()
    {
        InitializeComponent();

        colIdVehiculo.DataPropertyName = "idVehiculo";

        // Inicialización y mapeo de datos en la grilla sin autogenerar columnas basura
        _vehiculos = new BindingList<Vehiculo>();
        dgvVehiculos.AutoGenerateColumns = false;
        dgvVehiculos.DataSource = _vehiculos;


        // Estilizado visual de la grilla
        dgvVehiculos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        // Enlace de eventos del ciclo de vida y controles de la interfaz
        dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
        txtBuscar.TextChanged += txtBuscar_TextChanged;

        // INTERACTIVOS: Enlazamos los nuevos componentes de fotos
        btnSubirImagen.Click += btnSubirImagen_Click;
        dgvVehiculos.CellContentClick += dgvVehiculos_CellContentClick;

        // Inyección del texto de sugerencia en el cuadro de búsqueda mediante código nativo
        SendMessage(txtBuscar.Handle, EM_SETCUEBANNER, 1, "Ingresa el DNI o Número de Placa...");
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        // Apagamos el evento temporalmente al arrancar para que la selección automática inicial no trabe los controles
        dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;

        CargarVehiculos();

        // Controlamos los anchos proporcionales de tus columnas de datos reales
        if (dgvVehiculos.Columns["colId"] != null) { dgvVehiculos.Columns["colId"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells; dgvVehiculos.Columns["colId"].DisplayIndex = 0; dgvVehiculos.Columns["colId"].DefaultCellStyle.Format = "D2"; }
        if (dgvVehiculos.Columns["colPlaca"] != null) dgvVehiculos.Columns["colPlaca"].FillWeight = 10;
        if (dgvVehiculos.Columns["colCliente"] != null) dgvVehiculos.Columns["colCliente"].FillWeight = 20;
        if (dgvVehiculos.Columns["colTelefono"] != null) dgvVehiculos.Columns["colTelefono"].FillWeight = 12;
        if (dgvVehiculos.Columns["colModelo"] != null) dgvVehiculos.Columns["colModelo"].FillWeight = 16;
        if (dgvVehiculos.Columns["colDni"] != null) dgvVehiculos.Columns["colDni"].FillWeight = 12;
        if (dgvVehiculos.Columns["colEstadoVehiculo"] != null) dgvVehiculos.Columns["colEstadoVehiculo"].FillWeight = 12;
        if (dgvVehiculos.Columns["colImagenes"] != null) dgvVehiculos.Columns["colImagenes"].FillWeight = 11;
        if (dgvVehiculos.Columns["colFechaRegistro"] != null) dgvVehiculos.Columns["colFechaRegistro"].FillWeight = 15;

        dgvVehiculos.ClearSelection();
        LimpiarCamposVehiculo();

        dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
    }

    // ============================================================
    // LOGICA AUTOMÁTICA DE BÚSQUEDA / SELECCIÓN
    // ============================================================
    private void dgvVehiculos_SelectionChanged(object sender, EventArgs e)
    {
        // Evento que se dispara al hacer clic en una fila: Autorrellena los cuadros de texto superiores
        if (dgvVehiculos.CurrentRow?.DataBoundItem is Vehiculo vehiculo)
        {
            txtPlaca.Text = vehiculo.Placa;
            txtCliente.Text = vehiculo.Cliente;
            txtModelo.Text = vehiculo.Modelo;
            txtTelefono.Text = vehiculo.Telefono;
            txtDni.Text = vehiculo.Dni;

            // Bloqueamos la placa para impedir alteraciones accidentales en el UPDATE por ser la llave primaria física en SQL
            txtPlaca.ReadOnly = true;

            // --- GESTIÓN DE VISTA PREVIA DE IMÁGENES ---
            pbVisorImagen.Image = null; // Reseteamos el visor superior por seguridad

            try
            {
                var fotosBytes = _repositorio.ObtenerImagenesPorPlaca(vehiculo.Placa);
                if (fotosBytes != null && fotosBytes.Count > 0)
                {
                    // Convertimos el primer arreglo de bytes en una imagen real para el visor superior
                    using var ms = new MemoryStream(fotosBytes[0]);
                    pbVisorImagen.Image = Image.FromStream(ms);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error al cargar miniatura: " + ex.Message);
            }
        }
    }

    // ============================================================
    // ¡NUEVO!: ACCIÓN PARA ADJUNTAR MÚLTIPLES FOTOS EN LOTE CON LIMITE DE PESO
    // ============================================================
    private void btnSubirImagen_Click(object sender, EventArgs e)
    {
        string placa = txtPlaca.Text.Trim().ToUpper();

        // Validamos que exista una placa válida escrita o seleccionada antes de meter fotos a SQL
        if (string.IsNullOrWhiteSpace(placa) || placa == "PLACA")
        {
            MessageBox.Show("Por favor, escriba o seleccione la placa de un vehículo para asociarle las fotos.",
                "Placa Requerida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!_repositorio.ExistePlaca(placa))
        {
            MessageBox.Show("La placa ingresada debe estar guardada en el sistema antes de subir imágenes.",
                "Vehículo no encontrado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Mostramos la ventana nativa de selección de archivos de Windows
        if (openFileDialogImagenes.ShowDialog() == DialogResult.OK)
        {
            try
            {
                int contador = 0;
                int omitidosPorPeso = 0;

                // Recorremos secuencialmente todas las rutas seleccionadas por el operador
                foreach (string ruta in openFileDialogImagenes.FileNames)
                {
                    FileInfo infoArchivo = new FileInfo(ruta);

                    // LÍMITE DE SEGURIDAD: 3 MB por archivo (3 * 1024 * 1024 bytes) para evitar lentitud
                    if (infoArchivo.Length > 3145728)
                    {
                        omitidosPorPeso++;
                        continue;
                    }

                    byte[] fotoBytes = File.ReadAllBytes(ruta); // Conversión a binario puro
                    _repositorio.GuardarImagenVehiculo(placa, fotoBytes);
                    contador++;
                }

                if (omitidosPorPeso > 0)
                {
                    MessageBox.Show($"Carga completada parcialmente:\n\n" +
                                    $"✔ {contador} imágenes guardadas con éxito.\n" +
                                    $"⚠️ {omitidosPorPeso} archivos superaron el límite de 3 MB y fueron omitidos.",
                        "Control de Archivos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"¡Se guardaron exitosamente {contador} imágenes para la placa {placa}!",
                        "Carga Completa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Forzamos un refresco de la grilla resguardando el estado de los controles
                dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;
                CargarVehiculos(txtBuscar.Text);
                dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ocurrió un error al cargar los archivos: " + ex.Message,
                    "Error de Entrada/Salida", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ============================================================
    // ¡NUEVO!: ACCIÓN AL HACER CLIC EN EL BOTÓN "VER FOTOS" DEL GRID
    // ============================================================
    private void dgvVehiculos_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0 && dgvVehiculos.Columns[e.ColumnIndex].Name == "colImagenes")
        {
            if (dgvVehiculos.Rows[e.RowIndex].DataBoundItem is Vehiculo vehiculo)
            {
                var listaFotos = _repositorio.ObtenerImagenesPorPlaca(vehiculo.Placa);

                if (listaFotos == null || listaFotos.Count == 0)
                {
                    MessageBox.Show($"El vehículo con placa {vehiculo.Placa} no tiene fotos adjuntas por el momento.",
                        "Galería Vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Creamos un carrusel dinámico interactivo en caliente (FormFlotante)
                Form frmGaleria = new Form
                {
                    Text = $"Galería de Fotos - Placa [{vehiculo.Placa}]",
                    Size = new Size(500, 450),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false
                };

                int indiceActual = 0;

                PictureBox pbCarrusel = new PictureBox
                {
                    Location = new Point(20, 20),
                    Size = new Size(440, 320),
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                Button btnAnterior = new Button { Text = "◀ Anterior", Location = new Point(80, 360), Size = new Size(100, 30) };
                Button btnSiguiente = new Button { Text = "Siguiente ▶", Location = new Point(300, 360), Size = new Size(100, 30) };
                Label lblContador = new Label { Text = $"1 / {listaFotos.Count}", Location = new Point(200, 368), AutoSize = true, Font = new Font("Segoe UI", 9F, FontStyle.Bold) };

                Action CambiarFoto = () =>
                {
                    using var ms = new MemoryStream(listaFotos[indiceActual]);
                    pbCarrusel.Image = Image.FromStream(ms);
                    lblContador.Text = $"{indiceActual + 1} / {listaFotos.Count}";
                    btnAnterior.Enabled = indiceActual > 0;
                    btnSiguiente.Enabled = indiceActual < listaFotos.Count - 1;
                };

                CambiarFoto();

                btnAnterior.Click += (s, args) => { if (indiceActual > 0) { indiceActual--; CambiarFoto(); } };
                btnSiguiente.Click += (s, args) => { if (indiceActual < listaFotos.Count - 1) { indiceActual++; CambiarFoto(); } };

                frmGaleria.Controls.AddRange(new Control[] { pbCarrusel, btnAnterior, btnSiguiente, lblContador });
                frmGaleria.ShowDialog(this);
            }
        }
    }

    // ============================================================
    // BOTÓN: REGISTRAR VEHÍCULO
    // ============================================================
    private void btnRegistrarVehiculo_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtCliente.Text) ||
            string.IsNullOrWhiteSpace(txtModelo.Text) ||
            string.IsNullOrWhiteSpace(txtDni.Text) ||
            string.IsNullOrWhiteSpace(txtPlaca.Text))
        {
            MessageBox.Show("Completa todos los campos para registrar el vehículo.", "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!PlacaValidator.EsValida(txtPlaca.Text, out var mensajePlaca))
        {
            MessageBox.Show(mensajePlaca, "Validación",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPlaca.Focus();
            return;
        }

        var placa = PlacaValidator.Normalizar(txtPlaca.Text);

        try
        {
            if (_repositorio.ExistePlaca(placa))
            {
                MessageBox.Show("La placa ya está registrada.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var vehiculo = new Vehiculo(
                placa,
                txtCliente.Text.Trim(),
                txtModelo.Text.Trim(),
                txtDni.Text.Trim(),
                txtTelefono.Text.Trim());

            dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;

            _repositorio.RegistrarVehiculo(vehiculo);
            CargarVehiculos();
            LimpiarCamposVehiculo();

            dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
        }
        catch (Exception ex)
        {
            dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
            MostrarErrorBaseDatos(ex);
        }
    }

    // ============================================================
    // OPERACIÓN CRUD: UPDATE (Actualizar Datos por Placa)
    // ============================================================
    public void btnUpdate_Click(object sender, EventArgs e)
    {
        if (dgvVehiculos.CurrentRow?.DataBoundItem is Vehiculo vehiculo)
        {
            if (string.IsNullOrWhiteSpace(txtCliente.Text) ||
                string.IsNullOrWhiteSpace(txtModelo.Text) ||
                string.IsNullOrWhiteSpace(txtDni.Text))
            {
                MessageBox.Show("Por favor, llena los datos en los campos superiores antes de actualizar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var respuesta = MessageBox.Show($"¿Deseas actualizar los datos del vehículo con Placa {vehiculo.Placa}?", "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    vehiculo.Cliente = txtCliente.Text.Trim();
                    vehiculo.Modelo = txtModelo.Text.Trim();
                    vehiculo.Telefono = txtTelefono.Text.Trim();
                    vehiculo.Dni = txtDni.Text.Trim();

                    dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;

                    _repositorio.ActualizarVehiculo(vehiculo);
                    CargarVehiculos();
                    LimpiarCamposVehiculo();

                    dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
                    MessageBox.Show("Registro modificado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
                    MostrarErrorBaseDatos(ex);
                }
            }
        }
        else
        {
            MessageBox.Show("Selecciona un vehículo de la lista de abajo para actualizar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ============================================================
    // OPERACIÓN CRUD: DELETE (Eliminar Registro)
    // ============================================================
    public void btnDelete_Click(object sender, EventArgs e)
    {
        if (dgvVehiculos.CurrentRow?.DataBoundItem is Vehiculo vehiculo)
        {
            var respuesta = MessageBox.Show($"¿Estás seguro de que deseas eliminar permanentemente el vehículo con placa {vehiculo.Placa}?", "Confirmar Eliminación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;

                    _repositorio.EliminarVehiculo(vehiculo.idVehiculo);
                    CargarVehiculos();
                    LimpiarCamposVehiculo();

                    dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
                    MessageBox.Show("Vehículo removido con éxito de la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
                    MostrarErrorBaseDatos(ex);
                }
            }
        }
        else
        {
            MessageBox.Show("Selecciona el vehículo de la lista que deseas remover.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ============================================================
    // FILTRO DINÁMICO POR TEXTO (PLACA O DNI)
    // ============================================================
    private void txtBuscar_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBuscar.Text))
        {
            dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;

            CargarVehiculos("");
            dgvVehiculos.ClearSelection();
            LimpiarCamposVehiculo();

            dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
        }
        else
        {
            CargarVehiculos(txtBuscar.Text);
        }
    }

    // =============================================================================================
    // NAVEGACIÓN: Lanza el Form2 (Gestión de Trabajos) heredando la Placa del vehículo seleccionado
    // =============================================================================================
    private void btnRegistroTrabajos_Click(object sender, EventArgs e)
    {
        CargarVehiculos();

        if (_vehiculos.Count == 0)
        {
            MessageBox.Show("Debes registrar al menos un vehículo para asignar trabajos.", "Información",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var placaSeleccionada = dgvVehiculos.CurrentRow?.DataBoundItem is Vehiculo vehiculo
            ? vehiculo.Placa
            : null;

        using var formTrabajos = new Form2(placaSeleccionada);
        formTrabajos.ShowDialog(this);

        dgvVehiculos.SelectionChanged -= dgvVehiculos_SelectionChanged;
        CargarVehiculos();
        dgvVehiculos.ClearSelection();
        LimpiarCamposVehiculo();
        dgvVehiculos.SelectionChanged += dgvVehiculos_SelectionChanged;
    }

    private void CargarVehiculos(string filtro = "")
    {
        try
        {
            _vehiculos.Clear();
            var listaCompleta = _repositorio.ObtenerVehiculos();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                filtro = filtro.Trim().ToUpper();

                listaCompleta = listaCompleta.Where(v =>
                    v.Placa.ToUpper().Contains(filtro) ||
                    v.Dni.Contains(filtro)
                ).ToList();
            }

            foreach (var vehiculo in listaCompleta)
            {
                _vehiculos.Add(vehiculo);
            }
        }
        catch (Exception ex)
        {
            MostrarErrorBaseDatos(ex);
        }
    }

    private void LimpiarCamposVehiculo()
    {
        txtPlaca.Clear();
        txtPlaca.ReadOnly = false; // El control recupera la edición nativa de inmediato
        txtCliente.Clear();
        txtTelefono.Clear();
        txtModelo.Clear();
        txtDni.Clear();
        txtBuscar.Clear();
        pbVisorImagen.Image = null;
        dgvVehiculos.ClearSelection();
        txtPlaca.Focus();
    }

    private static void MostrarErrorBaseDatos(Exception ex)
    {
        MessageBox.Show(
            $"No se pudo conectar o guardar en SQL Server.{Environment.NewLine}{Environment.NewLine}{ex.Message}",
            "Base de datos",
            MessageBoxButtons.OK,
            MessageBoxIcon.Error);
    }
}