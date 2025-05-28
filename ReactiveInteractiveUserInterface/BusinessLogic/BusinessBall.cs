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

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Ball : IBall
    {
        public Ball(Data.IBall ball)
        {
            _dataBall = ball;
            ball.NewPositionNotification += RaisePositionChangeEvent;
        }

        #region IBall

        public event EventHandler<IPosition>? NewPositionNotification;
        public Data.IVector Velocity
        {
            get => _dataBall.Velocity;
            set => _dataBall.Velocity = value;
        }

        public Position Position
        {
            get => new Position(_dataBall.Position.x, _dataBall.Position.y);
            set => _dataBall.Position = new Vector(value.x, value.y);
        }
        public void Move(IVector v)
        {
            _dataBall.Move(v);
        }

        #endregion IBall

        #region private
        private readonly Data.IBall _dataBall;

        private void RaisePositionChangeEvent(object? sender, Data.IVector e)
        {
            NewPositionNotification?.Invoke(this, new Position(e.x, e.y));
        }
        #endregion private
    }
}