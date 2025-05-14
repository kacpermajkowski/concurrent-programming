using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class SetupWindowViewModel : ViewModelBase, IDisposable
    {
        private SetupWindowViewModel() : this(null)
        { }
        public SetupWindowViewModel(IWindowService windowService)
        {
            ModelLayer = ModelAbstractApi.CreateModel();
            StartSimulationCmd = new RelayCommand(StartSimulation);
            _windowService = windowService;
        }

        private int _numberOfBalls = 5;
        public string NumberOfBalls {
            get
            {
                return _numberOfBalls.ToString();
            }
            set
            {
                _numberOfBalls = int.TryParse(value, out int ballCount) ? ballCount : _numberOfBalls;
                RaisePropertyChanged(nameof(NumberOfBalls));
            }
        }

        public ICommand StartSimulationCmd { get; }
        private void StartSimulation()
        {
           _windowService.ShowSimulationWindow();
           ModelLayer.Start(_numberOfBalls);
           _windowService.CloseSetupWindow();
        }


        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(SetupWindowViewModel));
            Disposed = true;
            GC.SuppressFinalize(this);
        }

        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        private IWindowService _windowService;
    }
}
