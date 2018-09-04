using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialGCodeCollisionWarning
{
    class GCodeUtil
    {
        public static List<double> GetG1XFromList(List<string> data)
        {
            List<double> values = new List<double>();

            foreach (var line in data)
            {
                if (line.Contains("X") && line.StartsWith("G1") || line.StartsWith("G0"))
                {
                    double val;
                    double.TryParse(line.Substring(line.IndexOf("X") + 1, 6), out val);
                    values.Add(val);
                }
            }

            return values;
        }

        public static List<double> GetG1YFromList(List<string> data)
        {
            List<double> values = new List<double>();

            foreach (var line in data)
            {
                if (line.Contains("Y") && line.StartsWith("G1") || line.StartsWith("G0"))
                {
                    double val;
                    double.TryParse(line.Substring(line.IndexOf("Y") + 1, 6), out val);
                    values.Add(val);
                }
            }

            return values;
        }
    }
}
