//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Reactive.Subjects;
using TP.ConcurrentProgramming.Presentation.Model;
using ModelIBall = TP.ConcurrentProgramming.Presentation.Model.IBall;

namespace TP.ConcurrentProgramming.Presentation.ViewModel.Test
{
  [TestClass]
  public class MainWindowViewModelUnitTest
  {
    [TestMethod]
    public void ConstructorTest()
    {
      ModelNullFixture nullModelFixture = new();
      Assert.AreEqual<int>(0, nullModelFixture.Disposed);
      Assert.AreEqual<int>(0, nullModelFixture.Started);
      Assert.AreEqual<int>(0, nullModelFixture.Subscribed);
      using (MainWindowViewModel viewModel = new(nullModelFixture))
      {
        Random random = new Random();
        int numberOfBalls = random.Next(1, 10);
        viewModel.BallCount = numberOfBalls;
        viewModel.StartCommand.Execute(null);
        Assert.IsNotNull(viewModel.Balls);
        Assert.AreEqual<int>(0, nullModelFixture.Disposed);
        Assert.AreEqual<int>(numberOfBalls, nullModelFixture.Started);
        Assert.AreEqual<int>(1, nullModelFixture.Subscribed);
      }
      Assert.AreEqual<int>(1, nullModelFixture.Disposed);
    }

    [TestMethod]
    public void BehaviorTestMethod()
    {
      ModelSimulatorFixture modelSimulator = new();
      MainWindowViewModel viewModel = new(modelSimulator);
      Assert.IsNotNull(viewModel.Balls);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
      Random random = new Random();
      int numberOfBalls = random.Next(1, 10);
      viewModel.BallCount = numberOfBalls;
      viewModel.StartCommand.Execute(null);
      Assert.AreEqual<int>(numberOfBalls, viewModel.Balls.Count);
      viewModel.Dispose();
      Assert.IsTrue(modelSimulator.Disposed);
      Assert.AreEqual<int>(0, viewModel.Balls.Count);
    }

    #region testing infrastructure

    private class ModelNullFixture : ModelAbstractApi
    {
      #region Test

      internal int Disposed = 0;
      internal int Started = 0;
      internal int Subscribed = 0;

      #endregion Test

      #region ModelAbstractApi

      public override event EventHandler<double> NewScaleNotification;

      public override Dimensions Dimensions => new(100.0, 100.0);

      public override void Dispose()
      {
        Disposed++;
      }

      public override void Start(int numberOfBalls)
      {
        Started = numberOfBalls;
      }

      public override void SetWindowDimensions(double height, double width)
      { }

      public override void SetDefaultWindowDimensions(double DEFAULT_WINDOW_HEIGHT, double DEFAULT_WINDOW_WIDTH)
      { }

      public override IDisposable Subscribe(IObserver<ModelIBall> observer)
      {
        Subscribed++;
        return new NullDisposable();
      }

      #endregion ModelAbstractApi

      #region private

      private class NullDisposable : IDisposable
      {
        public void Dispose()
        { }
      }

      #endregion private
    }

    private class ModelSimulatorFixture : ModelAbstractApi
    {
      #region Testing indicators

      internal bool Disposed = false;

      #endregion Testing indicators

      #region ModelAbstractApi fixture

      public override event EventHandler<double> NewScaleNotification;

      public override Dimensions Dimensions => new(100.0, 100.0);

      public override IDisposable? Subscribe(IObserver<ModelIBall> observer)
      {
        return modelBallSubject.Subscribe(observer);
      }

      public override void Start(int numberOfBalls)
      {
        for (int i = 0; i < numberOfBalls; i++)
        {
          ModelBall newBall = new ModelBall(0, 0) { };
          modelBallSubject.OnNext(newBall);
        }
      }

      public override void SetWindowDimensions(double height, double width)
      { }

      public override void SetDefaultWindowDimensions(double DEFAULT_WINDOW_HEIGHT, double DEFAULT_WINDOW_WIDTH)
      { }

      public override void Dispose()
      {
        Disposed = true;
      }

      #endregion ModelAbstractApi

      #region private

      private readonly Subject<ModelIBall> modelBallSubject = new();

      private class ModelBall : ModelIBall
      {
        public ModelBall(double top, double left)
        {
          Top = top;
          Left = left;
        }

        #region IBall

        public double Diameter => 10.0;

        public double Top { get; }

        public double Left { get; }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged = null;

        #endregion INotifyPropertyChanged

        #endregion IBall
      }

      #endregion private
    }

    #endregion testing infrastructure
  }
}