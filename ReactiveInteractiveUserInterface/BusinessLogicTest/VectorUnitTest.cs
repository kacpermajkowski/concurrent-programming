using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.BusinessLogic.Test
{
    [TestClass]
    public class VectorUnitTest
    {
        [TestMethod]
        public void ConstructorTestMethod()
        {
            Random random = new Random();
            double initialX = random.NextDouble();
            double initialY = random.NextDouble();
            Data.IVector vector = new Vector(initialX, initialY);
            Assert.AreEqual<double>(initialX, vector.x);
            Assert.AreEqual<double>(initialY, vector.y);
        }
    }
}