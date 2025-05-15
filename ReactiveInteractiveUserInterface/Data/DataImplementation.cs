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
using System.Xml;

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
                double x = random.NextDouble() * (772 - 2 * radius) + radius;
                double y = random.NextDouble() * (572 - 2 * radius) + radius;
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

            MoveTimer = new Timer(Move, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(16));
        }


        #endregion DataAbstractAPI

        #region IDisposable

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                if (disposing)
                {
                    MoveTimer.Dispose();
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

        private Timer MoveTimer { get; set; }
        private Random RandomGenerator = new();
        private List<Ball> BallsList = [];

        private void Move(object? x)
        {
            foreach (Ball item in BallsList)
            {
                IVector velocity = item.Velocity;
                Vector position = item.GetPosition();
                double radius = item.Diameter / 2.0;
                double xResult = position.x + velocity.x;
                double yResult = position.y + velocity.y;

                bool bounced = false;

                if (xResult - radius < 0)
                {
                    xResult = radius;
                    velocity = new Vector(-velocity.x, velocity.y);
                    bounced = true;
                }
                else if (xResult + radius > 792)
                {
                    xResult = 792 - radius;
                    velocity = new Vector(-velocity.x, velocity.y);
                    bounced = true;
                }
                if (yResult - radius < 0)
                {
                    yResult = radius;
                    velocity = new Vector(velocity.x, -velocity.y);
                    bounced = true;
                }
                else if (yResult + radius > 592)
                {
                    yResult = 592 - radius;
                    velocity = new Vector(velocity.x, -velocity.y);
                    bounced = true;
                }

                item.Velocity = velocity;
                if (bounced)
                {
                    item.Move(new Vector(xResult - position.x, yResult - position.y));
                }
            }

            int count = BallsList.Count;
            for (int i = 0; i < count; i++)
            {
                for (int j = i + 1; j < count; j++)
                {
                    Ball ballA = BallsList[i];
                    Ball ballB = BallsList[j];

                    Vector posA = ballA.GetPosition();
                    Vector posB = ballB.GetPosition();

                    double dx = posB.x - posA.x;
                    double dy = posB.y - posA.y;
                    double distance = Math.Sqrt(dx * dx + dy * dy);

                    double minDist = (ballA.Diameter + ballB.Diameter) / 2.0;

                    if (distance < minDist && distance > 0)
                    {
                        double mA = ballA.GetMass();
                        double mB = ballB.GetMass();
                        Vector velA = (Vector)ballA.Velocity;
                        Vector velB = (Vector)ballB.Velocity;

                        double nx = dx / distance;
                        double ny = dy / distance;
                        double pA = velA.x * nx + velA.y * ny;
                        double pB = velB.x * nx + velB.y * ny;

                        double pAnew = (pA * (mA - mB) + 2 * mB * pB) / (mA + mB);
                        double pBnew = (pB * (mB - mA) + 2 * mA * pA) / (mA + mB);

                        Vector newVelA = new Vector(
                            velA.x + (pAnew - pA) * nx,
                            velA.y + (pAnew - pA) * ny
                        );
                        Vector newVelB = new Vector(
                            velB.x + (pBnew - pB) * nx,
                            velB.y + (pBnew - pB) * ny
                        );

                        ballA.Velocity = newVelA;
                        ballB.Velocity = newVelB;

                        double overlap = minDist - distance + 0.1;
                        double totalMass = mA + mB;
                        double moveA = overlap * (mB / totalMass);
                        double moveB = overlap * (mA / totalMass);

                        ballA.Move(new Vector(-moveA * nx, -moveA * ny));
                        ballB.Move(new Vector(moveB * nx, moveB * ny));
                    }
                }
            }

            foreach (Ball item in BallsList)
            {
                item.Move(new Vector(item.Velocity.x, item.Velocity.y));
            }
        }




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