using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    public class WindowService : IWindowService
    {
        public void CloseSetupWindow()
        {
            var setupWindow = Application.Current.Windows.OfType<SetupWindow>().FirstOrDefault();
            if (setupWindow != null)
            {
                setupWindow.Close();
            }
        }

        public void ShowSimulationWindow()
        {
            var window = new MainWindow();
            window.Show();
        }
    }
}
