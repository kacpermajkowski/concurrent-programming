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
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  internal class BusinessLogicImplementation : BusinessLogicAbstractAPI
  {
    #region ctor

    public BusinessLogicImplementation() : this(null)
    {
    }

    internal BusinessLogicImplementation(DataAbstractAPI? dataLayer)
    {
      this.dataLayer = dataLayer == null ? DataAbstractAPI.GetDataLayer() : dataLayer;
      MoveTimer = new Timer(tickSimulation, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

      MoveTimer.Dispose();
      dataLayer.Dispose();

      foreach (Ball ball in BallsList)
      {
          ball.Dispose();
      }
      BallsList.Clear();

      Disposed = true;
    }

    public override void Start(int numberOfBalls, Action<IPosition, IBall> ballCreatedHandler)
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));
      if (ballCreatedHandler == null)
        throw new ArgumentNullException(nameof(ballCreatedHandler));

      BallsList.Clear();

      dataLayer.Start(numberOfBalls, (startingPosition, databall) => 
      {
        Ball logicBall = new Ball(databall);
        BallsList.Add(logicBall);
        ballCreatedHandler(new Position(startingPosition.x, startingPosition.y), logicBall);
      });
    }

    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;

    private readonly DataAbstractAPI dataLayer;
    private List<Ball> BallsList = new List<Ball>();

    private readonly Timer MoveTimer;
    private Random RandomGenerator = new();

    private void tickSimulation(object? x)
    {
      foreach (Ball item in BallsList)
        item.Move(
          (RandomGenerator.NextDouble() - 0.5) * 10,
          (RandomGenerator.NextDouble() - 0.5) * 10
          );
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