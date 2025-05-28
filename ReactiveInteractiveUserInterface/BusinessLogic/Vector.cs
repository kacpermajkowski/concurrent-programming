using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal class Vector : IVector
    {
        public double x { get; init; }
        public double y { get; init; }

        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
