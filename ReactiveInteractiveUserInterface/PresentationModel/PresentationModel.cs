//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2024, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//_____________________________________________________________________________________________________________________________________

using System;
using System.Diagnostics;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.Presentation.Model
{
  /// <summary>
  /// Class Model - implements the <see cref="ModelAbstractApi" />
  /// </summary>
  internal class ModelImplementation : ModelAbstractApi
  {
    internal ModelImplementation() : this(null)
    { }

    internal ModelImplementation(BusinessLogicAbstractAPI logicLayer)
    {
      this.businessLogic = logicLayer == null ? BusinessLogicAbstractAPI.GetBusinessLogicLayer() : logicLayer;
    }

    #region ModelAbstractApi

    public override Dimensions Dimensions {
      get => new Dimensions(businessLogic.Dimensions.TableHeight, businessLogic.Dimensions.TableWidth);
    }

    public override event EventHandler<double> NewScaleNotification;

    public override void Dispose()
    {
      if (Disposed)
        throw new ObjectDisposedException(nameof(Model));
      businessLogic.Dispose();
      ballCreatedSubject.Dispose();
      Disposed = true;
    }

    public override IDisposable Subscribe(IObserver<IBall> observer)
    {
      return ballCreatedSubject.Subscribe(observer);
    }

    public override void Start(int numberOfBalls)
    {
      businessLogic.Start(numberOfBalls, BallCreatedHandler);
    }
    public override void SetWindowDimensions(double height, double width)
    {
      _windowHeight = height;
      _windowWidth = width;
      UpdateScale();
    }

    public override void SetDefaultWindowDimensions(double DEFAULT_WINDOW_HEIGHT, double DEFAULT_WINDOW_WIDTH)
    {
      _defaultWindowHeight = DEFAULT_WINDOW_HEIGHT;
      _defaultWindowWidth = DEFAULT_WINDOW_WIDTH;
      UpdateScale();
    }

    #endregion ModelAbstractApi

    #region API


    #endregion API

    #region private

    private void UpdateScale()
    {
      double tempScale1 = _windowHeight / _defaultWindowHeight;
      double tempScale2 = _windowWidth / _defaultWindowWidth;
      double tempScale = Math.Min(tempScale2, tempScale1);

      _scale = tempScale;
      NewScaleNotification?.Invoke(this, _scale);
    }

    private bool Disposed = false;

    private double _scale = 1.0;

    private double _windowHeight, _windowWidth, _defaultWindowHeight, _defaultWindowWidth;

    private readonly Subject<IBall> ballCreatedSubject = new Subject<IBall>();

    private readonly BusinessLogicAbstractAPI businessLogic = null;

    private void BallCreatedHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
    {
      ModelBall newBall = new ModelBall(position.x, position.y, 20.0, ball, this);
      ballCreatedSubject.OnNext(newBall);
    }

    #endregion private

    #region TestingInfrastructure

    [Conditional("DEBUG")]
    internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
    {
      returnInstanceDisposed(Disposed);
    }

    [Conditional("DEBUG")]
    internal void CheckUnderneathLayerAPI(Action<BusinessLogicAbstractAPI> returnNumberOfBalls)
    {
      returnNumberOfBalls(businessLogic);
    }

    [Conditional("DEBUG")]
    internal void CheckBallChangedSubject(Action<bool> returnBallChangedIsNull)
    {
      returnBallChangedIsNull(ballCreatedSubject == null);
    }

    #endregion TestingInfrastructure
  }
}