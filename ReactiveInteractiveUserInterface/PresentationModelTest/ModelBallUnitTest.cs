//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//_____________________________________________________________________________________________________________________________________

using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model.Test
{
  [TestClass]
  public class ModelBallUnitTest
  {
    [TestMethod]
    public void ConstructorTestMethod()
    {
      using ModelImplementation model = new(new UnderneathLayerFixture());
      ModelBall ball = new ModelBall(0.0, 0.0, 10.0, new BusinessLogicIBallFixture(), model);
      Assert.AreEqual<double>(0.0, ball.Top);
      Assert.AreEqual<double>(0.0, ball.Top);
    }

    [TestMethod]
    public void PositionChangeNotificationTestMethod()
    {
      int notificationCounter = 0;
      using ModelImplementation model = new(new UnderneathLayerFixture());
      ModelBall ball = new ModelBall(0.0, 0.0, 10.0, new BusinessLogicIBallFixture(), model);
      ball.PropertyChanged += (sender, args) => notificationCounter++;
      Assert.AreEqual(0, notificationCounter);
      ball.SetLeft(1.0);
      Assert.AreEqual<int>(1, notificationCounter);
      Assert.AreEqual<double>(1.0, ball.Left);
      Assert.AreEqual<double>(0.0, ball.Top);
      ball.SettTop(1.0);
      Assert.AreEqual(2, notificationCounter);
      Assert.AreEqual<double>(1.0, ball.Left);
      Assert.AreEqual<double>(1.0, ball.Top);
    }

    #region testing instrumentation

    private class BusinessLogicIBallFixture : BusinessLogic.IBall
    {
      public event EventHandler<IPosition>? NewPositionNotification;

      internal void RaiseNewPosition(double x, double y)
      {
        NewPositionNotification?.Invoke(this, new PositionFixture(x, y));
      }

      private record PositionFixture(double x, double y) : IPosition;

    }

    private class UnderneathLayerFixture : BusinessLogicAbstractAPI
    {
      public override TP.ConcurrentProgramming.BusinessLogic.Dimensions Dimensions => new(100.0, 100.0);

      public override void Start(int numberOfBalls, Action<IPosition, BusinessLogic.IBall> upperLayerHandler)
      { }

      public override void Dispose()
      {
      }
    }

    #endregion testing instrumentation
  }
}