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
            ModelBallCreationObserver = ModelLayer.Subscribe<ModelIBall>(x => BallList.Add(x));
            BorderSizeObserver = ModelLayer.Subscribe<int>(x => BorderSize = x);
            ModelLayer.UpdateWindowSize(_windowWidth, _windowHeight);
        }
        #endregion ctor

        #region public API
        public ObservableCollection<ModelIBall> BallList { get; } = new ObservableCollection<ModelIBall>();

        public void Start(int numberOfBalls)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(MainWindowViewModel));
            ModelLayer.Start(numberOfBalls);
            //usunąłem ModelBallCreationObserver.Dispose();
            //ponieważ i tak jest usuwany w Dispose() i być może będę chciał go jeszcze użyć
        }

        public int WindowHeight
        {
            get => _windowHeight;
            set
            {
                if (_windowHeight != value)
                {
                    _windowHeight = value;
                    ModelLayer.UpdateWindowSize(_windowWidth, _windowHeight);
                    RaisePropertyChanged(nameof(WindowHeight));
                }
            }
        }
        public int WindowWidth
        {
            get => _windowWidth;
            set
            {
                if (_windowWidth != value)
                {
                    _windowWidth = value;
                    ModelLayer.UpdateWindowSize(_windowWidth, _windowHeight);
                    RaisePropertyChanged(nameof(WindowWidth));
                }
            }
        }

        public int BorderSize
        {
            get => _borderSize;
            set
            {
                if (_borderSize != value)
                {
                    _borderSize = value;
                    RaisePropertyChanged(nameof(BorderSize));
                }
            }
        }

        #endregion public API

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    BallList.Clear();
                    ModelBallCreationObserver.Dispose();
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
        private IDisposable ModelBallCreationObserver = null;
        private IDisposable BorderSizeObserver = null;
        private ModelAbstractApi ModelLayer;
        private int _windowHeight = 600;
        private int _windowWidth = 800;
        private int _borderSize = 1;
        private bool Disposed = false;
        #endregion private
    }
}