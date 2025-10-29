namespace popus_pizzeria.Model
{
    partial class Paymentform
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
            this.txtTotal = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtRecibido = new Guna.UI2.WinForms.Guna2TextBox();
            this.txtCambio = new System.Windows.Forms.Label();
            this.cambio = new System.Windows.Forms.Label();
            this.cmbPaymentMethod = new Guna.UI2.WinForms.Guna2ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtTotal
            // 
            this.txtTotal.AutoSize = true;
            this.txtTotal.Location = new System.Drawing.Point(230, 167);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(41, 23);
            this.txtTotal.TabIndex = 1;
            this.txtTotal.Text = "0.00";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(54, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Total :";
            // 
            // txtRecibido
            // 
            this.txtRecibido.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtRecibido.DefaultText = "";
            this.txtRecibido.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtRecibido.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtRecibido.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRecibido.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtRecibido.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRecibido.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtRecibido.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtRecibido.Location = new System.Drawing.Point(234, 247);
            this.txtRecibido.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtRecibido.Name = "txtRecibido";
            this.txtRecibido.PasswordChar = '\0';
            this.txtRecibido.PlaceholderText = "";
            this.txtRecibido.SelectedText = "";
            this.txtRecibido.Size = new System.Drawing.Size(229, 35);
            this.txtRecibido.TabIndex = 3;
            this.txtRecibido.TextChanged += new System.EventHandler(this.txtRecibido_TextChanged);
            // 
            // txtCambio
            // 
            this.txtCambio.AutoSize = true;
            this.txtCambio.Location = new System.Drawing.Point(230, 307);
            this.txtCambio.Name = "txtCambio";
            this.txtCambio.Size = new System.Drawing.Size(0, 23);
            this.txtCambio.TabIndex = 4;
            // 
            // cambio
            // 
            this.cambio.AutoSize = true;
            this.cambio.Location = new System.Drawing.Point(54, 307);
            this.cambio.Name = "cambio";
            this.cambio.Size = new System.Drawing.Size(55, 23);
            this.cambio.TabIndex = 4;
            this.cambio.Text = "Total :";
            // 
            // cmbPaymentMethod
            // 
            this.cmbPaymentMethod.BackColor = System.Drawing.Color.Transparent;
            this.cmbPaymentMethod.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPaymentMethod.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbPaymentMethod.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.cmbPaymentMethod.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbPaymentMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(68)))), ((int)(((byte)(88)))), ((int)(((byte)(112)))));
            this.cmbPaymentMethod.ItemHeight = 30;
            this.cmbPaymentMethod.Items.AddRange(new object[] {
            "Efectivo",
            "Transferencia",
            "QR",
            "Tarjeta de Débito",
            "Tarjeta de Crédito"});
            this.cmbPaymentMethod.Location = new System.Drawing.Point(234, 204);
            this.cmbPaymentMethod.Name = "cmbPaymentMethod";
            this.cmbPaymentMethod.Size = new System.Drawing.Size(229, 36);
            this.cmbPaymentMethod.TabIndex = 12;
            this.cmbPaymentMethod.SelectedIndexChanged += new System.EventHandler(this.cmbPaymentMethod_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(54, 260);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 23);
            this.label4.TabIndex = 13;
            this.label4.Text = "Paga con :";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(54, 217);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(147, 23);
            this.label5.TabIndex = 14;
            this.label5.Text = "Metodo de pago :";
            // 
            // Paymentform
            // 
            this.ClientSize = new System.Drawing.Size(475, 454);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmbPaymentMethod);
            this.Controls.Add(this.cambio);
            this.Controls.Add(this.txtCambio);
            this.Controls.Add(this.txtRecibido);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTotal);
            this.Name = "Paymentform";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        public Guna.UI2.WinForms.Guna2TextBox txtNombre;
        private System.Windows.Forms.Label txtTotal;
        private System.Windows.Forms.Label label3;
        private Guna.UI2.WinForms.Guna2TextBox txtRecibido;
        private System.Windows.Forms.Label txtCambio;
        private System.Windows.Forms.Label cambio;
        public Guna.UI2.WinForms.Guna2ComboBox cmbPaymentMethod;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
    }
}