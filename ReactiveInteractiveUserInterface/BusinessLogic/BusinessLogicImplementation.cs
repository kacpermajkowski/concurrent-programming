//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System.Diagnostics;
using System.Numerics;
using TP.ConcurrentProgramming.Data;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        {
        }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBelow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI
        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

            layerBelow.Dispose();
            Disposed = true;
        }

        public override void Start(int numberOfBalls, Action<IPosition, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            layerBelow.Start(numberOfBalls, (startingPosition, databall) => {
                var ball = new Ball(databall);
                upperLayerHandler(new Position(startingPosition.x, startingPosition.x), ball);
                BallList.Add(ball);
            });

            Task.Run(() => MoveBallsLoop());
        }

        public override void SetBorderSize(int borderSize)
        {
            _borderSize = borderSize;
        }
        #endregion BusinessLogicAbstractAPI

        #region private
        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBelow;

        private List<Ball> BallList = new List<Ball>();
        private int _borderSize = 600;

        private async Task MoveBallsLoop()
        {
            const double FPS = 60.0;
            while (true)
            {
                foreach (var ball in BallList)
                {
                    double newX = ball.Position.x + ball.Velocity.x / FPS;
                    double newY = ball.Position.y + ball.Velocity.y / FPS;

                    double diameter = 20.0;

                    if (newX < 0)
                    {
                        newX = 0;
                        ball.Velocity = new Vector(-ball.Velocity.x, ball.Velocity.y);
                    }
                    else if (newX + diameter > _borderSize)
                    {
                        newX = _borderSize - diameter;
                        ball.Velocity = new Vector(-ball.Velocity.x, ball.Velocity.y);
                    }

                    if (newY < 0)
                    {
                        newY = 0;
                        ball.Velocity = new Vector(ball.Velocity.x, -ball.Velocity.y);
                    }
                    else if (newY + diameter > _borderSize)
                    {
                        newY = _borderSize - diameter;
                        ball.Velocity = new Vector(ball.Velocity.x, -ball.Velocity.y);
                    }

                    ball.Move(new Vector(newX - ball.Position.x, newY - ball.Position.y));
                }
                await Task.Delay((int)(1000.0 / FPS));
            }
        }
        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}