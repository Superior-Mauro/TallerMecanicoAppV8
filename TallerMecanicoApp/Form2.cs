using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using TallerMecanicoApp.Data;
using TallerMecanicoApp.Models;

namespace TallerMecanicoApp;

public partial class Form2 : Form
{
    private readonly TallerRepository _repositorio = new();
    private readonly BindingList<Trabajo> _trabajos;
    private readonly BindingList<Vehiculo> _vehiculos = new();
    private readonly BindingList<Mecanico> _mecanicos = new();
    private readonly BindingSource _vehiculosBindingSource = new();
    private readonly BindingSource _mecanicosBindingSource = new();
    private readonly List<Servicio> _servicios;
    private readonly string? _placaPreseleccionada;

    // API para Watermark
    private const int EM_SETCUEBANNER = 0x1501;
    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

    public Form2(string? placaPreseleccionada = null)
    {
        InitializeComponent();
        _placaPreseleccionada = placaPreseleccionada;
        _trabajos = new BindingList<Trabajo>();
        _servicios = CatalogoServicios.ObtenerTodos();

        dgvTrabajos.AutoGenerateColumns = false;
        dgvTrabajos.DataSource = _trabajos;
        dgvTrabajos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

        _vehiculosBindingSource.DataSource = _vehiculos;
        cbPlaca.DataSource = _vehiculosBindingSource;
        cbPlaca.DisplayMember = nameof(Vehiculo.Placa);
        cbPlaca.ValueMember = "idVehiculo";

        _mecanicosBindingSource.DataSource = _mecanicos;
        cbMecanico.DataSource = _mecanicosBindingSource;
        cbMecanico.DisplayMember = nameof(Mecanico.Nombre);
        cbMecanico.ValueMember = nameof(Mecanico.Id);

        // Enlazamos eventos dinámicos de control
        dgvTrabajos.SelectionChanged += dgvTrabajos_SelectionChanged;
        txtBuscarTrabajo.TextChanged += txtBuscarTrabajo_TextChanged;

        // NUEVO: Enlace para detectar el clic en la celda de "Ver fotos"
        dgvTrabajos.CellContentClick += dgvTrabajos_CellContentClick;

        if (dgvTrabajos.Columns["colIdTrabajo"] != null) {dgvTrabajos.Columns["colIdTrabajo"].DefaultCellStyle.Format = "D2"; }
        if (dgvTrabajos.Columns["colIdVehiculo"] != null)
        {
            dgvTrabajos.Columns["colIdVehiculo"].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
    }

    private void Form2_Load(object sender, EventArgs e)
    {
        ConfigurarControles();
        CargarDatos();

        // Activamos marca de agua elegante
        SendMessage(txtBuscarTrabajo.Handle, EM_SETCUEBANNER, 1, "Mecánico o Placa...");

        // Iniciamos el formulario limpio y vacío
        dgvTrabajos.ClearSelection();
        LimpiarFormulario();
    }

    private void ConfigurarControles()
    {
        cbServicio.DataSource = _servicios;
        cbServicio.DisplayMember = nameof(Servicio.Nombre);
        cbServicio.ValueMember = nameof(Servicio.Nombre);

        cbEstado.Items.Clear();
        cbEstado.Items.AddRange(new object[] { "Pendiente", "En proceso", "Finalizado" });
        cbEstado.SelectedIndex = 0;
        cbServicio.SelectedIndex = 0;
    }

    private void CargarDatos(string filtro = "")
    {
        try
        {
            if (_vehiculos.Count == 0)
            {
                var vehiculosBD = _repositorio.ObtenerVehiculos();
                foreach (var vehiculo in vehiculosBD)
                {
                    _vehiculos.Add(vehiculo);
                }
                CargarMecanicos();
                if (cbMecanico.Items.Count > 0) cbMecanico.SelectedIndex = 0;
                SeleccionarPlaca(_placaPreseleccionada);
            }

            _trabajos.Clear();
            var listaTrabajos = _repositorio.ObtenerTrabajos();

            // ============================================================
            // LÓGICA DE FILTRADO INTELIGENTE (LINQ)
            // ============================================================
            if (!string.IsNullOrWhiteSpace(filtro))
            {
                // SI EL OPERADOR ESCRIBE ALGO: Se busca en TODO (incluyendo los "Finalizado")
                filtro = filtro.Trim().ToUpper();
                listaTrabajos = listaTrabajos.Where(t =>
                    t.Placa.ToUpper().Contains(filtro) ||
                    t.Mecanico.ToUpper().Contains(filtro)
                ).ToList();
            }


            foreach (var trabajo in listaTrabajos)
            {
                _trabajos.Add(trabajo);
            }

            RecalcularTotales();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error en SQL Server: {ex.Message}", "Base de datos", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CargarMecanicos()
    {
        _mecanicos.Clear();
        var id = 1;
        foreach (var nombre in CatalogoMecanicos.Nombres)
        {
            _mecanicos.Add(new Mecanico { Id = id++, Nombre = nombre });
        }
    }

    // ============================================================
    // SELECCIÓN AUTOMÁTICA DE LA FILA DE TRABAJOS
    // ============================================================
    private void dgvTrabajos_SelectionChanged(object sender, EventArgs e)
    {
        if (dgvTrabajos.CurrentRow?.DataBoundItem is Trabajo trabajo)
        {
            txtDescripcion.Text = trabajo.Descripcion;
            cbEstado.Text = trabajo.Estado;
            cbMecanico.Text = trabajo.Mecanico;
            cbServicio.Text = trabajo.ServicioNombre;

            chkRefrigerante.Checked = trabajo.CambioRefrigerante;
            chkLiquidoFrenos.Checked = trabajo.CambioLiquidoFrenos;
            chkBujias.Checked = trabajo.CambioBujias;

            cbPlaca.SelectedValue = trabajo.idVehiculo;

            // ============================================================
            // ACTUALIZADO: CARGAR MINIATURA POR idTrabajo 📷
            // ============================================================
            try
            {
                // Usamos el nuevo método del repositorio que busca por ID de orden
                var imagenes = _repositorio.ObtenerImagenesPorTrabajo(trabajo.idTrabajo);

                if (imagenes != null && imagenes.Count > 0)
                {
                    using (var ms = new System.IO.MemoryStream(imagenes[0]))
                    {
                        pbVehiculoTrabajo.Image = Image.FromStream(ms);
                    }
                }
                else
                {
                    pbVehiculoTrabajo.Image = null;
                }
            }
            catch { pbVehiculoTrabajo.Image = null; }
        }
    }

    // ============================================================
    // FILTRADO DINÁMICO POR TEXTO
    // ============================================================
    private void txtBuscarTrabajo_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtBuscarTrabajo.Text))
        {
            dgvTrabajos.SelectionChanged -= dgvTrabajos_SelectionChanged;
            CargarDatos("");
            dgvTrabajos.ClearSelection();
            LimpiarFormulario();
            dgvTrabajos.SelectionChanged += dgvTrabajos_SelectionChanged;
        }
        else
        {
            CargarDatos(txtBuscarTrabajo.Text);
        }
    }

