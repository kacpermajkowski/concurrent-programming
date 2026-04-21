//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

namespace TP.ConcurrentProgramming.Data
{
  internal class Ball : IBall
  {
    #region ctor

    internal Ball(Vector initialPosition, Vector initialVelocity, double diameter)
    {
      _position = initialPosition;
      Velocity = initialVelocity;
      Diameter = diameter;
    }

    #endregion ctor

    #region IBall

    public event EventHandler<IVector>? NewPositionNotification;

    public IVector Velocity { get; set; }
    public double Diameter { get; }


    public IVector Position { get => _position; }
    

    public void Move(IVector delta)
    {
      _position = new Vector(_position.x + delta.x, _position.y + delta.y);
      RaiseNewPositionChangeNotification();
    }

    #endregion IBall

    #region private

    private Vector _position;

    private void RaiseNewPositionChangeNotification()
    {
      NewPositionNotification?.Invoke(this, _position);
    }

    #endregion private
  }
}