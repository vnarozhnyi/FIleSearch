using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FileSearchApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var splashForm = new MainForm();
            var mainThread = new Thread(() => Application.Run(splashForm));
            mainThread.SetApartmentState(ApartmentState.STA);
            mainThread.Start();
        }
    }
}