    private void btnRegistrarTrabajo_Click(object sender, EventArgs e)
    {
        if (cbPlaca.SelectedItem is not Vehiculo vSel || 
            cbServicio.SelectedItem is not Servicio sSel || 
            string.IsNullOrWhiteSpace(txtDescripcion.Text) || 
            cbMecanico.SelectedItem is not Mecanico mSel)
        {
            MessageBox.Show("Completa la información necesaria.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int idVehiculoRelacional = vSel.idVehiculo;
        string placaTexto = vSel.Placa;

        var trabajo = ConstruitObjetoTrabajo(idVehiculoRelacional, placaTexto, mSel.Nombre, sSel); try
        {
            _repositorio.RegistrarTrabajo(trabajo);
            CargarDatos();
            LimpiarFormulario();
            MessageBox.Show("Trabajo creado con éxito.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error al guardar: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // ============================================================
    // BOTÓN: UPDATE TRABAJO (No estaba implementado en el Repositorio anterior, se emula o prepara)
    // ============================================================
    private void btnUpdateTrabajo_Click(object sender, EventArgs e)
    {
        if (dgvTrabajos.CurrentRow?.DataBoundItem is Trabajo trabajoSeleccionado)
        {
            if (cbPlaca.SelectedItem is not Vehiculo vSel || 
                cbServicio.SelectedItem is not Servicio sSel || 
                cbMecanico.SelectedItem is not Mecanico mSel)
            {
                MessageBox.Show("Por favor, asegúrate de seleccionar un vehículo, servicio y mecánico válidos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var respuesta = MessageBox.Show($"¿Deseas guardar los cambios de la orden N° {trabajoSeleccionado.idTrabajo:D2}?", "Confirmar Modificación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    // Construimos el objeto actualizado manteniendo el mismo ID original de la fila
                    var trabajoActualizado = ConstruitObjetoTrabajo(vSel.idVehiculo, vSel.Placa, mSel.Nombre, sSel); 
                    trabajoActualizado.idTrabajo = trabajoSeleccionado.idTrabajo;

                    // Guardamos el cambio directamente en SQL Server
                    _repositorio.ActualizarTrabajo(trabajoActualizado);

                    CargarDatos();
                    LimpiarFormulario();
                    MessageBox.Show("Orden de trabajo actualizada en SQL Server correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al actualizar: {ex.Message}", "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Selecciona un trabajo de la grilla para actualizar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    // ============================================================
    // BOTÓN: DELETE TRABAJO
    // ============================================================
    private void btnDeleteTrabajo_Click(object sender, EventArgs e)
    {
        if (dgvTrabajos.CurrentRow?.DataBoundItem is Trabajo trabajo)
        {
            var respuesta = MessageBox.Show($"¿Estás seguro de remover permanentemente la orden N° {trabajo.idTrabajo:D2} asociada a la placa {trabajo.Placa}?", "Peligro", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (respuesta == DialogResult.Yes)
            {
                try
                {
                    // Eliminamos directamente desde la base de datos usando el ID único de la orden
                    _repositorio.EliminarTrabajo(trabajo.idTrabajo);

                    CargarDatos();
                    LimpiarFormulario();
                    MessageBox.Show("Orden removida exitosamente de la base de datos.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al eliminar de SQL: {ex.Message}", "Error SQL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        else
        {
            MessageBox.Show("Selecciona una orden de trabajo de la lista para eliminar.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }


    // ============================================================
    // EVENTO: ADJUNTAR FOTO DESDE REGISTRO DE TRABAJOS
    // ============================================================
    private void btnAdjuntarFotos_Click(object sender, EventArgs e)
    {
        // CAMBIO CRÍTICO: Obtenemos el trabajo directamente de la fila seleccionada en la grilla
        if (dgvTrabajos.CurrentRow?.DataBoundItem is not Trabajo trabajoSeleccionado)
        {
            MessageBox.Show("Por favor, selecciona una orden de trabajo de la lista de abajo para adjuntarle fotos.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Seguridad: Si el trabajo es nuevo y no se ha guardado, el ID será 0
        if (trabajoSeleccionado.idTrabajo <= 0)
        {
            MessageBox.Show("Debes registrar el trabajo primero antes de añadirle fotos.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Imágenes (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
        dialog.Title = $"Adjuntar foto a la Orden N° {trabajoSeleccionado.idTrabajo:D2}";

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            try
            {
                byte[] imagenBytes = System.IO.File.ReadAllBytes(dialog.FileName);

                // GUARDAMOS VINCULANDO AL idTrabajo
                _repositorio.GuardarImagenTrabajo(trabajoSeleccionado.idTrabajo, imagenBytes);

                // Refrescamos la miniatura lateral de inmediato
                using (var ms = new System.IO.MemoryStream(imagenBytes))
                {
                    pbVehiculoTrabajo.Image = Image.FromStream(ms);
                }

                MessageBox.Show($"¡Foto guardada con éxito en la Orden N° {trabajoSeleccionado.idTrabajo:D2}!", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la imagen: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }


    // ============================================================
    // EVENTO: VER FOTOS DESDE EL DATAGRIDVIEW
    // ============================================================
    private void dgvTrabajos_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        // 1. Validamos que el clic sea en la columna de la galería y no en el encabezado
        if (e.RowIndex >= 0 && dgvTrabajos.Columns[e.ColumnIndex].Name == "colGaleriaFotos")
        {
            if (dgvTrabajos.Rows[e.RowIndex].DataBoundItem is Trabajo trabajoSeleccionado)
            {
                // 2. Recuperamos las fotos asociadas al idTrabajo específico (Relacional)
                var listaFotos = _repositorio.ObtenerImagenesPorTrabajo(trabajoSeleccionado.idTrabajo);

                if (listaFotos == null || listaFotos.Count == 0)
                {
                    MessageBox.Show($"La orden de trabajo N° {trabajoSeleccionado.idTrabajo:D2} no tiene fotos registradas.",
                        "Galería Vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // 3. CREACIÓN DE LA VENTANA DE GALERÍA DINÁMICA (CARRUSEL)
                Form frmGaleria = new Form
                {
                    Text = $"Evidencia de Trabajo - Orden N° [{trabajoSeleccionado.idTrabajo:D2}]",
                    Size = new Size(500, 450),
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.White
                };

                int indiceActual = 0;

                // Contenedor de la imagen
                PictureBox pbCarrusel = new PictureBox
                {
                    Location = new Point(20, 20),
                    Size = new Size(440, 320),
                    BorderStyle = BorderStyle.FixedSingle,
                    SizeMode = PictureBoxSizeMode.Zoom
                };

                // Botones de navegación
                Button btnAnterior = new Button { Text = "◀ Anterior", Location = new Point(80, 360), Size = new Size(100, 30) };
                Button btnSiguiente = new Button { Text = "Siguiente ▶", Location = new Point(300, 360), Size = new Size(100, 30) };
                Label lblContador = new Label
                {
                    Text = $"1 / {listaFotos.Count}",
                    Location = new Point(200, 368),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };

                // Lógica para cambiar la imagen en el visor
                Action ActualizarVisor = () =>
                {
                    // Liberamos memoria de la imagen anterior si existe
                    pbCarrusel.Image?.Dispose();

                    using var ms = new MemoryStream(listaFotos[indiceActual]);
                    pbCarrusel.Image = Image.FromStream(ms);

                    lblContador.Text = $"{indiceActual + 1} / {listaFotos.Count}";
                    btnAnterior.Enabled = indiceActual > 0;
                    btnSiguiente.Enabled = indiceActual < listaFotos.Count - 1;
                };

                // Eventos de los botones
                btnAnterior.Click += (s, args) => { if (indiceActual > 0) { indiceActual--; ActualizarVisor(); } };
                btnSiguiente.Click += (s, args) => { if (indiceActual < listaFotos.Count - 1) { indiceActual++; ActualizarVisor(); } };

                // Iniciar carrusel
                ActualizarVisor();

                // Agregar controles y mostrar
                frmGaleria.Controls.AddRange(new Control[] { pbCarrusel, btnAnterior, btnSiguiente, lblContador });
                frmGaleria.ShowDialog(this);
            }
        }
    }

    private Trabajo ConstruitObjetoTrabajo(int idVehiculo, string placa, string mecanico, Servicio serv)
    {
        var total = serv.PrecioBase;
        var tiempo = serv.TiempoBase;

        if (chkRefrigerante.Checked) { total += 90m; tiempo += TimeSpan.FromMinutes(40); }
        if (chkLiquidoFrenos.Checked) { total += 50m; tiempo += TimeSpan.FromMinutes(30); }

        var aplicaBujias = (serv.Nombre is "Mantenimiento Regular" or "Mantenimiento Completo" or "Afinamiento") && chkBujias.Checked;
        if (aplicaBujias) { total += 40m; tiempo += TimeSpan.FromMinutes(10); }

        return new Trabajo
        {
            idVehiculo = idVehiculo,
            Placa = placa,
            Mecanico = mecanico,
            Descripcion = txtDescripcion.Text.Trim(),
            Estado = cbEstado.Text,
            ServicioNombre = serv.Nombre,
            PrecioBase = serv.PrecioBase,
            TiempoBase = serv.TiempoBase,
            CambioRefrigerante = chkRefrigerante.Checked,
            CambioLiquidoFrenos = chkLiquidoFrenos.Checked,
            CambioBujias = aplicaBujias,
            TotalPagar = total,
            TiempoEstimado = tiempo
        };
    }

    private void cbServicio_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cbServicio.SelectedItem is not Servicio sSel) return;

        // Si el usuario selecciona "OTROS", disparamos la ventana emergente personalizada
        if (sSel.Nombre == "Otros")
        {
            // Crear el formulario flotante temporal
            Form ventanaOtros = new Form();
            ventanaOtros.Text = "Detalle de Trabajo Personalizado";
            ventanaOtros.Size = new Size(400, 320);
            ventanaOtros.FormBorderStyle = FormBorderStyle.FixedDialog;
            ventanaOtros.StartPosition = FormStartPosition.CenterParent;
            ventanaOtros.MaximizeBox = false;
            ventanaOtros.MinimizeBox = false;

            // Crear controles: Etiquetas
            Label lblObs = new Label() { Text = "Observaciones / Tareas:", Location = new Point(20, 20), Width = 200 };
            Label lblCosto = new Label() { Text = "Costo del Trabajo (S/):", Location = new Point(20, 130), Width = 150 };
            Label lblTiempo = new Label() { Text = "Tiempo Estimado (Minutos):", Location = new Point(20, 180), Width = 180 };

            // Crear controles: Cajas de texto e ingresos
            TextBox txtObs = new TextBox() { Location = new Point(20, 45), Width = 340, Multiline = true, Height = 70 };
            NumericUpDown numCosto = new NumericUpDown() { Location = new Point(220, 128), Width = 140, Maximum = 99999, DecimalPlaces = 2 };
            NumericUpDown numTiempo = new NumericUpDown() { Location = new Point(220, 178), Width = 140, Maximum = 9999 };

            // Crear control: Botón Aceptar
            Button btnAceptar = new Button() { Text = "Aceptar", Location = new Point(140, 230), Width = 110, Height = 30, DialogResult = DialogResult.OK };
            ventanaOtros.AcceptButton = btnAceptar;

            // Agregar los controles a la ventana flotante
            ventanaOtros.Controls.AddRange(new Control[] { lblObs, txtObs, lblCosto, numCosto, lblTiempo, numTiempo, btnAceptar });

            // Mostrar la ventana y esperar que el usuario presione Aceptar
            if (ventanaOtros.ShowDialog() == DialogResult.OK)
            {
                // Transferimos los valores digitados al servicio "Otros" en memoria
                sSel.Detalle = txtObs.Text.Trim();
                sSel.PrecioBase = numCosto.Value;
                sSel.TiempoBase = TimeSpan.FromMinutes((double)numTiempo.Value);

                // Colocamos la observación en el cuadro de descripción principal automáticamente
                txtDescripcion.Text = txtObs.Text.Trim();
            }
            else
            {
                // Si cancela o cierra la ventana, regresamos el combo al primer servicio para no dejarlo trabado
                cbServicio.SelectedIndex = 0;
                return;
            }
        }

        // Actualiza la etiqueta de dos líneas en la pantalla principal
        lblResumenServicio.Text = $"{sSel.Detalle}\r\nBase: S/{sSel.PrecioBase:0.00} - {FormatearTiempo(sSel.TiempoBase)}";

        // Regla de bujías: Solo se habilita si es Regular, Completo o Afinamiento
        var aplicaBujias = sSel.Nombre is "Mantenimiento Regular" or "Mantenimiento Completo" or "Afinamiento";
        chkBujias.Enabled = aplicaBujias;
        if (!aplicaBujias) chkBujias.Checked = false;

        RecalcularTotales();
    }

    private void chkAdicional_CheckedChanged(object sender, EventArgs e) => RecalcularTotales();

    private void RecalcularTotales()
    {
        if (cbServicio.SelectedItem is not Servicio sSel)
        {
            lblTotal.Text = "Total: S/0.00";
            lblTiempoEstimado.Text = "Tiempo estimado: 0h 0m";
            return;
        }

        var total = sSel.PrecioBase;
        var tiempo = sSel.TiempoBase;

        // Lógica matemática exacta según tus especificaciones
        if (chkRefrigerante.Checked) { total += 90m; tiempo += TimeSpan.FromMinutes(40); }
        if (chkLiquidoFrenos.Checked) { total += 50m; tiempo += TimeSpan.FromMinutes(30); }
        if (chkBujias.Checked && chkBujias.Enabled) { total += 40m; tiempo += TimeSpan.FromMinutes(10); }

        lblTotal.Text = $"Total: S/{total:0.00}";
        lblTiempoEstimado.Text = $"Tiempo estimado: {FormatearTiempo(tiempo)}";
    }

    private void SeleccionarPlaca(string? placa)
    {
        if (string.IsNullOrWhiteSpace(placa) || _vehiculos.Count == 0)
        {
            if (_vehiculos.Count > 0) cbPlaca.SelectedIndex = 0;
            return;
        }
        var veh = _vehiculos.FirstOrDefault(v => v.Placa.Equals(placa, StringComparison.OrdinalIgnoreCase));
        if (veh != null) cbPlaca.SelectedValue = veh.idVehiculo;
    }

    private string? ObtenerPlacaSeleccionada() => cbPlaca.SelectedItem is Vehiculo v ? v.Placa : null;
    private static string FormatearTiempo(TimeSpan t) => $"{(int)t.TotalHours}h {t.Minutes}m";

    private void LimpiarFormulario()
    {
        // 1. Desactivamos temporalmente el evento de la grilla para evitar que el autorelleno se dispare al limpiar
        dgvTrabajos.SelectionChanged -= dgvTrabajos_SelectionChanged;

        // 2. Quitamos cualquier fila azul seleccionada en la grilla
        dgvTrabajos.ClearSelection();

        // 3. Vaciamos las cajas de texto y restablecemos los checkboxes
        txtDescripcion.Clear();
        chkRefrigerante.Checked = false;
        chkLiquidoFrenos.Checked = false;
        chkBujias.Checked = false;

        // 4. Restablecemos los ComboBoxes a sus posiciones iniciales por defecto
        if (cbEstado.Items.Count > 0) cbEstado.SelectedIndex = 0;
        if (cbMecanico.Items.Count > 0) cbMecanico.SelectedIndex = 0;
        if (cbPlaca.Items.Count > 0) cbPlaca.SelectedIndex = 0;
        if (cbServicio.Items.Count > 0) cbServicio.SelectedIndex = 0;

        // 5. ¡ESTO ES CRUCIAL PARA "OTROS"! 
        // Buscamos el servicio "Otros" en el catálogo y limpiamos sus valores para que la próxima orden empiece desde cero
        var servicioOtros = _servicios.FirstOrDefault(s => s.Nombre == "Otros");
        if (servicioOtros != null)
        {
            servicioOtros.Detalle = "Trabajo personalizado.\r\nDetallar manualmente las tareas en la descripción del recuadro.";
            servicioOtros.PrecioBase = 0m;
            servicioOtros.TiempoBase = TimeSpan.Zero;
        }

        // 6. Forzamos a la etiqueta informativa a mostrar los datos limpios por defecto
        if (cbServicio.SelectedItem is Servicio sSel)
        {
            lblResumenServicio.Text = $"{sSel.Detalle}\r\nBase: S/{sSel.PrecioBase:0.00} - {FormatearTiempo(sSel.TiempoBase)}";
        }

        // 7. Recalculamos los contadores visuales para que marquen S/0.00 y 0h 0m
        RecalcularTotales();

        // 8. Volvemos a activar el evento de selección para cuando el usuario dé un clic manual
        dgvTrabajos.SelectionChanged += dgvTrabajos_SelectionChanged;
    }
}