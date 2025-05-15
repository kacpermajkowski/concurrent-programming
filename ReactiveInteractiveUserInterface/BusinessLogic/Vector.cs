using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data;

namespace TP.ConcurrentProgramming.BusinessLogic
{
    internal record Vector : IVector
    {
        public double x { get; init; }
        public double y { get; init; }
        public Vector(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector(IVector v)
        {
            this.x = v.x;
            this.y = v.y;
        }
    }
}
