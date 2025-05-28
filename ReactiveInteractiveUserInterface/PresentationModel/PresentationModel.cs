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
using UnderneathLayerAPI = TP.ConcurrentProgramming.BusinessLogic.BusinessLogicAbstractAPI;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    /// <summary>
    /// Class Model - implements the <see cref="ModelAbstractApi" />
    /// </summary>
    internal class ModelImplementation : ModelAbstractApi
    {
        internal ModelImplementation() : this(null)
        { }

        internal ModelImplementation(UnderneathLayerAPI underneathLayer)
        {
            layerBelow = underneathLayer == null ? UnderneathLayerAPI.GetBusinessLogicLayer() : underneathLayer;
            ballChangedObservable = Observable.FromEventPattern<BallChangedEventArgs>(this, "BallChanged");
            borderSizeChangedObservable = Observable.FromEventPattern<BorderSizeChangedEventArgs>(this, "BorderSizeChanged");
        }

        #region ModelAbstractApi
        public override void Dispose()
        {
            if (Disposed)
                throw new ObjectDisposedException(nameof(Model));
            layerBelow.Dispose();
            Disposed = true;
        }

        public override IDisposable SubscribeBallChanged(IObserver<IBall> observer)
        {
            return ballChangedObservable.Subscribe(x => observer.OnNext(x.EventArgs.Ball), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override IDisposable SubscribeBorderSizeChanged(IObserver<int> observer)
        {
            return borderSizeChangedObservable.Subscribe(x => observer.OnNext(x.EventArgs.BorderSize), ex => observer.OnError(ex), () => observer.OnCompleted());
        }

        public override void Start(int numberOfBalls)
        {
            layerBelow.Start(numberOfBalls, StartHandler);
        }

        public override void UpdateWindowSize(int windowWidth, int windowHeight)
        {

            this._borderSize = windowHeight > windowWidth ? windowWidth : windowHeight;
            this._borderSize -= BORDER_SUBTRAHEND;

            if(_borderSize < 1)
                _borderSize = 1;

            BorderSizeChanged?.Invoke(this, new BorderSizeChangedEventArgs() { BorderSize = _borderSize });
        }

        #endregion ModelAbstractApi

        #region API

        public event EventHandler<BallChangedEventArgs> BallChanged;
        public event EventHandler<BorderSizeChangedEventArgs> BorderSizeChanged;

        #endregion API

        #region private
        private bool Disposed = false;

        private readonly UnderneathLayerAPI layerBelow = null;

        private readonly IObservable<EventPattern<BallChangedEventArgs>> ballChangedObservable = null;
        private readonly IObservable<EventPattern<BorderSizeChangedEventArgs>> borderSizeChangedObservable = null;

        private const int BORDER_THICKNESS = 4;
        private const int WINDOW_BAR_OFFSET = 30;
        private const int BORDER_SUBTRAHEND = 2 * BORDER_THICKNESS + WINDOW_BAR_OFFSET;
        private const int DEFAULT_BORDER_SIZE = 600 - BORDER_SUBTRAHEND;
        private int _borderSize = DEFAULT_BORDER_SIZE;

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            ModelBall newBall = new ModelBall(position.x, position.y, ball) { Diameter = 20.0 };
            BallChanged.Invoke(this, new BallChangedEventArgs() { Ball = newBall });
        }
        #endregion private

        #region TestingInfrastructure

        [Conditional("DEBUG")]
        internal void CheckObjectDisposed(Action<bool> returnInstanceDisposed)
        {
            returnInstanceDisposed(Disposed);
        }

        [Conditional("DEBUG")]
        internal void CheckUnderneathLayerAPI(Action<UnderneathLayerAPI> returnNumberOfBalls)
        {
            returnNumberOfBalls(layerBelow);
        }

        [Conditional("DEBUG")]
        internal void CheckBallChangedEvent(Action<bool> returnBallChangedIsNull)
        {
            returnBallChangedIsNull(BallChanged == null);
        }

        #endregion TestingInfrastructure
    }

    public class BallChangedEventArgs : EventArgs
    {
        public IBall Ball { get; init; }
    }

    public class BorderSizeChangedEventArgs : EventArgs
    {
        public int BorderSize { get; init; }
    }
}