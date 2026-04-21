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
using static TP.ConcurrentProgramming.BusinessLogic.Ball;

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
    }

    #endregion ctor

    #region BusinessLogicAbstractAPI

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(BusinessLogicImplementation));

      if(MoveTimer != null)
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

      MoveTimer = new Timer(tickSimulation, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    }

    #endregion BusinessLogicAbstractAPI

    #region private

    private bool Disposed = false;

    private readonly DataAbstractAPI dataLayer;
    private List<Ball> BallsList = new List<Ball>();

    private Timer MoveTimer;
    private Random RandomGenerator = new();

    private void tickSimulation(object? x)
    {
      foreach (Ball item in BallsList)
      {
        double deltaX = item.Velocity.x;
        double deltaY = item.Velocity.y;

        if (item.Position.x + deltaX < 0 || item.Position.x + item.Diameter + deltaX > dataLayer.BoardWidth)
          deltaX = -deltaX;
        if (item.Position.y + deltaY < 0 || item.Position.y + item.Diameter + deltaY > dataLayer.BoardHeight)
          deltaY = -deltaY;

        item.Move(deltaX, deltaY);

        item.Velocity = new Vector(
          (RandomGenerator.NextDouble() - 0.5) * 10,
          (RandomGenerator.NextDouble() - 0.5) * 10
        );
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