using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
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
                if(result == null)
                    labelFileStatus.Show();
                else{
                    labelFileStatus.Hide();
                    var structure = "";
                    var classes = new List<string>(result.Keys);
                    //Add classes
                    foreach (var _class in classes){
                        structure += "Class: " + _class + "\n";
                        foreach (var method in result[_class]){
                            //Add method name                       
                            structure += "|__ Method: " + method["name"] + "\n";
                            //Add params
                            var parameters = "";
                            int i = 0;
                            foreach (var param in (IEnumerable) method["param"]){
                                if (i > 0)
                                    parameters += ",";
                                parameters += " " + param;
                                i++;
                            }
                            structure += "|____ Params: " + parameters + "\n";
                            //Add return
                            structure += "|____ Return: " + method["return"] + "\n";
                        }

                        structure += "\n";
                    }
                    
                    labelTest.Text = structure;
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

        private static Dictionary<string, List<Hashtable>> ScanFile(string filename){
            if (filename.Contains(".py")){
                var result = new Dictionary<string, List<Hashtable>>();
                Console.WriteLine("Scaning starterd. File: " + filename.Split('/').Last());
                var rawText = File.ReadAllText(filename);
                var text = rawText.Split(new[]{"\r\n", "\r", "\n"}, StringSplitOptions.None);
                var isOOP = IsOOP(text);
                //Check if scipt is using OOP
                if (isOOP){
                    Console.WriteLine("Detected objected programing.");
                    //Get List of all classes
                    result = GetFileStructure(text);
                    string json = JsonConvert.SerializeObject(result);
                    Console.WriteLine(json);
                }
                else{
                    Console.WriteLine("Detected script without OOP.");
                }
                TestScriptLinux(filename, "Shababa\n");
                return result;
            }
            Console.WriteLine("Not python :(");
            var scanError = new Dictionary<string, List<Hashtable>>();
            return scanError;
        }
        
        private static Dictionary<string, List<Hashtable>> GetFileStructure(IEnumerable<string> lines){
            var structure = new Dictionary<string, List<Hashtable>>();
            var methodList = new List<Hashtable>();
            var returnValue = "";
            var body = "";
            var className = "";
            
            foreach (var line in lines){
                if (line.Contains("class") && !line.Contains('\'') && !line.Contains('#') &&!line.Contains('\"') && !line.Contains("def")){
                    if (className != "" && methodList.Count > 0){
                        structure.Add(className, new List<Hashtable>(methodList));
                        methodList.Clear();
                    }
                    className = line.Split()[1].Split(':')[0];
                } else if (line.Contains("def")&& !line.Contains('\'') && !line.Contains('#') &&!line.Contains('\"')){
                    var methodInfo = new Hashtable();

                    var cleaLine = line.Replace(" ", string.Empty);
                    //Get name of method
                    var methodName = cleaLine.Substring(cleaLine.IndexOf("def", StringComparison.Ordinal) +3, cleaLine.LastIndexOf('(')-3);
                    //Get params passed to method
                    var methodParams = cleaLine.Substring(cleaLine.IndexOf('(') + 1 , cleaLine.IndexOf(')') - cleaLine.LastIndexOf('(') - 1).Split(',');
                    
                    methodInfo.Add("name", methodName);
                    methodInfo.Add("param", methodParams);
                    methodInfo.Add("return", returnValue);
                    methodInfo.Add("body", body);
                    methodList.Add(methodInfo);
                    returnValue = "";
                    body = "";

                } else if (line.Contains("return")&& !line.Contains('\'') && !line.Contains('#') &&!line.Contains('\"')){
                    var cleaLine = line.Replace(" ", string.Empty);
                    returnValue = cleaLine.Substring(cleaLine.IndexOf("return", StringComparison.Ordinal) + 6);
                }
                else{
                    body += line + "\n";
                }
            }
            structure.Add(className, methodList);
            
            return structure;
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

