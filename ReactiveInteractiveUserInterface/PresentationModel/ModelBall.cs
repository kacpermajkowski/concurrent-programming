//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  internal class ModelBall : IBall
  {
    public ModelBall(double top, double left, double diameter, LogicIBall logicBall, ModelImplementation modelLayer)
    {
      _top = top;
      _left = left;
      _diameter = diameter;

      logicBall.NewPositionNotification += NewPositionNotification;
      modelLayer.NewScaleNotification += NewScaleNotification;
    }

    #region IBall

    public double Top
    {
      get { return _top * _scale; }
      private set
      {
        if (_top == value)
          return;
        _top = value;
        RaisePropertyChanged();
      }
    }

    public double Left
    {
      get { return _left * _scale; }
      private set
      {
        if (_left == value)
          return;
        _left = value;
        RaisePropertyChanged();
      }
    }

    public double Diameter {
      get => _diameter * _scale;
    }

    #region INotifyPropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    #endregion INotifyPropertyChanged

    #endregion IBall

    #region private

    private double _top, _left, _diameter;

    private double _scale = 1.0;

    private void NewPositionNotification(object sender, IPosition newPos)
    {
      Top = newPos.y; 
      Left = newPos.x; 
    }

    private void NewScaleNotification(object sender, double scale)
    {
      _scale = scale;
      RaisePropertyChanged(nameof(Diameter));
    }

    private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
    {
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion private

    #region testing instrumentation

    [Conditional("DEBUG")]
    internal void SetLeft(double x)
    { Left = x; }

    [Conditional("DEBUG")]
    internal void SettTop(double x)
    { Top = x; }

    #endregion testing instrumentation
  }
}