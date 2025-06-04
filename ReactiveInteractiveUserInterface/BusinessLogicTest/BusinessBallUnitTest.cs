using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessBallUnitTest
    {
        [TestMethod]
        public void NewPositionNotification_IsForwarded()
        {
            var dataBall = new DataBallFixture();
            var businessBall = new Ball(dataBall);
            int callbackCount = 0;
            businessBall.NewPositionNotification += (s, p) => callbackCount++;
            dataBall.Move();
            Assert.AreEqual(1, callbackCount);
        }

        private class DataBallFixture : Data.IBall
        {
            public Data.IVector Velocity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
            public event EventHandler<Data.IVector>? NewPositionNotification;
            public double Diameter => 1.0;
            public double GetMass() => 1.0;
            public Data.IVector GetPosition() => new VectorFixture(0.0, 0.0);
            public void Move(Data.IVector v) => NewPositionNotification?.Invoke(this, v);
            // Dla testu wywołujemy Move bez parametru
            public void Move() => NewPositionNotification?.Invoke(this, new VectorFixture(1.0, 2.0));
        }
        
        private class VectorFixture : Data.IVector
        {
            public VectorFixture(double x, double y) { this.x = x; this.y = y; }
            public double x { get; init; }
            public double y { get; init; }
        }
    }
}