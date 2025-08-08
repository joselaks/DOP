using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;

namespace InstaladorDataObra
    {
    public partial class MainWindow : Window
        {
        private const string DotNetDownloadUrl = "https://download.visualstudio.microsoft.com/download/pr/0e7e2e2b-9e2e-4e7e-9e2e-7e2e9e2e9e2e/1a2b3c4d5e6f7g8h9i0j/dotnet-desktop-runtime-9.0.7-win-x64.exe";
        private const string DotNetInstallerFileName = "dotnet-desktop-runtime-9.0.7-win-x64.exe";
        private const string ClickOnceUrl = "https://instaladordataobra.blob.core.windows.net/$web/DataObra.application";

        public MainWindow()
            {
            InitializeComponent();
            }

        private async void instalaNET_Click(object sender, RoutedEventArgs e)
            {
            instalaNET.IsEnabled = false;
            progressBar.Value = 0;

            if (await Task.Run(() => IsDotNet9Installed()))
                {
                MessageBox.Show(".NET 9 ya está instalado.", "Estado", MessageBoxButton.OK, MessageBoxImage.Information);
                instalaNET.IsEnabled = true;
                return;
                }

            try
                {
                progressBar.Value = 0;
                await DownloadFileAsync(DotNetDownloadUrl, DotNetInstallerFileName);

                progressBar.Value = 100;
                MessageBox.Show("Descarga completa. Instalando .NET 9...", "Instalador", MessageBoxButton.OK, MessageBoxImage.Information);

                var process = Process.Start(new ProcessStartInfo
                    {
                    FileName = DotNetInstallerFileName,
                    Arguments = "/quiet /norestart",
                    UseShellExecute = true
                    });
                process.WaitForExit();

                if (await Task.Run(() => IsDotNet9Installed()))
                    {
                    MessageBox.Show(".NET 9 instalado correctamente.", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                else
                    {
                    MessageBox.Show("La instalación de .NET 9 no se completó correctamente.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            catch (Exception ex)
                {
                MessageBox.Show("Error al descargar o instalar .NET 9:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            finally
                {
                instalaNET.IsEnabled = true;
                progressBar.Value = 0;
                }
            }

        private async void InstalaDO_Click(object sender, RoutedEventArgs e)
            {
            InstalaDO.IsEnabled = false;
            progressBar.Value = 0;

            try
                {
                // Descarga el archivo .application mostrando el progreso
                await DownloadFileAsync(ClickOnceUrl, "DataObra.application");
                progressBar.Value = 10;

                // Ejecuta el archivo .application para iniciar la instalación ClickOnce
                Process.Start(new ProcessStartInfo
                    {
                    FileName = Path.GetFullPath("DataObra.application"),
                    UseShellExecute = true
                    });

                MessageBox.Show("Se ha iniciado la instalación de DataObra. Siga las instrucciones en pantalla.", "Instalador", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            catch (Exception ex)
                {
                MessageBox.Show("Error al descargar o iniciar la instalación de DataObra:\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            finally
                {
                InstalaDO.IsEnabled = true;
                progressBar.Value = 0;
                }
            }

        // Descarga un archivo mostrando el progreso en el ProgressBar
        private async Task DownloadFileAsync(string url, string destinationFile)
            {
            using (var client = new WebClient())
                {
                client.DownloadProgressChanged += (s, e) =>
                {
                    Dispatcher.Invoke(() => progressBar.Value = e.ProgressPercentage);
                };
                await client.DownloadFileTaskAsync(new Uri(url), destinationFile);
                }
            }

        // Verifica si .NET 9 Desktop Runtime está instalado
        private bool IsDotNet9Installed()
            {
            try
                {
                string dotnetPath = Environment.ExpandEnvironmentVariables(@"%ProgramFiles%\dotnet\dotnet.exe");
                if (File.Exists(dotnetPath))
                    {
                    var psi = new ProcessStartInfo
                        {
                        FileName = dotnetPath,
                        Arguments = "--list-runtimes",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                        };

                    using (var proc = Process.Start(psi))
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