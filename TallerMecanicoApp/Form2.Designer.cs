using Microsoft.VisualBasic.Logging;
using System.ComponentModel.Design;
using System.Numerics;

namespace TallerMecanicoApp;

partial class Form2
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>

    private void InitializeComponent()
    {
        lblTitulo = new Label();
        gbRegistro = new GroupBox();
        lblResumenServicio = new Label();
        lblTiempoEstimado = new Label();
        lblTotal = new Label();
        chkBujias = new CheckBox();
        chkLiquidoFrenos = new CheckBox();
        chkRefrigerante = new CheckBox();
        btnRegistrarTrabajo = new Button();
        cbEstado = new ComboBox();
        cbMecanico = new ComboBox();
        cbServicio = new ComboBox();
        txtDescripcion = new TextBox();
        cbPlaca = new ComboBox();
        lblEstado = new Label();
        lblMecanico = new Label();
        lblServicio = new Label();
        lblDescripcion = new Label();
        lblPlaca = new Label();
        btnUpdateTrabajo = new Button();
        btnDeleteTrabajo = new Button();
        lblBuscarTrabajo = new Label();
        txtBuscarTrabajo = new TextBox();
        dgvTrabajos = new DataGridView();
        colIdTrabajo = new DataGridViewTextBoxColumn();
        colIdVehiculo = new DataGridViewTextBoxColumn();
        colPlaca = new DataGridViewTextBoxColumn();
        colServicio = new DataGridViewTextBoxColumn();
        colDescripcion = new DataGridViewTextBoxColumn();
        colEstado = new DataGridViewTextBoxColumn();
        colMecanico = new DataGridViewTextBoxColumn();
        colGaleriaFotos = new DataGridViewButtonColumn();
        colAdicionales = new DataGridViewTextBoxColumn();
        colTotal = new DataGridViewTextBoxColumn();
        colTiempo = new DataGridViewTextBoxColumn();
        btnAdjuntarFotos = new Button();
        pbVehiculoTrabajo = new PictureBox();
        gbRegistro.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrabajos).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pbVehiculoTrabajo).BeginInit();
        SuspendLayout();
        // 
        // lblTitulo
        // 
        lblTitulo.AutoSize = true;
        lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        lblTitulo.Location = new Point(20, 18);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(266, 25);
        lblTitulo.TabIndex = 0;
        lblTitulo.Text = "Registro de trabajos y costos";
        // 
        // gbRegistro
        // 
        gbRegistro.Controls.Add(lblResumenServicio);
        gbRegistro.Controls.Add(lblTiempoEstimado);
        gbRegistro.Controls.Add(lblTotal);
        gbRegistro.Controls.Add(chkBujias);
        gbRegistro.Controls.Add(chkLiquidoFrenos);
        gbRegistro.Controls.Add(chkRefrigerante);
        gbRegistro.Controls.Add(btnRegistrarTrabajo);
        gbRegistro.Controls.Add(cbEstado);
        gbRegistro.Controls.Add(cbMecanico);
        gbRegistro.Controls.Add(cbServicio);
        gbRegistro.Controls.Add(txtDescripcion);
        gbRegistro.Controls.Add(cbPlaca);
        gbRegistro.Controls.Add(lblEstado);
        gbRegistro.Controls.Add(lblMecanico);
        gbRegistro.Controls.Add(lblServicio);
        gbRegistro.Controls.Add(lblDescripcion);
        gbRegistro.Controls.Add(lblPlaca);
        gbRegistro.Controls.Add(btnUpdateTrabajo);
        gbRegistro.Controls.Add(btnDeleteTrabajo);
        gbRegistro.Controls.Add(lblBuscarTrabajo);
        gbRegistro.Controls.Add(txtBuscarTrabajo);
        gbRegistro.Location = new Point(20, 56);
        gbRegistro.Name = "gbRegistro";
        gbRegistro.Size = new Size(760, 230); // Compactado para dar espacio al PictureBox
        gbRegistro.TabIndex = 1;
        gbRegistro.TabStop = false;
        gbRegistro.Text = "Detalle del trabajo";
        // 
        // lblResumenServicio
        // 
        lblResumenServicio.AutoSize = true;
        lblResumenServicio.Location = new Point(24, 192);
        lblResumenServicio.Name = "lblResumenServicio";
        lblResumenServicio.Size = new Size(127, 15);
        lblResumenServicio.TabIndex = 14;
        lblResumenServicio.Text = "Resumen del servicio...";
        // 
        // lblTiempoEstimado
        // 
        lblTiempoEstimado.AutoSize = true;
        lblTiempoEstimado.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTiempoEstimado.Location = new Point(445, 192);
        lblTiempoEstimado.Name = "lblTiempoEstimado";
        lblTiempoEstimado.Size = new Size(123, 15);
        lblTiempoEstimado.TabIndex = 13;
        lblTiempoEstimado.Text = "Tiempo estimado: 0h";
        // 
        // lblTotal
        // 
        lblTotal.AutoSize = true;
        lblTotal.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblTotal.Location = new Point(445, 170);
        lblTotal.Name = "lblTotal";
        lblTotal.Size = new Size(76, 15);
        lblTotal.TabIndex = 12;
        lblTotal.Text = "Total: S/0.00";
        // 
        // chkBujias
        // 
        chkBujias.AutoSize = true;
        chkBujias.Location = new Point(445, 115);
        chkBujias.Name = "chkBujias";
        chkBujias.Size = new Size(240, 19);
        chkBujias.TabIndex = 11;
        chkBujias.Text = "Cambio de Bujías (+S/40 y +10 min, S/R)";
        chkBujias.UseVisualStyleBackColor = true;
        chkBujias.CheckedChanged += chkAdicional_CheckedChanged;
        // 
        // chkLiquidoFrenos
        // 
        chkLiquidoFrenos.AutoSize = true;
        chkLiquidoFrenos.Location = new Point(445, 90);
        chkLiquidoFrenos.Name = "chkLiquidoFrenos";
        chkLiquidoFrenos.Size = new Size(223, 19);
        chkLiquidoFrenos.TabIndex = 10;
        chkLiquidoFrenos.Text = "Cambio de Líquido de Frenos (+S/50)";
        chkLiquidoFrenos.UseVisualStyleBackColor = true;
        chkLiquidoFrenos.CheckedChanged += chkAdicional_CheckedChanged;
        // 
        // chkRefrigerante
        // 
        chkRefrigerante.AutoSize = true;
        chkRefrigerante.Location = new Point(445, 65);
        chkRefrigerante.Name = "chkRefrigerante";
        chkRefrigerante.Size = new Size(193, 19);
        chkRefrigerante.TabIndex = 9;
        chkRefrigerante.Text = "Cambio de Refrigerante (+S/90)";
        chkRefrigerante.UseVisualStyleBackColor = true;
        chkRefrigerante.CheckedChanged += chkAdicional_CheckedChanged;
        // 
        // btnRegistrarTrabajo
        // 
        btnRegistrarTrabajo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnRegistrarTrabajo.Location = new Point(615, 140);
        btnRegistrarTrabajo.Name = "btnRegistrarTrabajo";
        btnRegistrarTrabajo.Size = new Size(130, 26);
        btnRegistrarTrabajo.TabIndex = 8;
        btnRegistrarTrabajo.Text = "Registrar trabajo";
        btnRegistrarTrabajo.UseVisualStyleBackColor = true;
        btnRegistrarTrabajo.Click += btnRegistrarTrabajo_Click;
        // 
        // cbEstado
        // 
        cbEstado.DropDownStyle = ComboBoxStyle.DropDownList;
        cbEstado.FormattingEnabled = true;
        cbEstado.Location = new Point(99, 112);
        cbEstado.Name = "cbEstado";
        cbEstado.Size = new Size(260, 23);
        cbEstado.TabIndex = 7;
        // 
        // cbMecanico
        // 
        cbMecanico.DropDownStyle = ComboBoxStyle.DropDownList;
        cbMecanico.FormattingEnabled = true;
        cbMecanico.Location = new Point(445, 140);
        cbMecanico.Name = "cbMecanico";
        cbMecanico.Size = new Size(160, 23);
        cbMecanico.TabIndex = 15;
        // 
        // cbServicio
        // 
        cbServicio.DropDownStyle = ComboBoxStyle.DropDownList;
        cbServicio.FormattingEnabled = true;
        cbServicio.Location = new Point(99, 76);
        cbServicio.Name = "cbServicio";
        cbServicio.Size = new Size(260, 23);
        cbServicio.TabIndex = 6;
        cbServicio.SelectedIndexChanged += cbServicio_SelectedIndexChanged;
        // 
        // txtDescripcion
        // 
        txtDescripcion.Location = new Point(445, 32);
        txtDescripcion.Name = "txtDescripcion";
        txtDescripcion.Size = new Size(300, 23); // Ajustado para evitar colisión derecha
        txtDescripcion.TabIndex = 5;
        // 
        // cbPlaca
        // 
        cbPlaca.DropDownStyle = ComboBoxStyle.DropDownList;
        cbPlaca.FormattingEnabled = true;
        cbPlaca.Location = new Point(99, 40);
        cbPlaca.Name = "cbPlaca";
        cbPlaca.Size = new Size(260, 23);
        cbPlaca.TabIndex = 4;
        // 
        // lblEstado
        // 
        lblEstado.AutoSize = true;
        lblEstado.Location = new Point(49, 115);
        lblEstado.Name = "lblEstado";
        lblEstado.Size = new Size(45, 15);
        lblEstado.TabIndex = 3;
        lblEstado.Text = "Estado:";
        // 
        // lblMecanico
        // 
        lblMecanico.AutoSize = true;
        lblMecanico.Location = new Point(377, 143);
        lblMecanico.Name = "lblMecanico";
        lblMecanico.Size = new Size(62, 15);
        lblMecanico.TabIndex = 16;
        lblMecanico.Text = "Mecánico:";
        // 
        // lblServicio
        // 
        lblServicio.AutoSize = true;
        lblServicio.Location = new Point(45, 79);
        lblServicio.Name = "lblServicio";
        lblServicio.Size = new Size(51, 15);
        lblServicio.TabIndex = 2;
        lblServicio.Text = "Servicio:";
        // 
        // lblDescripcion
        // 
        lblDescripcion.AutoSize = true;
        lblDescripcion.Location = new Point(367, 35);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(72, 15);
        lblDescripcion.TabIndex = 1;
        lblDescripcion.Text = "Descripción:";
        // 
        // lblPlaca
        // 
        lblPlaca.AutoSize = true;
        lblPlaca.Location = new Point(52, 43);
        lblPlaca.Name = "lblPlaca";
        lblPlaca.Size = new Size(38, 15);
        lblPlaca.TabIndex = 0;
        lblPlaca.Text = "Placa:";
        // 
        // btnUpdateTrabajo
        // 
        btnUpdateTrabajo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnUpdateTrabajo.Location = new Point(615, 169);
        btnUpdateTrabajo.Name = "btnUpdateTrabajo";
        btnUpdateTrabajo.Size = new Size(130, 26);
        btnUpdateTrabajo.TabIndex = 17;
        btnUpdateTrabajo.Text = "Update";
        btnUpdateTrabajo.UseVisualStyleBackColor = true;
        btnUpdateTrabajo.Click += btnUpdateTrabajo_Click;
        // 
        // btnDeleteTrabajo
        // 
        btnDeleteTrabajo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnDeleteTrabajo.Location = new Point(615, 198);
        btnDeleteTrabajo.Name = "btnDeleteTrabajo";
        btnDeleteTrabajo.Size = new Size(130, 26);
        btnDeleteTrabajo.TabIndex = 18;
        btnDeleteTrabajo.Text = "Delete";
        btnDeleteTrabajo.UseVisualStyleBackColor = true;
        btnDeleteTrabajo.Click += btnDeleteTrabajo_Click;
        // 
        // lblBuscarTrabajo
        // 
        lblBuscarTrabajo.AutoSize = true;
        lblBuscarTrabajo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBuscarTrabajo.ForeColor = Color.Blue;
        lblBuscarTrabajo.Location = new Point(46, 151);
        lblBuscarTrabajo.Name = "lblBuscarTrabajo";
        lblBuscarTrabajo.Size = new Size(47, 15);
        lblBuscarTrabajo.TabIndex = 19;
        lblBuscarTrabajo.Text = "Buscar:";
        // 
        // txtBuscarTrabajo
        // 
        txtBuscarTrabajo.Location = new Point(99, 148);
        txtBuscarTrabajo.Name = "txtBuscarTrabajo";
        txtBuscarTrabajo.Size = new Size(260, 23);
        txtBuscarTrabajo.TabIndex = 20;
        // 
        // pbVehiculoTrabajo (PictureBox)
        // 
        pbVehiculoTrabajo.BorderStyle = BorderStyle.FixedSingle;
        pbVehiculoTrabajo.Location = new Point(795, 65); // Alineado a la derecha del GroupBox
        pbVehiculoTrabajo.Name = "pbVehiculoTrabajo";
        pbVehiculoTrabajo.Size = new Size(260, 180); // Proporcional al panel de Form1
        pbVehiculoTrabajo.SizeMode = PictureBoxSizeMode.Zoom;
        pbVehiculoTrabajo.TabStop = false;
        // 
        // btnAdjuntarFotos 
        // 
        btnAdjuntarFotos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnAdjuntarFotos.Location = new Point(795, 252); // Posicionado justo debajo del PictureBox
        btnAdjuntarFotos.Name = "btnAdjuntarFotos";
        btnAdjuntarFotos.Size = new Size(260, 34);
        btnAdjuntarFotos.Text = "📷 Adjuntar Fotos";
        btnAdjuntarFotos.UseVisualStyleBackColor = true;
        btnAdjuntarFotos.Click += btnAdjuntarFotos_Click;
        // 
        // dgvTrabajos
        // 
        dgvTrabajos.AllowUserToAddRows = false;
        dgvTrabajos.AllowUserToDeleteRows = false;
        dgvTrabajos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvTrabajos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvTrabajos.Columns.AddRange(new DataGridViewColumn[] { colIdTrabajo, colIdVehiculo, colPlaca, colServicio, colDescripcion, colEstado, colMecanico, colGaleriaFotos, colAdicionales, colTotal, colTiempo });
        dgvTrabajos.Location = new Point(20, 302);
        dgvTrabajos.Name = "dgvTrabajos";
        dgvTrabajos.ReadOnly = true;
        dgvTrabajos.RowHeadersVisible = false;
        dgvTrabajos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTrabajos.Size = new Size(1038, 265);
        dgvTrabajos.TabIndex = 2;
        // 
        // colIdTrabajo
        // 
        colIdTrabajo.DataPropertyName = "idTrabajo";
        colIdTrabajo.HeaderText = "idTrabajo";
        colIdTrabajo.Name = "colIdTrabajo";
        colIdTrabajo.ReadOnly = true;
        colIdTrabajo.Width = 80;
        // 
        // colIdVehiculo
        // 
        colIdVehiculo.DataPropertyName = "idVehiculo";
        colIdVehiculo.HeaderText = "idVehiculo";
        colIdVehiculo.Name = "colIdVehiculo";
        colIdVehiculo.ReadOnly = true;
        colIdVehiculo.Width = 85;
        // 
        // colPlaca
        // 
        colPlaca.DataPropertyName = "Placa";
        colPlaca.HeaderText = "Placa";
        colPlaca.Name = "colPlaca";
        colPlaca.ReadOnly = true;
        colPlaca.Width = 90;
        // 
        // colServicio
        // 
        colServicio.DataPropertyName = "ServicioNombre";
        colServicio.HeaderText = "Servicio";
        colServicio.Name = "colServicio";
        colServicio.ReadOnly = true;
        colServicio.Width = 160;
        // 
        // colDescripcion
        // 
        colDescripcion.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        colDescripcion.DataPropertyName = "Descripcion";
        colDescripcion.HeaderText = "Descripción";
        colDescripcion.Name = "colDescripcion";
        colDescripcion.ReadOnly = true;
        // 
        // colEstado
        // 
        colEstado.DataPropertyName = "Estado";
        colEstado.HeaderText = "Estado";
        colEstado.Name = "colEstado";
        colEstado.ReadOnly = true;
        colEstado.Width = 100;
        // 
        // colMecanico
        // 
        colMecanico.DataPropertyName = "Mecanico";
        colMecanico.HeaderText = "Mecánico";
        colMecanico.Name = "colMecanico";
        colMecanico.ReadOnly = true;
        colMecanico.Width = 120;
        // 
        // colGaleriaFotos
        // 
        colGaleriaFotos.HeaderText = "Galería Fotos";
        colGaleriaFotos.Name = "colGaleriaFotos";
        colGaleriaFotos.Text = "Ver fotos 📷";
        colGaleriaFotos.UseColumnTextForButtonValue = true;
        colGaleriaFotos.Width = 110;
        colGaleriaFotos.ReadOnly = true;
        // 
        // colAdicionales
        // 
        colAdicionales.DataPropertyName = "AdicionalesTexto";
        colAdicionales.HeaderText = "Adicionales";
        colAdicionales.Name = "colAdicionales";
        colAdicionales.ReadOnly = true;
        colAdicionales.Width = 140;
        // 
        // colTotal
        // 
        colTotal.DataPropertyName = "TotalPagar";
        colTotal.HeaderText = "Total (S/)";
        colTotal.Name = "colTotal";
        colTotal.ReadOnly = true;
        colTotal.Width = 90;
        // 
        // colTiempo
        // 
        colTiempo.DataPropertyName = "TiempoEstimadoTexto";
        colTiempo.HeaderText = "Tiempo estimado";
        colTiempo.Name = "colTiempo";
        colTiempo.ReadOnly = true;
        colTiempo.Width = 125;
        // 
        // Form2
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1078, 582);
        Controls.Add(dgvTrabajos);
        Controls.Add(gbRegistro);
        Controls.Add(lblTitulo);
        Controls.Add(pbVehiculoTrabajo);
        Controls.Add(btnAdjuntarFotos);
        MinimumSize = new Size(1094, 621);
        Name = "Form2";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Registro de Trabajos";
        Load += Form2_Load;
        gbRegistro.ResumeLayout(false);
        gbRegistro.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrabajos).EndInit();
        ((System.ComponentModel.ISupportInitialize)pbVehiculoTrabajo).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private Label lblTitulo;
    private GroupBox gbRegistro;
    private Label lblResumenServicio;
    private Label lblTiempoEstimado;
    private Label lblTotal;
    private CheckBox chkBujias;
    private CheckBox chkLiquidoFrenos;
    private CheckBox chkRefrigerante;
    private Button btnRegistrarTrabajo;
    private Button btnAdjuntarFotos;
    private ComboBox cbEstado;
    private ComboBox cbMecanico;
    private ComboBox cbServicio;
    private TextBox txtDescripcion;
    private ComboBox cbPlaca;
    private Label lblEstado;
    private Label lblMecanico;
    private Label lblServicio;
    private Label lblDescripcion;
    private Label lblPlaca;
    private DataGridView dgvTrabajos;
    private DataGridViewTextBoxColumn colIdTrabajo;
    private DataGridViewTextBoxColumn colIdVehiculo;
    private DataGridViewTextBoxColumn colPlaca;
    private DataGridViewTextBoxColumn colServicio;
    private DataGridViewTextBoxColumn colDescripcion;
    private DataGridViewTextBoxColumn colEstado;
    private DataGridViewTextBoxColumn colMecanico;
    private DataGridViewTextBoxColumn colAdicionales;
    private DataGridViewTextBoxColumn colTotal;
    private DataGridViewTextBoxColumn colTiempo;
    private DataGridViewButtonColumn colGaleriaFotos;

    // DEFINICIONES DE LOS NUEVOS CONTROLES
    private Button btnUpdateTrabajo;
    private Button btnDeleteTrabajo;
    private Label lblBuscarTrabajo;
    private TextBox txtBuscarTrabajo;


    private PictureBox pbVehiculoTrabajo;
}
