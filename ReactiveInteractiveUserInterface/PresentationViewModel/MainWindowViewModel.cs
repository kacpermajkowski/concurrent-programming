//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.Collections.ObjectModel;
using TP.ConcurrentProgramming.Presentation.Model;
using TP.ConcurrentProgramming.Presentation.ViewModel.MVVMLight;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel
{
    public class MainWindowViewModel : ViewModelBase, IDisposable
    {
        #region ctor

        public MainWindowViewModel() : this(null)
        { }

        internal MainWindowViewModel(ModelAbstractApi modelLayerAPI)
        {
            ModelLayer = modelLayerAPI == null ? ModelAbstractApi.CreateModel() : modelLayerAPI;
            Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
        }

        #endregion ctor

        private int ballCount = 5;         
        private string ballCountText = "5";

        #region public API

        public int BallCount
        {
            get => ballCount;
            set
            {
                if (value > 0 && value != ballCount)
                {
                    ballCount = value;
                    ballCountText = value.ToString();
                    RaisePropertyChanged(nameof(BallCount));
                    RaisePropertyChanged(nameof(BallCountText));
                }
            }
        }

        public string BallCountText
        {
            get => ballCountText;
            set
            {
                if (value == "")
                {
                    ballCountText = value;
                    ballCount = 0;
                    RaisePropertyChanged(nameof(BallCountText));
                    RaisePropertyChanged(nameof(BallCount));
                }
                if (int.TryParse(value, out int parsed) && parsed > 0)
                {
                    if (parsed != ballCount)
                    {
                        ballCount = parsed;
                        ballCountText = value;
                        RaisePropertyChanged(nameof(BallCount));
                        RaisePropertyChanged(nameof(BallCountText));
                    }
                }
                else
                {
                    RaisePropertyChanged(nameof(BallCountText));
                }
            }
        }

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            Observer.Dispose();
        }

        public ObservableCollection<ModelIBall> Balls { get; } = new ObservableCollection<ModelIBall>();

        #endregion public API

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Balls.Clear();
                    Observer.Dispose();
                    ModelLayer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                Disposed = true;
            }
        }

        public void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private IDisposable Observer = null;
        private ModelAbstractApi ModelLayer;
        private bool Disposed = false;

        #endregion private
    }
}