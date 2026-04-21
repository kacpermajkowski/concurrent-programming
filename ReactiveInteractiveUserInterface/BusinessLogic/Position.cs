using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.BusinessLogic
{
  public class Position : IPosition
  {
    public Position(double x, double y)
    {
      this.x = x;
      this.y = y;
    }
    public double x { get; init; }
    public double y { get; init; }
  }
}
