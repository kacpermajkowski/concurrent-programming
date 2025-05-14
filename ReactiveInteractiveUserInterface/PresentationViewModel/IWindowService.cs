using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Presentation.Model;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public interface IWindowService
    {
        public void ShowSimulationWindow();
        public void CloseSetupWindow();
    }
}
