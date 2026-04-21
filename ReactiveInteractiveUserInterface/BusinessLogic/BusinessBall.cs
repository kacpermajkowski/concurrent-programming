//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.Data;

using DataIVector = TP.ConcurrentProgramming.Data.IVector;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class Ball : IBall, IDisposable
  {
    private readonly Data.IBall _dataBall;

    public Ball(Data.IBall ball)
    {
      _dataBall = ball;
      _dataBall.NewPositionNotification += RaisePositionChangeEvent;
    }

    public void Dispose()
    {
      _dataBall.NewPositionNotification -= RaisePositionChangeEvent;
    }

    #region IBall

    public event EventHandler<IPosition>? NewPositionNotification;

    public void Move(double xDelta, double yDelta)
    {
      _dataBall.Move(new Vector(xDelta, yDelta));
    }

    #endregion IBall

    #region private

    private void RaisePositionChangeEvent(object? sender, Data.IVector e)
    {
      NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
    }

    #endregion private

    public class Vector : DataIVector
    {
      public Vector(double x, double y)
      {
        this.x = x;
        this.y = y;
      }
      public double x { get; init; }
      public double y { get; init; }
    }
  }
}