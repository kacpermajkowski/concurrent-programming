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

            ballCount = BallsList.Count;
            for (int i = 0; i < ballCount; i++)
            {
                int id = i; // lambda będzie korzystała z tej zmiennej, więc musimy skopiować wartość i
                //bo lamba sama tego nie zrobi i będzie korzystała z ostatniej wartości
                var thread = new Thread(() => BallThreadLoop(id));
                thread.IsBackground = true;
                ballThreads.Add(thread);
                thread.Start();
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
                    lock (barrierLock)
                    {
                        Monitor.PulseAll(barrierLock);
                    }
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
        private List<Thread> ballThreads = new();
        private int ballCount = 0;

        private readonly object barrierLock = new();
        private int readyCount = 0;
        private int moveCount = 0;

        private void BallThreadLoop(int id)
        {
            Ball myBall = BallsList[id];
            while (!Disposed)
            {
                CalculateCollisionsForBall(id);

                lock (barrierLock)
                {
                    readyCount++;
                    if (readyCount == ballCount)
                    {
                        readyCount = 0;
                        Monitor.PulseAll(barrierLock);
                    }
                    else
                    {
                        Monitor.Wait(barrierLock);
                    }
                }

                MoveBall(myBall);

                lock (barrierLock)
                {
                    moveCount++;
                    if (moveCount == ballCount)
                    {
                        moveCount = 0;
                        Monitor.PulseAll(barrierLock);
                    }
                    else
                    {
                        Monitor.Wait(barrierLock);
                    }
                }

                Thread.Sleep(16);
            }
        }

        private void CalculateCollisionsForBall(int id)
        {
            Ball ballA = BallsList[id];
            Vector posA, velA;
            double mA, radiusA, diameterA;

            lock (ballA)
            {
                posA = ballA.GetPosition();
                velA = (Vector)ballA.Velocity;
                mA = ballA.GetMass();
                diameterA = ballA.Diameter;
                radiusA = diameterA / 2.0;
            }

            double xResult = posA.x + velA.x;
            double yResult = posA.y + velA.y;
            bool bounced = false;

            if (xResult - radiusA < 0)
            {
                xResult = radiusA;
                velA = new Vector(-velA.x, velA.y);
                bounced = true;
            }
            else if (xResult + radiusA > 792)
            {
                xResult = 792 - radiusA;
                velA = new Vector(-velA.x, velA.y);
                bounced = true;
            }
            if (yResult - radiusA < 0)
            {
                yResult = radiusA;
                velA = new Vector(velA.x, -velA.y);
                bounced = true;
            }
            else if (yResult + radiusA > 592)
            {
                yResult = 592 - radiusA;
                velA = new Vector(velA.x, -velA.y);
                bounced = true;
            }

            if (bounced)
            {
                lock (ballA)
                {
                    ballA.Velocity = velA;
                    ballA.Move(new Vector(xResult - posA.x, yResult - posA.y));
                }
            }
            else
            {
                lock (ballA)
                {
                    ballA.Velocity = velA;
                }
            }

            for (int j = 0; j < BallsList.Count; j++)
            {
                if (j == id) continue;
                Ball ballB = BallsList[j];

                //blokujemy zawsze w tej samej kolejnosci, zeby uniknac zakleszczenia
                Ball first = id < j ? ballA : ballB;
                Ball second = id < j ? ballB : ballA;

                lock (first)
                {
                    lock (second)
                    {
                        Vector posB = ballB.GetPosition();
                        double diameterB = ballB.Diameter;
                        double radiusB = diameterB / 2.0;
                        double mB = ballB.GetMass();
                        Vector velB = (Vector)ballB.Velocity;

                        double dx = posB.x - posA.x;
                        double dy = posB.y - posA.y;
                        double distance = Math.Sqrt(dx * dx + dy * dy);

                        double minDist = (diameterA + diameterB) / 2.0;

                        if (distance < minDist && distance > 0)
                        {
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
            }
        }

        private void MoveBall(Ball ball)
        {
            lock (ball)
            {
                ball.Move(new Vector(ball.Velocity.x, ball.Velocity.y));
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
