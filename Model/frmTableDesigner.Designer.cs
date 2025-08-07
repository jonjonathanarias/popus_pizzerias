namespace popus_pizzeria.Model
{
    partial class frmTableDesigner
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelPlano = new Guna.UI2.WinForms.Guna2Panel();
            this.btnAgregarMesa = new Guna.UI2.WinForms.Guna2Button();
            this.btnGuardarPosiciones = new Guna.UI2.WinForms.Guna2Button();
            this.btnEliminarMesa = new Guna.UI2.WinForms.Guna2Button();
            this.guna2MessageDialog1 = new Guna.UI2.WinForms.Guna2MessageDialog();
            this.SuspendLayout();
            // 
            // panelPlano
            // 
            this.panelPlano.BackColor = System.Drawing.Color.White;
            this.panelPlano.Location = new System.Drawing.Point(0, 0);
            this.panelPlano.Name = "panelPlano";
            this.panelPlano.Size = new System.Drawing.Size(1404, 532);
            this.panelPlano.TabIndex = 0;
            // 
            // btnAgregarMesa
            // 
            this.btnAgregarMesa.AutoRoundedCorners = true;
            this.btnAgregarMesa.BackColor = System.Drawing.Color.Transparent;
            this.btnAgregarMesa.BorderRadius = 21;
            this.btnAgregarMesa.CustomizableEdges.TopRight = false;
            this.btnAgregarMesa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnAgregarMesa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnAgregarMesa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnAgregarMesa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnAgregarMesa.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(194)))), ((int)(((byte)(96)))));
            this.btnAgregarMesa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnAgregarMesa.ForeColor = System.Drawing.Color.White;
            this.btnAgregarMesa.Location = new System.Drawing.Point(321, 538);
            this.btnAgregarMesa.Name = "btnAgregarMesa";
            this.btnAgregarMesa.Size = new System.Drawing.Size(160, 45);
            this.btnAgregarMesa.TabIndex = 2;
            this.btnAgregarMesa.Text = "Agregar Mesa";
            this.btnAgregarMesa.Click += new System.EventHandler(this.btnAgregarMesa_Click_1);
            // 
            // btnGuardarPosiciones
            // 
            this.btnGuardarPosiciones.AutoRoundedCorners = true;
            this.btnGuardarPosiciones.BackColor = System.Drawing.Color.Transparent;
            this.btnGuardarPosiciones.BorderRadius = 21;
            this.btnGuardarPosiciones.CustomizableEdges.TopRight = false;
            this.btnGuardarPosiciones.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardarPosiciones.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnGuardarPosiciones.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnGuardarPosiciones.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnGuardarPosiciones.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(246)))), ((int)(((byte)(169)))), ((int)(((byte)(43)))));
            this.btnGuardarPosiciones.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnGuardarPosiciones.ForeColor = System.Drawing.Color.White;
            this.btnGuardarPosiciones.Location = new System.Drawing.Point(653, 538);
            this.btnGuardarPosiciones.Name = "btnGuardarPosiciones";
            this.btnGuardarPosiciones.Size = new System.Drawing.Size(160, 45);
            this.btnGuardarPosiciones.TabIndex = 3;
            this.btnGuardarPosiciones.Text = "Guardar Posiciones";
            this.btnGuardarPosiciones.Click += new System.EventHandler(this.btnGuardarPosiciones_Click_1);
            // 
            // btnEliminarMesa
            // 
            this.btnEliminarMesa.AutoRoundedCorners = true;
            this.btnEliminarMesa.BackColor = System.Drawing.Color.Transparent;
            this.btnEliminarMesa.BorderRadius = 21;
            this.btnEliminarMesa.CustomizableEdges.TopRight = false;
            this.btnEliminarMesa.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminarMesa.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnEliminarMesa.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnEliminarMesa.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnEliminarMesa.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(236)))), ((int)(((byte)(50)))), ((int)(((byte)(55)))));
            this.btnEliminarMesa.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnEliminarMesa.ForeColor = System.Drawing.Color.White;
            this.btnEliminarMesa.Location = new System.Drawing.Point(487, 538);
            this.btnEliminarMesa.Name = "btnEliminarMesa";
            this.btnEliminarMesa.Size = new System.Drawing.Size(160, 45);
            this.btnEliminarMesa.TabIndex = 2;
            this.btnEliminarMesa.Text = "Eliminar Mesa";
            this.btnEliminarMesa.Click += new System.EventHandler(this.btnEliminarMesa_Click);
            // 
            // guna2MessageDialog1
            // 
            this.guna2MessageDialog1.Buttons = Guna.UI2.WinForms.MessageDialogButtons.OK;
            this.guna2MessageDialog1.Caption = "popus_pizzeria";
            this.guna2MessageDialog1.Icon = Guna.UI2.WinForms.MessageDialogIcon.Information;
            this.guna2MessageDialog1.Parent = null;
            this.guna2MessageDialog1.Style = Guna.UI2.WinForms.MessageDialogStyle.Light;
            this.guna2MessageDialog1.Text = null;
            // 
            // frmTableDesigner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1254, 569);
            this.Controls.Add(this.btnEliminarMesa);
            this.Controls.Add(this.btnAgregarMesa);
            this.Controls.Add(this.btnGuardarPosiciones);
            this.Controls.Add(this.panelPlano);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "frmTableDesigner";
            this.Text = "frmTableDesigner";
            this.Load += new System.EventHandler(this.frmTableDesigner_Load_1);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel panelPlano;
        protected Guna.UI2.WinForms.Guna2Button btnAgregarMesa;
        protected Guna.UI2.WinForms.Guna2Button btnGuardarPosiciones;
        protected Guna.UI2.WinForms.Guna2Button btnEliminarMesa;
        private Guna.UI2.WinForms.Guna2MessageDialog guna2MessageDialog1;
    }
}