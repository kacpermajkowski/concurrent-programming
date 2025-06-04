using TP.ConcurrentProgramming.BusinessLogic;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class BusinessLogicAbstractAPIUnitTest
    {
        [TestMethod]
        public void BusinessLogicSingletonTest()
        {
            var instance1 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            var instance2 = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            Assert.AreSame(instance1, instance2);
        }

        [TestMethod]
        public void Dispose_ThrowsOnSecondCall()
        {
            var instance = BusinessLogicAbstractAPI.GetBusinessLogicLayer();
            instance.Dispose();
            Assert.ThrowsException<ObjectDisposedException>(() => instance.Dispose());
        }

        [TestMethod]
        public void GetDimensions_ReturnsExpectedValues()
        {
            var expected = new Dimensions(10.0, 10.0, 10.0);
            Assert.AreEqual(expected, BusinessLogicAbstractAPI.GetDimensions);
        }
    }
}