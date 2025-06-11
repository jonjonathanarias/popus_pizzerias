namespace popus_pizzeria.Model
{
    partial class frmObservation
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtObservation = new Guna.UI2.WinForms.Guna2TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Size = new System.Drawing.Size(207, 23);
            this.label1.Text = "Observacion del producto";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(122, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(186, 23);
            this.label2.TabIndex = 1;
            this.label2.Text = "Agregar Observaciones";
            // 
            // txtObservation
            // 
            this.txtObservation.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtObservation.DefaultText = "";
            this.txtObservation.DisabledState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(208)))), ((int)(((byte)(208)))));
            this.txtObservation.DisabledState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(226)))), ((int)(((byte)(226)))));
            this.txtObservation.DisabledState.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtObservation.DisabledState.PlaceholderForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(138)))), ((int)(((byte)(138)))), ((int)(((byte)(138)))));
            this.txtObservation.FocusedState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtObservation.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtObservation.HoverState.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(148)))), ((int)(((byte)(255)))));
            this.txtObservation.Location = new System.Drawing.Point(127, 173);
            this.txtObservation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtObservation.Name = "txtObservation";
            this.txtObservation.PasswordChar = '\0';
            this.txtObservation.PlaceholderText = "";
            this.txtObservation.SelectedText = "";
            this.txtObservation.Size = new System.Drawing.Size(380, 98);
            this.txtObservation.TabIndex = 2;
            // 
            // frmObservation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(686, 368);
            this.Controls.Add(this.txtObservation);
            this.Controls.Add(this.label2);
            this.Name = "frmObservation";
            this.Text = "frmObservation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private Guna.UI2.WinForms.Guna2TextBox txtObservation;
    }
}