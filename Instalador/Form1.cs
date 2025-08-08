using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace Instalador
    {
    public partial class Form1 : Form
        {
        private const string DotNetDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/0e7e2e2b-9e2e-4e7e-9e2e-7e2e9e2e9e2e/1a2b3c4d5e6f7g8h9i0j/dotnet-desktop-runtime-9.0.7-win-x64.exe";
        private const string DotNetInstallerFileName = "dotnet-desktop-runtime-9.0.7-win-x64.exe";
        private const string ClickOnceUrl = "https://instaladordataobra.blob.core.windows.net/$web/DataObra.application";
        private const string LocalApplicationFile = "DataObra.application";
        private WebClient webClient;

        public Form1()
            {
            InitializeComponent();
            }

        // Verifica e instala .NET 9 Desktop Runtime
        private void btnInstalarNET_Click(object sender, EventArgs e)
            {
            btnInstalarNET.Enabled = false;
            progressBar.Value = 0;
            lblEstado.Text = "";

            if (IsDotNet9Installed())
                {
                MessageBox.Show(".NET 9 ya está instalado.", "Estado", MessageBoxButtons.OK, MessageBoxIcon.Information);
                btnInstalarNET.Enabled = true;
                return;
                }

            lblEstado.Text = "Descargando .NET 9 Desktop Runtime...";
            webClient = new WebClient();
            webClient.DownloadProgressChanged += (s, ev) =>
            {
                if (progressBar.InvokeRequired)
                    progressBar.Invoke(new Action(() => progressBar.Value = ev.ProgressPercentage));
                else
                    progressBar.Value = ev.ProgressPercentage;
            };
            webClient.DownloadFileCompleted += (s, ev) =>
            {
                lblEstado.Text = "Descarga completa. Ejecutando instalador...";
                try
                    {
                    Process installer = Process.Start(new ProcessStartInfo
                        {
                        FileName = DotNetInstallerFileName,
                        Arguments = "/quiet /norestart",
                        UseShellExecute = true
                        });
                    installer.WaitForExit();
                    if (IsDotNet9Installed())
                        {
                        MessageBox.Show(".NET 9 instalado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    else
                        {
                        MessageBox.Show("La instalación de .NET 9 no se completó correctamente.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Error al instalar .NET 9:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                btnInstalarNET.Enabled = true;
                lblEstado.Text = "";
                progressBar.Value = 0;
            };
            try
                {
                webClient.DownloadFileAsync(new Uri(DotNetDownloadUrl), DotNetInstallerFileName);
                }
            catch (Exception ex)
                {
                MessageBox.Show("Error al descargar .NET 9:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInstalarNET.Enabled = true;
                lblEstado.Text = "";
                progressBar.Value = 0;
                }
            }

        // Instala la aplicación ClickOnce de modo silencioso
        private void btnInstalarDO_Click(object sender, EventArgs e)
            {
            btnInstalaDO.Enabled = false;
            progressBar.Value = 0;
            lblEstado.Text = "Descargando instalador DataObra...";

            webClient = new WebClient();
            webClient.DownloadProgressChanged += (s, ev) =>
            {
                if (progressBar.InvokeRequired)
                    progressBar.Invoke(new Action(() => progressBar.Value = ev.ProgressPercentage));
                else
                    progressBar.Value = ev.ProgressPercentage;
            };
            webClient.DownloadFileCompleted += (s, ev) =>
            {
                lblEstado.Text = "Descarga completa. Instalando DataObra...";
                try
                    {
                    string dfsvcPath = System.IO.Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                        @"Microsoft.NET\Framework\v4.0.30319\dfsvc.exe"
                    );

                    if (!System.IO.File.Exists(dfsvcPath))
                        {
                        MessageBox.Show("No se encontró dfsvc.exe. ClickOnce requiere .NET Framework 4.x instalado.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        btnInstalaDO.Enabled = true;
                        lblEstado.Text = "";
                        progressBar.Value = 0;
                        return;
                        }

                    ProcessStartInfo psi = new ProcessStartInfo
                        {
                        FileName = dfsvcPath,
                        Arguments = $"/Install \"{System.IO.Path.GetFullPath(LocalApplicationFile)}\"",
                        UseShellExecute = false,
                        CreateNoWindow = true
                        };

                    using (Process proc = Process.Start(psi))
                        {
                        proc.WaitForExit();
                        }

                    MessageBox.Show("Instalación de DataObra finalizada correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                catch (Exception ex)
                    {
                    MessageBox.Show("Error al instalar DataObra:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                btnInstalaDO.Enabled = true;
                lblEstado.Text = "";
                progressBar.Value = 0;
            };
            try
                {
                webClient.DownloadFileAsync(new Uri(ClickOnceUrl), LocalApplicationFile);
                }
            catch (Exception ex)
                {
                MessageBox.Show("Error al descargar DataObra:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnInstalaDO.Enabled = true;
                lblEstado.Text = "";
                progressBar.Value = 0;
                }
            }

        // Verifica si .NET 9 Desktop Runtime está instalado
        private bool IsDotNet9Installed()
            {
            try
                {
                string dotnetPath = Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\dotnet\dotnet.exe");
                if (System.IO.File.Exists(dotnetPath))
                    {
                    ProcessStartInfo psi = new ProcessStartInfo
                        {
                        FileName = dotnetPath,
                        Arguments = "--list-runtimes",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                        };

                    using (Process proc = Process.Start(psi))
                        {
                        string output = proc.StandardOutput.ReadToEnd();
                        return output.Contains("Microsoft.WindowsDesktop.App 9.");
                        }
                    }
                }
            catch
                {
                // Ignorar errores
                }
            return false;
            }
        }
    }