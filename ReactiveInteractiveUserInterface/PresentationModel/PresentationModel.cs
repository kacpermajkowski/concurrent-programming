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
using System.Reactive.Disposables;
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
            layerBelow.SetBorderSize(_borderSize - 2 * BORDER_THICKNESS);
            // we only initialize the value above once and then scale everything here if it changes
            // we have to subtract the border thickness because the balls are drawn inside the border
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
            this._borderSize -= WINDOW_BAR_OFFSET;

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
        private const int WINDOW_BAR_OFFSET = 40;
        private const int DEFAULT_BORDER_SIZE = 600 - WINDOW_BAR_OFFSET;
        private const int DEFAULT_BORDER_SIZE_NO_THICKNESS = DEFAULT_BORDER_SIZE - 2 * BORDER_THICKNESS;
        private int _borderSize = DEFAULT_BORDER_SIZE;

        private void StartHandler(BusinessLogic.IPosition position, BusinessLogic.IBall ball)
        {
            ModelBall newBall = new ModelBall(position.x, position.y, 20.0, GetScale, ball);
            BallChanged.Invoke(this, new BallChangedEventArgs() { Ball = newBall });
        }

        private double GetScale()
        {
            return 1.0 * (double) (_borderSize - 2 * BORDER_THICKNESS) / (double) DEFAULT_BORDER_SIZE_NO_THICKNESS;
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