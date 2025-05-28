//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;

namespace TP.ConcurrentProgramming.Data
{
    internal class DataImplementation : DataAbstractAPI
    {
        #region ctor
        public DataImplementation()
        {
            //
        }
        #endregion ctor

        #region DataAbstractAPI
        public override void Start(int numberOfBalls, Action<IVector, IBall> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            for (int i = 0; i < numberOfBalls; i++)
            {
                // RandomGenerator to pole w klasie typu Random
                int randomX = RandomGenerator.Next(100, 400 - 100);
                int randomY = RandomGenerator.Next(100, 400 - 100);

                Vector startingPosition = new(randomX, randomY);
                Vector startingVelocity = startingPosition; // dla czytelności

                Ball newBall = new(startingPosition, startingVelocity);

                upperLayerHandler(startingPosition, newBall);
                BallList.Add(newBall);
            }
        }
        #endregion DataAbstractAPI

        #region IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(DataImplementation));
            if (disposing)
                BallList.Clear();

            Disposed = true;
        }

        // Do not change the code below. Put cleanup code in 'Dispose(bool disposing)' method
        public override void Dispose()
        {

            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable

        #region private
        private bool Disposed = false;

        private Random RandomGenerator = new();
        private List<Ball> BallList = [];
        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckBallsList(Action<IEnumerable<IBall>> returnBallsList)
        {
            returnBallsList(BallList);
        }

        [Conditional("DEBUG")]
        internal void CheckNumberOfBalls(Action<int> returnNumberOfBalls)
        {
            returnNumberOfBalls(BallList.Count);
        }

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        #endregion TestingInfrastructure
    }
}