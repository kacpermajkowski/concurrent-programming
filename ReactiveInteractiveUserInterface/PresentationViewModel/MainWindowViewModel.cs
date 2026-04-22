//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
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

      ModelLayer.SetDefaultWindowDimensions(DEFAULT_WINDOW_HEIGHT, DEFAULT_WINDOW_WIDTH);
      ModelLayer.NewScaleNotification += NewScaleNotification;

      Observer = ModelLayer.Subscribe<ModelIBall>(x => Balls.Add(x));
      StartCommand = new RelayCommand(ExecuteStart, CanExecuteStart);
    }

    #endregion ctor

    #region public API

    public ICommand StartCommand { get; }

    public bool SettingsVisibility
    {
      get => _settingsVisiblity;
      set
      {
        _settingsVisiblity = value;
        RaisePropertyChanged(nameof(SettingsVisibility));
      }
    }

    public int BallCount
    {
      get => _ballCount;
      set
      {
        if (value > 0 && value != _ballCount)
        {
          _ballCount = value;
          _ballCountText = value.ToString();
          RaisePropertyChanged(nameof(BallCount));
          RaisePropertyChanged(nameof(BallCountText));
        }
      }
    }
    public string BallCountText
    {
      get => _ballCountText;
      set
      {
        if (value == "")
        {
          _ballCountText = value;
          _ballCount = 0;
          RaisePropertyChanged(nameof(BallCountText));
          RaisePropertyChanged(nameof(BallCount));
        }
        if (int.TryParse(value, out int parsed) && parsed > 0)
        {
          if (parsed != _ballCount)
          {
            _ballCount = parsed;
            _ballCountText = value;
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

    public double WindowHeight {
      get => _windowHeight;
      set
      {
        _windowHeight = value;
        ModelLayer.SetWindowDimensions(_windowHeight, _windowWidth);
      }
    }

    public double WindowWidth {
      get => _windowWidth;
      set
      {
        _windowWidth = value;
        ModelLayer.SetWindowDimensions(_windowHeight, _windowWidth);
      }
    }

    public double TableHeight
    {
      get => _tableHeight * _scale;
      set
      {
        _tableHeight = value;
      }
    }

    public double TableWidth
    {
      get => _tableWidth * _scale;
      set
      {
        _tableWidth = value;
      }
    }

    public double Scale { get => _scale; }

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

    private double _scale = 1.0;

    private void NewScaleNotification(object sender, double newScale)
    {
      _scale = newScale;
      RaisePropertyChanged(nameof(Scale));
      RaisePropertyChanged(nameof(TableHeight));
      RaisePropertyChanged(nameof(TableWidth));
    }

    private const double DEFAULT_WINDOW_HEIGHT = 650;
    private const double DEFAULT_WINDOW_WIDTH = 450;

    private double _windowHeight = DEFAULT_WINDOW_HEIGHT;
    private double _windowWidth = DEFAULT_WINDOW_WIDTH;

    private const double DEFAULT_TABLE_HEIGHT = 420;
    private const double DEFAULT_TABLE_WIDTH = 400;

    private double _tableHeight = DEFAULT_TABLE_HEIGHT;
    private double _tableWidth = DEFAULT_TABLE_WIDTH;

    private bool Disposed = false;

    private IDisposable Observer = null;
    private ModelAbstractApi ModelLayer;

    private bool _settingsVisiblity = true;

    private int _ballCount = 5;
    private string _ballCountText = "5";

    private void ExecuteStart()
    {
      Start(_ballCount);
    }

    private bool CanExecuteStart()
    {
      return _ballCount > 0;
    }

    private void Start(int numberOfBalls)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(MainWindowViewModel));

      SettingsVisibility = false;
      ModelLayer.Start(numberOfBalls);

      Observer.Dispose();
    }

    #endregion private
  }
}