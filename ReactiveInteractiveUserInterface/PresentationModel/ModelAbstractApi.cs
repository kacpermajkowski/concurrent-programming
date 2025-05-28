//__________________________________________________________________________________________
//
//  Copyright 2024 Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and to get started
//  comment using the discussion panel at
//  https://github.com/mpostol/TP/discussions/182
//__________________________________________________________________________________________

using System;
using System.ComponentModel;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    public interface IBall : INotifyPropertyChanged
    {
        double Top { get; }
        double Left { get; }
        double Diameter { get; }
    }

    public abstract class ModelAbstractApi : IObservable<IBall>, IObservable<int>, IDisposable
    {
        public static ModelAbstractApi CreateModel()
        {
            return modelInstance.Value;
        }

        public abstract void Start(int numberOfBalls);
        public abstract void UpdateWindowSize(int windowWidth, int windowHeight);


        #region IObservable

        public IDisposable Subscribe(IObserver<IBall> observer)
        {
            return SubscribeBallChanged(observer);
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return SubscribeBorderSizeChanged(observer);
        }
        public abstract IDisposable SubscribeBallChanged(IObserver<IBall> observer);
        public abstract IDisposable SubscribeBorderSizeChanged(IObserver<int> observer);

        #endregion IObservable

        #region IDisposable

        public abstract void Dispose();

        #endregion IDisposable

        #region private

        private static Lazy<ModelAbstractApi> modelInstance = new Lazy<ModelAbstractApi>(() => new ModelImplementation());

        #endregion private
    }
}