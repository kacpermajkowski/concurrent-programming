using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TP.ConcurrentProgramming.Presentation.ViewModel;

namespace TP.ConcurrentProgramming.PresentationView
{
    /// <summary>
    /// Logika interakcji dla klasy SetupWindow.xaml
    /// </summary>
    public partial class SetupWindow : Window
    {
        public SetupWindow()
        {
            InitializeComponent();
            IWindowService windowService = new WindowService();
            this.DataContext = new SetupWindowViewModel(windowService);
        }
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is SetupWindowViewModel viewModel)
                viewModel.Dispose();
            base.OnClosed(e);
        }
    }
}
