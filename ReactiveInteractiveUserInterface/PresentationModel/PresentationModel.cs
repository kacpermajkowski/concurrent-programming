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

    public override double Scale { protected get; set; }

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

    #endregion ModelAbstractApi

    #region API


    #endregion API

    #region private

    private bool Disposed = false;

    private readonly Subject<IBall> ballCreatedSubject = new Subject<IBall>();

    private readonly BusinessLogicAbstractAPI businessLogic = null;

    private void BallCreatedHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
    {
      ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = 20.0 };
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