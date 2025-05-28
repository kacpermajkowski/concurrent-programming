//____________________________________________________________________________________________________________________________________
//
//  Copyright (C) 2023, Mariusz Postol LODZ POLAND.
//
//  To be in touch join the community by pressing the `Watch` button and get started commenting using the discussion panel at
//
//  https://github.com/mpostol/TP/discussions/182
//
//  by introducing yourself and telling us what you do with this community.
//_____________________________________________________________________________________________________________________________________

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using TP.ConcurrentProgramming.BusinessLogic;
using LogicIBall = TP.ConcurrentProgramming.BusinessLogic.IBall;

namespace TP.ConcurrentProgramming.Presentation.Model
{
    internal class ModelBall : IBall
    {
        public ModelBall(
                        double top, double left, double diameter,
                        Func<double> scaler,
                        LogicIBall underneathBall)
        {
            _top = top;
            _left = left;
            Diameter = diameter; 
            OriginalDiameter = diameter;
            this.GetScale = scaler ?? throw new ArgumentNullException(nameof(scaler));
            underneathBall.NewPositionNotification += NewPositionNotification;
        }

        #region IBall

        public double Top
        {
            get { return _top; }
            private set
            {
                if (_top == value)
                    return;
                _top = value;
                RaisePropertyChanged();
            }
        }

        public double Left
        {
            get { return _left; }
            private set
            {
                if (_left == value)
                    return;
                _left = value;
                RaisePropertyChanged();
            }
        }

        public double Diameter
        {
            get { return _diameter; }
            private set
            {
                if (_diameter == value)
                    return;
                _diameter = value;
                RaisePropertyChanged();
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion INotifyPropertyChanged

        #endregion IBall

        #region private

        private double _top;
        private double _left;
        private double _diameter = 20.0;
        private readonly double OriginalDiameter;

        private void NewPositionNotification(object sender, IPosition e)
        {
            Top = e.y * GetScale.Invoke(); 
            Left = e.x * GetScale.Invoke();
            Diameter = OriginalDiameter * GetScale.Invoke();
        }

        private readonly Func<double> GetScale;

        private void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion private

        #region testing instrumentation

        [Conditional("DEBUG")]
        internal void SetLeft(double x)
        { Left = x; }

        [Conditional("DEBUG")]
        internal void SettTop(double x)
        { Top = x; }

        #endregion testing instrumentation
    }
}