using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Gtk;

namespace SharpTest{
    internal static class SharpTest{
        private static string osVersion = Environment.OSVersion.ToString();
        
        public static void Main(string[] args){
            Application.Init();

            var window = new Window("Sharp Test");
            window.Resize(600, 400);
            window.SetIconFromFile("/home/hubert/SharpTest/SharpTest/images/icon.png");
            window.DeleteEvent += Window_Delete;

            var fileButton = new FileChooserButton("Choose file", FileChooserAction.Open);
            
            var scanButton = new Button("Scan file");
            scanButton.SetSizeRequest(100, 50);
            scanButton.Clicked += (obj, evt) => ScanFile (fileButton.Filename);
            
            Console.WriteLine(Environment.OSVersion);
            
            var boxMain = new VBox();
            boxMain.PackStart(fileButton, false, false, 5);
            boxMain.PackStart(scanButton, false, false, 100);
            window.Add(boxMain);

            window.ShowAll();
            Application.Run();

        }

        private static void Window_Delete(object o, DeleteEventArgs args){
            Application.Quit ();
            args.RetVal = true;
        }

        private static void ScanFile(string filename){
            if (filename.Contains(".py")){
                Console.WriteLine("Scaning starterd. File: " + filename.Split('/').Last());
                var rawText = File.ReadAllText(filename);
                var text = rawText.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None);
                var isOOP = IsOOP(text);
                Console.WriteLine(isOOP ? "Detected objected programing." : "Detected script without OOP.");
                TestScriptLinux(filename, "Shababa\n");
                Console.WriteLine(text[1]);
            }
            else{
                Console.WriteLine("Not python :(");
            }
        }

        private static bool IsOOP(IEnumerable<string> text){
            return text.Any(line => line.Contains("class"));
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

            Console.ReadLine();
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
            if (output == shouldReturn){
                Console.WriteLine("Hurra");
            }
            p.WaitForExit();
        }
    }
}

