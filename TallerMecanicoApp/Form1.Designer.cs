namespace TallerMecanicoApp;

partial class Form1
{
    // Variable del diseñador requerida para la gestión de componentes.
    private System.ComponentModel.IContainer components = null;

    // Limpia los recursos (memoria) que se estén utilizando.
    /// <param name="disposing">true si los recursos administrados se deben desechar; de lo contrario, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    // Método necesario para el soporte del Diseñador. No se debe modificar
    // el contenido de este método con el editor de código.
    private void InitializeComponent()
    {
        DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
        lblTitulo = new Label();
        gbRecepcion = new GroupBox();
        pbVisorImagen = new PictureBox();
        btnSubirImagen = new Button();
        txtDni = new TextBox();
        txtModelo = new TextBox();
        txtTelefono = new TextBox();
        txtCliente = new TextBox();
        txtPlaca = new TextBox();
        lblDni = new Label();
        lblModelo = new Label();
        lblTelefono = new Label();
        lblCliente = new Label();
        lblPlaca = new Label();
        lblBuscar = new Label();
        txtBuscar = new TextBox();
        openFileDialogImagenes = new OpenFileDialog();
        btnRegistrarVehiculo = new Button();
        btnUpdate = new Button();
        btnDelete = new Button();
        btnRegistroTrabajos = new Button();
        dgvVehiculos = new DataGridView();
        colIdVehiculo = new DataGridViewTextBoxColumn();
        colPlaca = new DataGridViewTextBoxColumn();
        colCliente = new DataGridViewTextBoxColumn();
        colTelefono = new DataGridViewTextBoxColumn();
        colModelo = new DataGridViewTextBoxColumn();
        colDni = new DataGridViewTextBoxColumn();
        colEstadoVehiculo = new DataGridViewTextBoxColumn();
        colImagenes = new DataGridViewButtonColumn();
        colFechaRegistro = new DataGridViewTextBoxColumn();
        gbRecepcion.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pbVisorImagen).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvVehiculos).BeginInit();
        SuspendLayout();
        // 
        // lblTitulo
        // 
        lblTitulo.AutoSize = true;
        lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitulo.Location = new Point(22, 12);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(300, 30);
        lblTitulo.TabIndex = 12;
        lblTitulo.Text = "Recepción - Taller Mecánico";
        // 
        // gbRecepcion
        // 
        gbRecepcion.Controls.Add(pbVisorImagen);
        gbRecepcion.Controls.Add(btnSubirImagen);
        gbRecepcion.Controls.Add(txtDni);
        gbRecepcion.Controls.Add(txtModelo);
        gbRecepcion.Controls.Add(txtTelefono);
        gbRecepcion.Controls.Add(txtCliente);
        gbRecepcion.Controls.Add(txtPlaca);
        gbRecepcion.Controls.Add(lblDni);
        gbRecepcion.Controls.Add(lblModelo);
        gbRecepcion.Controls.Add(lblTelefono);
        gbRecepcion.Controls.Add(lblCliente);
        gbRecepcion.Controls.Add(lblPlaca);
        gbRecepcion.Controls.Add(lblBuscar);
        gbRecepcion.Controls.Add(txtBuscar);
        gbRecepcion.Location = new Point(24, 50);
        gbRecepcion.Name = "gbRecepcion";
        gbRecepcion.Size = new Size(1085, 155);
        gbRecepcion.TabIndex = 1;
        gbRecepcion.TabStop = false;
        gbRecepcion.Text = "Datos de ingreso";
        // 
        // pbVisorImagen
        // 
        pbVisorImagen.BackColor = Color.White;
        pbVisorImagen.BorderStyle = BorderStyle.FixedSingle;
        pbVisorImagen.Location = new Point(795, 23);
        pbVisorImagen.Name = "pbVisorImagen";
        pbVisorImagen.Size = new Size(270, 115);
        pbVisorImagen.SizeMode = PictureBoxSizeMode.Zoom;
        pbVisorImagen.TabIndex = 14;
        pbVisorImagen.TabStop = false;
        // 
        // btnSubirImagen
        // 
        btnSubirImagen.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
        btnSubirImagen.Location = new Point(645, 30);
        btnSubirImagen.Name = "btnSubirImagen";
        btnSubirImagen.Size = new Size(130, 32);
        btnSubirImagen.TabIndex = 15;
        btnSubirImagen.Text = "📎 Adjuntar Fotos";
        btnSubirImagen.UseVisualStyleBackColor = true;
        // 
        // txtDni
        // 
        txtDni.Location = new Point(415, 112);
        txtDni.MaxLength = 8;
        txtDni.Name = "txtDni";
        txtDni.Size = new Size(200, 23);
        txtDni.TabIndex = 9;
        // 
        // txtModelo
        // 
        txtModelo.Location = new Point(115, 70);
        txtModelo.Name = "txtModelo";
        txtModelo.Size = new Size(200, 23);
        txtModelo.TabIndex = 6;
        // 
        // txtTelefono
        // 
        txtTelefono.Location = new Point(415, 70);
        txtTelefono.Name = "txtTelefono";
        txtTelefono.Size = new Size(200, 23);
        txtTelefono.TabIndex = 8;
        // 
        // txtCliente
        // 
        txtCliente.Location = new Point(415, 30);
        txtCliente.Name = "txtCliente";
        txtCliente.Size = new Size(200, 23);
        txtCliente.TabIndex = 5;
        // 
        // txtPlaca
        // 
        txtPlaca.CharacterCasing = CharacterCasing.Upper;
        txtPlaca.Location = new Point(115, 30);
        txtPlaca.MaxLength = 15;
        txtPlaca.Name = "txtPlaca";
        txtPlaca.Size = new Size(200, 23);
        txtPlaca.TabIndex = 4;
        // 
        // lblDni
        // 
        lblDni.AutoSize = true;
        lblDni.Location = new Point(350, 115);
        lblDni.Name = "lblDni";
        lblDni.Size = new Size(30, 15);
        lblDni.TabIndex = 16;
        lblDni.Text = "DNI:";
        // 
        // lblModelo
        // 
        lblModelo.AutoSize = true;
        lblModelo.Location = new Point(15, 73);
        lblModelo.Name = "lblModelo";
        lblModelo.Size = new Size(95, 15);
        lblModelo.TabIndex = 17;
        lblModelo.Text = "Marca / Modelo:";
        // 
        // lblTelefono
        // 
        lblTelefono.AutoSize = true;
        lblTelefono.Location = new Point(350, 73);
        lblTelefono.Name = "lblTelefono";
        lblTelefono.Size = new Size(55, 15);
        lblTelefono.TabIndex = 18;
        lblTelefono.Text = "Teléfono:";
        // 
        // lblCliente
        // 
        lblCliente.AutoSize = true;
        lblCliente.Location = new Point(350, 33);
        lblCliente.Name = "lblCliente";
        lblCliente.Size = new Size(47, 15);
        lblCliente.TabIndex = 19;
        lblCliente.Text = "Cliente:";
        // 
        // lblPlaca
        // 
        lblPlaca.AutoSize = true;
        lblPlaca.Location = new Point(15, 33);
        lblPlaca.Name = "lblPlaca";
        lblPlaca.Size = new Size(38, 15);
        lblPlaca.TabIndex = 20;
        lblPlaca.Text = "Placa:";
        // 
        // lblBuscar
        // 
        lblBuscar.AutoSize = true;
        lblBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBuscar.ForeColor = Color.Blue;
        lblBuscar.Location = new Point(15, 115);
        lblBuscar.Name = "lblBuscar";
        lblBuscar.Size = new Size(47, 15);
        lblBuscar.TabIndex = 21;
        lblBuscar.Text = "Buscar:";
        // 
        // txtBuscar
        // 
        txtBuscar.CharacterCasing = CharacterCasing.Upper;
        txtBuscar.Location = new Point(115, 112);
        txtBuscar.Name = "txtBuscar";
        txtBuscar.Size = new Size(200, 23);
        txtBuscar.TabIndex = 12;
        // 
        // btnRegistrarVehiculo
        // 
        btnRegistrarVehiculo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRegistrarVehiculo.Location = new Point(24, 220);
        btnRegistrarVehiculo.Name = "btnRegistrarVehiculo";
        btnRegistrarVehiculo.Size = new Size(130, 33);
        btnRegistrarVehiculo.TabIndex = 2;
        btnRegistrarVehiculo.Text = "Registrar vehículo";
        btnRegistrarVehiculo.UseVisualStyleBackColor = true;
        btnRegistrarVehiculo.Click += btnRegistrarVehiculo_Click;
        // 
        // btnUpdate
        // 
        btnUpdate.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnUpdate.Location = new Point(165, 220);
        btnUpdate.Name = "btnUpdate";
        btnUpdate.Size = new Size(90, 33);
        btnUpdate.TabIndex = 10;
        btnUpdate.Text = "Update";
        btnUpdate.UseVisualStyleBackColor = true;
        btnUpdate.Click += btnUpdate_Click;
        // 
        // btnDelete
        // 
        btnDelete.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnDelete.Location = new Point(265, 220);
        btnDelete.Name = "btnDelete";
        btnDelete.Size = new Size(90, 33);
        btnDelete.TabIndex = 11;
        btnDelete.Text = "Delete";
        btnDelete.UseVisualStyleBackColor = true;
        btnDelete.Click += btnDelete_Click;
        // 
        // btnRegistroTrabajos
        // 
        btnRegistroTrabajos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRegistroTrabajos.Location = new Point(365, 220);
        btnRegistroTrabajos.Name = "btnRegistroTrabajos";
        btnRegistroTrabajos.Size = new Size(147, 33);
        btnRegistroTrabajos.TabIndex = 3;
        btnRegistroTrabajos.Text = "Registro de trabajos";
        btnRegistroTrabajos.UseVisualStyleBackColor = true;
        btnRegistroTrabajos.Click += btnRegistroTrabajos_Click;
        // 
        // dgvVehiculos
        // 
        dgvVehiculos.AllowUserToAddRows = false;
        dgvVehiculos.AllowUserToDeleteRows = false;
        dgvVehiculos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvVehiculos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvVehiculos.Columns.AddRange(new DataGridViewColumn[] { colIdVehiculo, colPlaca, colCliente, colTelefono, colModelo, colDni, colEstadoVehiculo, colImagenes, colFechaRegistro });
        dgvVehiculos.Location = new Point(24, 265);
        dgvVehiculos.MultiSelect = false;
        dgvVehiculos.Name = "dgvVehiculos";
        dgvVehiculos.ReadOnly = true;
        dgvVehiculos.RowHeadersVisible = false;
        dgvVehiculos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvVehiculos.Size = new Size(1085, 340);
        dgvVehiculos.TabIndex = 4;
        // 
        // colId
        // 
        colIdVehiculo.DataPropertyName = "Id";
        colIdVehiculo.HeaderText = "Id";
        colIdVehiculo.Name = "colId";
        colIdVehiculo.ReadOnly = true;
        // 
        // colPlaca
        // 
        colPlaca.DataPropertyName = "Placa";
        colPlaca.HeaderText = "Placa";
        colPlaca.Name = "colPlaca";
        colPlaca.ReadOnly = true;
        // 
        // colCliente
        // 
        colCliente.DataPropertyName = "Cliente";
        colCliente.HeaderText = "Cliente";
        colCliente.Name = "colCliente";
        colCliente.ReadOnly = true;
        // 
        // colTelefono
        // 
        colTelefono.DataPropertyName = "Telefono";
        colTelefono.HeaderText = "Teléfono";
        colTelefono.Name = "colTelefono";
        colTelefono.ReadOnly = true;
        // 
        // colModelo
        // 
        colModelo.DataPropertyName = "Modelo";
        colModelo.HeaderText = "Modelo";
        colModelo.Name = "colModelo";
        colModelo.ReadOnly = true;
        // 
        // colDni
        // 
        colDni.DataPropertyName = "Dni";
        colDni.HeaderText = "DNI Cliente";
        colDni.Name = "colDni";
        colDni.ReadOnly = true;
        // 
        // colEstadoVehiculo
        // 
        colEstadoVehiculo.DataPropertyName = "Estado";
        colEstadoVehiculo.HeaderText = "Estado Actual";
        colEstadoVehiculo.Name = "colEstadoVehiculo";
        colEstadoVehiculo.ReadOnly = true;
        // 
        // colImagenes
        // 
        colImagenes.HeaderText = "Galería Fotos";
        colImagenes.Name = "colImagenes";
        colImagenes.ReadOnly = true;
        colImagenes.Text = "Ver fotos 📷";
        colImagenes.UseColumnTextForButtonValue = true;
        // 
        // colFechaRegistro
        // 
        colFechaRegistro.DataPropertyName = "FechaRegistro";
        dataGridViewCellStyle2.Format = "dd/MM/yyyy hh:mm tt";
        colFechaRegistro.DefaultCellStyle = dataGridViewCellStyle2;
        colFechaRegistro.HeaderText = "Fecha de Ingreso";
        colFechaRegistro.Name = "colFechaRegistro";
        colFechaRegistro.ReadOnly = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1134, 631);
        Controls.Add(btnDelete);
        Controls.Add(btnUpdate);
        Controls.Add(dgvVehiculos);
        Controls.Add(btnRegistroTrabajos);
        Controls.Add(btnRegistrarVehiculo);
        Controls.Add(gbRecepcion);
        Controls.Add(lblTitulo);
        MinimumSize = new Size(1150, 670);
        Name = "Form1";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Gestión de Taller Mecánico - Mecha Prime";
        Load += Form1_Load;
        gbRecepcion.ResumeLayout(false);
        gbRecepcion.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pbVisorImagen).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvVehiculos).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }


    #endregion

    // ============================================================
    // DECLARACIÓN INTERNA DE VARIABLES DE CONTROLES
    // ============================================================
    private Label lblTitulo;
    private GroupBox gbRecepcion;
    private TextBox txtDni;
    private TextBox txtModelo;
    private TextBox txtTelefono;
    private TextBox txtCliente;
    private TextBox txtPlaca;
    private Label lblDni;
    private Label lblModelo;
    private Label lblTelefono;
    private Label lblCliente;
    private Label lblPlaca;
    private Button btnRegistrarVehiculo;
    private Button btnUpdate;
    private Button btnDelete;
    private Button btnRegistroTrabajos;
    private DataGridView dgvVehiculos;

    private DataGridViewTextBoxColumn colIdVehiculo;   // Borrar linea si falla
    private DataGridViewTextBoxColumn colPlaca;
    private DataGridViewTextBoxColumn colCliente;
    private DataGridViewTextBoxColumn colModelo;
    private DataGridViewTextBoxColumn colDni;

    // 4. VARIABLE DECLARADA CORRECTAMENTE AL FINAL JUNTO A LAS OTRAS COLUMNAS
    private DataGridViewTextBoxColumn colEstadoVehiculo;

    private DataGridViewTextBoxColumn colFechaRegistro;
    private DataGridViewTextBoxColumn colTelefono;

    // DECLARACIONES AL FINAL DE LOS NUEVOS COMPONENTES MULTIMEDIA
    private DataGridViewButtonColumn colImagenes;
    private PictureBox pbVisorImagen;
    private Button btnSubirImagen;
    private OpenFileDialog openFileDialogImagenes;

    // DECLARACIONES AL FINAL DE LAS VARIABLES DEL BUSCADOR
    private Label lblBuscar;
    private TextBox txtBuscar;
}