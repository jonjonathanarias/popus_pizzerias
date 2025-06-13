namespace popus_pizzeria.Model
{
    partial class frmArqueoCaja
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
            this.lblFecha = new System.Windows.Forms.Label();
            this.lblDinIn = new System.Windows.Forms.Label();
            this.lblTakeAway = new System.Windows.Forms.Label();
            this.lblDelivery = new System.Windows.Forms.Label();
            this.lblTotalGeneral = new System.Windows.Forms.Label();
            this.btnCerrarArqueo = new Guna.UI2.WinForms.Guna2Button();
            this.SuspendLayout();
            // 
            // lblFecha
            // 
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(166, 58);
            this.lblFecha.Name = "lblFecha";
            this.lblFecha.Size = new System.Drawing.Size(44, 16);
            this.lblFecha.TabIndex = 1;
            this.lblFecha.Text = "label2";
            // 
            // lblDinIn
            // 
            this.lblDinIn.AutoSize = true;
            this.lblDinIn.Location = new System.Drawing.Point(166, 104);
            this.lblDinIn.Name = "lblDinIn";
            this.lblDinIn.Size = new System.Drawing.Size(44, 16);
            this.lblDinIn.TabIndex = 1;
            this.lblDinIn.Text = "label2";
            this.lblDinIn.Click += new System.EventHandler(this.label3_Click);
            // 
            // lblTakeAway
            // 
            this.lblTakeAway.AutoSize = true;
            this.lblTakeAway.Location = new System.Drawing.Point(166, 160);
            this.lblTakeAway.Name = "lblTakeAway";
            this.lblTakeAway.Size = new System.Drawing.Size(44, 16);
            this.lblTakeAway.TabIndex = 1;
            this.lblTakeAway.Text = "label2";
            // 
            // lblDelivery
            // 
            this.lblDelivery.AutoSize = true;
            this.lblDelivery.Location = new System.Drawing.Point(166, 205);
            this.lblDelivery.Name = "lblDelivery";
            this.lblDelivery.Size = new System.Drawing.Size(44, 16);
            this.lblDelivery.TabIndex = 1;
            this.lblDelivery.Text = "label2";
            // 
            // lblTotalGeneral
            // 
            this.lblTotalGeneral.AutoSize = true;
            this.lblTotalGeneral.Location = new System.Drawing.Point(166, 240);
            this.lblTotalGeneral.Name = "lblTotalGeneral";
            this.lblTotalGeneral.Size = new System.Drawing.Size(44, 16);
            this.lblTotalGeneral.TabIndex = 1;
            this.lblTotalGeneral.Text = "label2";
            // 
            // btnCerrarArqueo
            // 
            this.btnCerrarArqueo.AutoRoundedCorners = true;
            this.btnCerrarArqueo.BackColor = System.Drawing.Color.Transparent;
            this.btnCerrarArqueo.BorderRadius = 21;
            this.btnCerrarArqueo.CustomizableEdges.TopRight = false;
            this.btnCerrarArqueo.DisabledState.BorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarArqueo.DisabledState.CustomBorderColor = System.Drawing.Color.DarkGray;
            this.btnCerrarArqueo.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(169)))), ((int)(((byte)(169)))), ((int)(((byte)(169)))));
            this.btnCerrarArqueo.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(141)))), ((int)(((byte)(141)))), ((int)(((byte)(141)))));
            this.btnCerrarArqueo.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(123)))), ((int)(((byte)(194)))), ((int)(((byte)(96)))));
            this.btnCerrarArqueo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnCerrarArqueo.ForeColor = System.Drawing.Color.White;
            this.btnCerrarArqueo.Location = new System.Drawing.Point(169, 296);
            this.btnCerrarArqueo.Name = "btnCerrarArqueo";
            this.btnCerrarArqueo.Size = new System.Drawing.Size(229, 45);
            this.btnCerrarArqueo.TabIndex = 4;
            this.btnCerrarArqueo.Text = "Cerrar Aqueo de Caja";
            this.btnCerrarArqueo.Click += new System.EventHandler(this.btnCerrarArqueo_Click);
            // 
            // frmArqueoCaja
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCerrarArqueo);
            this.Controls.Add(this.lblTotalGeneral);
            this.Controls.Add(this.lblDelivery);
            this.Controls.Add(this.lblTakeAway);
            this.Controls.Add(this.lblDinIn);
            this.Controls.Add(this.lblFecha);
            this.Name = "frmArqueoCaja";
            this.Text = "frmArqueoCaja";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.lblFecha, 0);
            this.Controls.SetChildIndex(this.lblDinIn, 0);
            this.Controls.SetChildIndex(this.lblTakeAway, 0);
            this.Controls.SetChildIndex(this.lblDelivery, 0);
            this.Controls.SetChildIndex(this.lblTotalGeneral, 0);
            this.Controls.SetChildIndex(this.btnCerrarArqueo, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.Label lblDinIn;
        private System.Windows.Forms.Label lblTakeAway;
        private System.Windows.Forms.Label lblDelivery;
        private System.Windows.Forms.Label lblTotalGeneral;
        protected Guna.UI2.WinForms.Guna2Button btnCerrarArqueo;
    }
}