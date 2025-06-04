using System.Diagnostics;
using System.Numerics;
using UnderneathLayerAPI = TP.ConcurrentProgramming.Data.DataAbstractAPI;
using DataIBall = TP.ConcurrentProgramming.Data.IBall;
using TP.ConcurrentProgramming.Data;
using System.IO;


namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
    {
        #region ctor

        public BusinessLogicImplementation() : this(null)
        { }

        internal BusinessLogicImplementation(UnderneathLayerAPI? underneathLayer)
        {
            layerBellow = underneathLayer == null ? UnderneathLayerAPI.GetDataLayer() : underneathLayer;
        }

        #endregion ctor

        #region BusinessLogicAbstractAPI

        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            layerBellow.Dispose();
            logger?.Dispose();
            lock (barrierLock)
            {
                Monitor.PulseAll(barrierLock);
            }
            Disposed = true;
        }
        private DiagnosticsLogger? logger;

        public override void Start(int numberOfBalls, Action<IPosition, IBall, double> upperLayerHandler)
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
            if (upperLayerHandler == null)
                throw new ArgumentNullException(nameof(upperLayerHandler));

            logger = new DiagnosticsLogger("log.txt");

            layerBellow.Start(numberOfBalls, (startingPosition, databall, diameter) => 
                upperLayerHandler(new Position(startingPosition.x, startingPosition.y), new Ball(databall), diameter));

            ballCount = layerBellow.GetBallCount();
            for (int i = 0; i < ballCount; i++)
            {
                int id = i;
                var thread = new Thread(() => BallThreadLoop(id));
                thread.IsBackground = true;
                ballThreads.Add(thread);
                thread.Start();
            }
        }

        #endregion BusinessLogicAbstractAPI

        #region private

        private bool Disposed = false;
        private List<Thread> ballThreads = new();
        private int ballCount = 0;

        private readonly object barrierLock = new();
        private int readyCount = 0;
        private int moveCount = 0;

        private void BallThreadLoop(int id)
        {
            DataIBall myBall = layerBellow.GetBall(id);
            DateTime lastUpdate = DateTime.Now;
            const int targetFrameMs = 16; // ~60 FPS

            while (!Disposed)
            {
                var frameStart = DateTime.Now;
                double deltaTime = (frameStart - lastUpdate).TotalSeconds;
                lastUpdate = frameStart;

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

                layerBellow.MoveBall(id);

                var pos = myBall.GetPosition();
                var vel = myBall.Velocity;
                logger?.Log($"{DateTime.Now:O},{id},{pos.x},{pos.y},{vel.x},{vel.y}");

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

                var frameEnd = DateTime.Now;
                int elapsedMs = (int)(frameEnd - frameStart).TotalMilliseconds;
                int sleepMs = targetFrameMs - elapsedMs;
                if (sleepMs > 0)
                    Thread.Sleep(sleepMs);
                // else: pętla wykonała się za wolno, nie śpimy
            }
        }

        private void CalculateCollisionsForBall(int id)
        {
            DataIBall ballA = layerBellow.GetBall(id);

            IVector posA, velA;
            double mA, radiusA, diameterA;
            lock (ballA)
            {
                posA = ballA.GetPosition();
                velA = ballA.Velocity;
                mA = ballA.GetMass();
                diameterA = ballA.Diameter;
                radiusA = diameterA / 2.0;
            }

            if (posA.x + velA.x - radiusA < 0 || posA.x + velA.x + radiusA > 792)
                velA = new Vector(-velA.x, velA.y);
            if (posA.y + velA.y - radiusA < 0 || posA.y + velA.y + radiusA > 592)
                velA = new Vector(velA.x, -velA.y);

            IVector updatedVelA = velA;

            for (int j = 0; j < layerBellow.GetBallCount(); j++)
            {
                if (j == id) continue;
                DataIBall ballB = layerBellow.GetBall(j);

                DataIBall first = id < j ? ballA : ballB;
                DataIBall second = id < j ? ballB : ballA;

                lock (first)
                {
                    lock (second)
                    {
                        IVector posB = ballB.GetPosition();
                        IVector velB = ballB.Velocity;
                        double mB = ballB.GetMass();
                        double diameterB = ballB.Diameter;
                        double radiusB = diameterB / 2.0;

                        double dx = posB.x - posA.x;
                        double dy = posB.y - posA.y;
                        double dist = Math.Sqrt(dx * dx + dy * dy);
                        double minDist = radiusA + radiusB;

                        if (dist < minDist && dist > 0)
                        {
                            double nx = dx / dist;
                            double ny = dy / dist;

                            double vA_n = updatedVelA.x * nx + updatedVelA.y * ny;
                            double vB_n = velB.x * nx + velB.y * ny;

                            if (vA_n - vB_n <= 0)
                                continue;

                            double vA_n_new = (vA_n * (mA - mB) + 2 * mB * vB_n) / (mA + mB);
                            double vB_n_new = (vB_n * (mB - mA) + 2 * mA * vA_n) / (mA + mB);

                            double vA_t = -updatedVelA.x * ny + updatedVelA.y * nx;
                            double vB_t = -velB.x * ny + velB.y * nx;

                            Vector newVelA = new Vector(
                                vA_n_new * nx - vA_t * ny,
                                vA_n_new * ny + vA_t * nx
                            );
                            Vector newVelB = new Vector(
                                vB_n_new * nx - vB_t * ny,
                                vB_n_new * ny + vB_t * nx
                            );

                            updatedVelA = newVelA;
                            ballB.Velocity = newVelB;
                        }
                    }
                }
            }

            lock (ballA)
            {
                ballA.Velocity = updatedVelA;
            }
        }



        private readonly UnderneathLayerAPI layerBellow;

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
