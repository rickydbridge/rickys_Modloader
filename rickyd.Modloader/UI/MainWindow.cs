
namespace rickyd.Modloader.UI
{
    public partial class MainWindow : Form
    {
        public MainWindow ()
        {
            // Start the MainWindow with it's Designer variables
            InitializeComponent();

            Label modlistPathTextboxLabel = new()
            {
                Text = "Path to modlist file",
                Location = new Point(10, 10),
                Width = 600
            };

            TextBox modlistPathTextbox = new()
            {
                Location = new Point(modlistPathTextboxLabel.Location.X, modlistPathTextboxLabel.Bounds.Bottom + Padding.Top),
                Width = 600,
                TabIndex = 0
            };

            Button modlistPathBrowseButton = new()
            {
                Location = new Point(modlistPathTextbox.Bounds.Right + 10, modlistPathTextbox.Location.Y + (modlistPathTextbox.Height - 32) / 2),
                Size = new Size(100, 32),
                Padding = new Padding(8, 4, 8, 4),
                Text = "Browse...",
                TabIndex = 1
            };

            modlistPathBrowseButton.Click += (_, _) => BrowseForModlist();

            void BrowseForModlist ()
            {
                using OpenFileDialog dialog = new()
                {
                    Title = "Choose modlist file...",
                    Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                    FilterIndex = 1
                };
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    modlistPathTextbox.Text = dialog.FileName;
                }
            }

            Button downloadButton = new()
            {
                Location = new Point(modlistPathTextbox.Location.X, modlistPathTextbox.Bounds.Bottom + Padding.Top),
                Text = "Download",
                TabIndex = 2
            };

            downloadButton.Click += async (_, _) => await DownloadModsAsync();

            async Task DownloadModsAsync()
            {
                // Disable the download button when it's just been pressed
                downloadButton.Enabled = false;
                // Loop until broken
                while (true)
                {
                    // Start tracking for a download error
                    Exception? downloadError = null;
                    // Create the progress window
                    using Form progressWindow = new()
                    {
                        Text = "Download in progress",
                        FormBorderStyle = FormBorderStyle.FixedDialog,
                        StartPosition = FormStartPosition.CenterParent,
                        ClientSize = new Size(160, 50),
                        ControlBox = false,
                        ShowInTaskbar = false
                    };
                    Label progressLabel = new()
                    {
                        Text = "Downloading mods...",
                        AutoSize = true,
                        Location = new Point(12, 12)
                    };
                    progressWindow.Controls.Add(progressLabel);
                    // Create a timer for animating the download window with
                    using System.Windows.Forms.Timer progressTimer = new() { Interval = 700 };
                    int dotCount = 0;
                    progressTimer.Tick += (_, _) =>
                    {
                        dotCount = (dotCount + 1) % 4;
                        progressLabel.Text = $"Downloading mods{new string('.', dotCount)}";
                    };
                    progressWindow.Show(this);
                    progressTimer.Start();

                    try
                    {
                        await Program.DownloadModsFromList(modlistPathTextbox.Text);
                    }
                    catch (Exception ex)
                    {
                        Debug.Err(ex.ToString());
                        downloadError = ex;
                    }

                    progressTimer.Stop();
                    progressWindow.Close();

                    if (downloadError is null)
                    {
                        MessageBox.Show(this, $"Your download from {modlistPathTextbox.Text} completed successfully.\nSee bin/downloads for downloaded files.", "Download successful!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                    }

                    DialogResult result = MessageBox.Show(this, downloadError.Message, "Download failed", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result != DialogResult.Retry)
                    {
                        break;
                    }
                }

                downloadButton.Enabled = true;
            }

            Button openDownloadsFolderButton = new()
            {
                Location = new Point(downloadButton.Bounds.Right + 10, downloadButton.Location.Y),
                Size = new Size(160, downloadButton.Height),
                Text = "Open downloads folder",
                TabIndex = 3
            };

            openDownloadsFolderButton.Click += (_, _) => OpenDownloadsFolder();

            void OpenDownloadsFolder()
            {
                string downloadsFolder = Path.GetFullPath(Path.Combine(".", "bin", "downloads"));
                try
                {
                    Directory.CreateDirectory(downloadsFolder);
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "explorer.exe",
                        Arguments = $"/n, \"{downloadsFolder}\"",
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"Could not open the downloads folder:\n{ex.Message}", "Unable to open folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            Controls.Add(modlistPathTextboxLabel);
            Controls.Add(modlistPathTextbox);
            Controls.Add(modlistPathBrowseButton);
            Controls.Add(downloadButton);
            Controls.Add(openDownloadsFolderButton);
        }
    }
}
