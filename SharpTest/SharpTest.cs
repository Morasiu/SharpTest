using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gtk;
using Application = Gtk.Application;
using Process = System.Diagnostics.Process;
using Window = Gtk.Window;

namespace SharpTest{
    internal static class SharpTest{
        private static readonly string OsVersion = Environment.OSVersion.ToString();
        private static readonly string PythonVer = CheckPython();

        public static void Main(){
            Application.Init();
            var window = new Window("Sharp Test");
            window.Resize(600, 400);
            window.SetIconFromFile("/home/hubert/SharpTest/SharpTest/images/icon.png");
            window.DeleteEvent += Window_Delete;
            //File choosing components
            var labelOsName = new Label("OS Version: " + OsVersion);
            
            var labelPyhtonVer = new Label("Python ver: " + PythonVer);
            
            var labelChoose = new Label("Choose python file.");
            
            var labelFileStatus = new Label{Markup = "<span color=\"red\">Wrong file.</span>"};
            
            var fileButton = new FileChooserButton("Choose file", FileChooserAction.Open);
            
            var scanButton = new Button("Scan file");
            scanButton.SetSizeRequest(100, 50);
            
            var boxStart = new VBox();
            boxStart.PackStart(labelOsName, false,false, 5);
            boxStart.PackStart(labelPyhtonVer, false, false, 5);
            boxStart.PackStart(labelChoose, false, false, 5);
            boxStart.PackStart(fileButton, false, false, 5);
            boxStart.PackStart(labelFileStatus, false, false, 5);
            boxStart.PackStart(scanButton, false, false, 100);
            
            //Scaning window components
            var labelTest = new Label("Error while scanning file");
            labelTest.SetPadding(5, 5);
            
            var scrolledWin = new ScrolledWindow();
            scrolledWin.SetPolicy(PolicyType.Automatic, PolicyType.Automatic);
            scrolledWin.Add(labelTest);
        
            var boxScan = new VBox();
            boxScan.PackStart(scrolledWin, true, true, 5);
            
            scanButton.Clicked += (sender, args) => {
                var result = ScanFile(fileButton.Filename);
                if(result.Contains("Wrong file"))
                    labelFileStatus.Show();
                else{
                    labelFileStatus.Hide();
                    labelTest.Text = result[0];
                    window.Remove(boxStart);
                    window.Add(boxScan);
                    window.ShowAll();
                }
            };
            
            window.Add(boxStart);
            window.ShowAll();
            //Hide some widgets
            
            labelFileStatus.Hide();
            Application.Run();
        }

        private static string[] ScanFile(string filename){
            if (filename.Contains(".py")){
                Console.WriteLine("Scaning starterd. File: " + filename.Split('/').Last());
                var rawText = File.ReadAllText(filename);
                var text = rawText.Split(new[]{"\r\n", "\r", "\n"}, StringSplitOptions.None);
                var isOOP = IsOOP(text);
                Console.WriteLine(isOOP ? "Detected objected programing." : "Detected script without OOP.");
                TestScriptLinux(filename, "Shababa\n");
                Console.WriteLine(text[1]);
                string[] scan = {"Scan"};
                return scan;
            }
            Console.WriteLine("Not python :(");
            string[] scanError = {"Wrong file"};
            return scanError;
        }

        private static bool IsOOP(IEnumerable<string> text){
            return text.Any(line => line.Contains("class"));
        }
        
        private static void TestScriptLinux(string fileName, string shouldReturn){
            var p = new Process{
                StartInfo = new ProcessStartInfo(@"/usr/bin/python", fileName){
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            
            if (output == shouldReturn)
                Console.WriteLine("Hurra");
            
            p.WaitForExit();
        }
        
        private static void TestScriptWindows(string fileName){

            var p = new Process{
                StartInfo = new ProcessStartInfo(@"C:\Python27\python.exe", fileName){
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            Console.WriteLine(output);
        }

        private static string CheckPython(){
            var processInfo = new string[2];
            if(OsVersion.Contains("Unix")){
                //Linux
                processInfo[0] = "/usr/bin/python";
                processInfo[1] = "--version";
            }
            else{
                //Windows
                processInfo[0] = "cmd.exe";
                processInfo[1] = "python --version";
            }

            var p = new Process{
                StartInfo = new ProcessStartInfo{
                    FileName = processInfo[0],
                    Arguments = processInfo[1],
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };
            p.Start();
            
            var output = p.StandardError.ReadToEnd();
            p.WaitForExit();
            return output;
        }
        
        private static void Window_Delete(object o, DeleteEventArgs args){
            Application.Quit ();
            args.RetVal = true;
        }
    }
}

