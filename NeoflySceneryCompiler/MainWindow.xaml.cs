
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Diagnostics;
using System.IO;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace NeoflySceneryCompiler
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        private async void CompileButton_Click(object sender, RoutedEventArgs e)
        {
            var hwnd = WindowNative.GetWindowHandle(this);
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".xml");
            InitializeWithWindow.Initialize(picker, hwnd);
            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                string sdkToolPath = @"C:\MSFS SDK\Tools\fspackagetool.exe";
                string communityPath = @"D:\FS\Community";

                var startInfo = new ProcessStartInfo
                {
                    FileName = sdkToolPath,
                    Arguments = $""{file.Path}"",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };

                try
                {
                    var process = Process.Start(startInfo);
                    string output = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    string packageOutputPath = Path.Combine(Path.GetDirectoryName(file.Path), "Packages");
                    if (Directory.Exists(packageOutputPath))
                    {
                        foreach (var dir in Directory.GetDirectories(packageOutputPath))
                        {
                            string dirName = Path.GetFileName(dir);
                            string destPath = Path.Combine(communityPath, dirName);
                            if (Directory.Exists(destPath))
                                Directory.Delete(destPath, true);
                            Directory.Move(dir, destPath);
                        }
                        OutputBox.Text = "Compilation and copy to Community folder complete.";
                    }
                    else
                    {
                        OutputBox.Text = "Compilation complete, but no Packages folder found.";
                    }
                }
                catch (Exception ex)
                {
                    OutputBox.Text = $"Error: {ex.Message}";
                }
            }
        }
    }
}
