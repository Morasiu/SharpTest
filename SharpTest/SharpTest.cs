using System;
using Gtk;

namespace SharpTest{
    internal static class SharpTest{
        public static void Main(string[] args){
            Application.Init();

            var window = new Window("Sharp Test");
            window.Resize(600, 400);
            window.SetIconFromFile("/home/hubert/SharpTest/SharpTest/images/icon.png");
            window.DeleteEvent += new DeleteEventHandler (Window_Delete);

            var fileButton = new FileChooserButton("Choose file", FileChooserAction.Open);
            
            var scanButton = new Button("Scan");
            scanButton.Clicked += new EventHandler(ScanFile);
            
            var boxMain = new VBox();
            boxMain.PackStart(fileButton, false, true, 5);
            boxMain.PackStart(scanButton, false, false, 5);
            window.Add(boxMain);

            window.ShowAll();
            Application.Run();

        }

        private static void Window_Delete(object o, DeleteEventArgs args){
            Application.Quit ();
            args.RetVal = true;
        }

        private static void ScanFile(object sender, EventArgs eventArgs){
            
        }
    }
}

