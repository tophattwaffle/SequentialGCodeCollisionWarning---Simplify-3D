using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialGCodeCollisionWarning
{
    class CLIRectangle
    {
        //Bottom left
        public double X1 { get; set; } // X1 < X2
        public double Y1 { get; set; } // Y1 < Y2

        //Top right
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public CLIRectangle(List<string> data)
        {
            var xValues = GCodeUtil.GetG1XFromList(data);
            var yValues = GCodeUtil.GetG1YFromList(data);

            X2 = xValues.Max();
            X1 = xValues.Min();
            Y2 = yValues.Max();
            Y1 = yValues.Min();
        }

        public CLIRectangle(double x1, double y1, double x2, double y2)
        {
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        public Boolean Intersects(CLIRectangle that)
        {
            if((that.X2 >= X1 + (Program.West * - 1) && that.X1 <= X2 + Program.East) && (that.Y2 >= Y1 + (Program.South * -1) && that.Y1 <= Y2))
                return true;

            return false;
        }

        public override string ToString()
        {
            return $"LOWER LEFT X1:{X1} Y1:{Y1}\nUPPER RIGHT X2:{X2} Y2:{Y2}";
        }
    }
}
