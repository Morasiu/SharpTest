using System;
using Gtk;

namespace SharpTest{
    internal static class SharpTest{
        public static void Main(string[] args){
            Application.Init();

            var myWin = new Window("Sharp Test");
            myWin.Resize(600, 400);
            myWin.SetIconFromFile("/home/hubert/SharpTest/SharpTest/images/icon.png");
            
            var fileButton = new Button("Choose file", IconSize.Button, OpenDialog(myWin));
            
            var boxMain = new VBox();
            boxMain.PackStart(fileButton, false, true, 5);
            myWin.Add(boxMain);

            myWin.ShowAll();
            Application.Run();

        }

        static string OpenDialog(Window win){
            FileChooserDialog filechooser =
                new FileChooserDialog("Choose the file to open",
                    this,
                    FileChooserAction.Open,
                    "Cancel",ResponseType.Cancel,
                    "Open",ResponseType.Accept);

            if (filechooser.Run() == (int)ResponseType.Accept) 
            {
                System.IO.FileStream file = System.IO.File.OpenRead(filechooser.Filename);
                file.Close();
            }

            filechooser.Destroy();
        }
    }
}

