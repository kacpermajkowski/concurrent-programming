using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor

        public DataImplementation()
        {
        }

        #endregion ctor

        #region DataAbstractAPI

        public override void MoveBall(int id)
        {
            Ball ball = BallsList[id];
            lock (ball)
            {
                ball.Move(new Vector(ball.Velocity.x, ball.Velocity.y));
            }
        }

        public override IBall GetBall(int id)
        {
            return BallsList[id];
        }

        public override int GetBallCount()
        {
            return BallsList.Count;
        }

        public override void Start(int numberOfBalls, Action<IVector, IBall, double> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));
            Random random = new Random();
            const double massConst = 10.0;
            for (int i = 0; i < numberOfBalls; i++)
            {
                double diameter = random.Next(10, 51);
                double radius = diameter / 2.0;
                double x = random.NextDouble() * (792 - 2 * radius) + radius;
                double y = random.NextDouble() * (592 - 2 * radius) + radius;
                Vector startingPosition = new(x, y);

                Vector ballVelocity;
                double mass = (Math.PI * diameter * diameter) / (4.0 * massConst);
                do
                {
                    ballVelocity = new((RandomGenerator.NextDouble() - 0.5) * 4,
                                       (RandomGenerator.NextDouble() - 0.5) * 3);
                } while (ballVelocity.x == 0 || ballVelocity.y == 0);
                Ball newBall = new(startingPosition, ballVelocity, diameter, mass);
                upperLayerHandler(startingPosition, newBall, diameter);
                BallsList.Add(newBall);
            }
        }

        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    Disposed = true;
                    BallsList.Clear();
                }
                Disposed = true;
            }
            else
                throw new ObjectDisposedException(nameof(DataImplementation));
        }

        public override void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable

        #region private

        private bool Disposed = false;
        private Random RandomGenerator = new();
        private List<Ball> BallsList = new();

        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallsList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallsList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}
