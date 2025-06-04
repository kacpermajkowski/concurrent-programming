using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.Data.Test
{
    [TestClass]
    public class BallUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Vector testVector = new Vector(0.0, 0.0);
            Ball newInstance = new(testVector, testVector, 1.0, 1.0);
            Assert.IsNotNull(newInstance);
        }

        [TestMethod]
        public void MoveTestMethod()
        {
            Vector initialPosition = new(10.0, 10.0);
            Ball newInstance = new(initialPosition, new Vector(0.0, 0.0), 1.0, 1.0);
            IVector currentPosition = new Vector(0.0, 0.0);
            int numberOfCallBackCalled = 0;
            newInstance.NewPositionNotification += (sender, position) => { Assert.IsNotNull(sender); currentPosition = position; numberOfCallBackCalled++; };
            newInstance.Move(new Vector(0.0, 0.0));
            Assert.AreEqual(1, numberOfCallBackCalled);
            Assert.AreEqual(initialPosition, currentPosition);
        }

        [TestMethod]
        public void GetPositionTestMethod()
        {
            Vector initialPosition = new(20.0, 20.0);
            Ball newBall = new(initialPosition, initialPosition, 1.0, 1.0);
            Assert.AreEqual(initialPosition, newBall.GetPosition());
        }
    }
}