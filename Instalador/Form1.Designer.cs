namespace Instalador
    {
    partial class Form1
        {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnInstalarNET;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblEstado;

        protected override void Dispose(bool disposing)
            {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
            }

        private void InitializeComponent()
            {
            this.btnInstalarNET = new System.Windows.Forms.Button();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblEstado = new System.Windows.Forms.Label();
            this.btnInstalaDO = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInstalarNET
            // 
            this.btnInstalarNET.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnInstalarNET.Location = new System.Drawing.Point(229, 130);
            this.btnInstalarNET.Name = "btnInstalarNET";
            this.btnInstalarNET.Size = new System.Drawing.Size(200, 40);
            this.btnInstalarNET.TabIndex = 0;
            this.btnInstalarNET.Text = "Instalar .NET 9";
            this.btnInstalarNET.UseVisualStyleBackColor = true;
            this.btnInstalarNET.Click += new System.EventHandler(this.btnInstalarNET_Click);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(229, 248);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(200, 23);
            this.progressBar.TabIndex = 1;
            // 
            // lblEstado
            // 
            this.lblEstado.AutoSize = true;
            this.lblEstado.Location = new System.Drawing.Point(40, 130);
            this.lblEstado.Name = "lblEstado";
            this.lblEstado.Size = new System.Drawing.Size(0, 20);
            this.lblEstado.TabIndex = 2;
            // 
            // btnInstalaDO
            // 
            this.btnInstalaDO.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.btnInstalaDO.Location = new System.Drawing.Point(229, 186);
            this.btnInstalaDO.Name = "btnInstalaDO";
            this.btnInstalaDO.Size = new System.Drawing.Size(200, 40);
            this.btnInstalaDO.TabIndex = 3;
            this.btnInstalaDO.Text = "Instala DataObra";
            this.btnInstalaDO.UseVisualStyleBackColor = true;
            this.btnInstalaDO.Click += new System.EventHandler(this.btnInstalarDO_Click);
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(654, 569);
            this.Controls.Add(this.btnInstalaDO);
            this.Controls.Add(this.lblEstado);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.btnInstalarNET);
            this.Name = "Form1";
            this.Text = "Instalador de .NET 9";
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        private System.Windows.Forms.Button btnInstalaDO;
        }
    }