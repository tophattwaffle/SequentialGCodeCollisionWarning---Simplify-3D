using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialGCodeCollisionWarning
{
    class Layer
    {
        public double ZHeight;
        public List<string> RawData { get; set; }
        public List<string> Skirt { get; } //1 ; skirt
        public List<string> InnerPerimeter { get; } //2 ; inner perimeter
        public List<string> OuterPerimeter { get; } //3 ; outer perimeter
        public List<string> InternalSingleExtrusion { get; } //4 ; internal single extrusion
        public List<string> Support { get; } //5 ; support
        public List<string> Infill { get; } //6 ; infill
        public List<string> SolidLayer { get; } //7 ; solid layer
        public List<string> ExternalSingleExtrusion { get; } //8 ; external single extrusion
        public List<string> Bridge { get; } //9 ; bridge
        public List<string> DenseSupport { get; } //10 ; dense support
        public CLIRectangle BoundingBox { get; set; }

        public Layer(List<string> data)
        {
            RawData = data;
            SetZHeight();

            Skirt = new List<string>();
            InnerPerimeter = new List<string>();
            OuterPerimeter = new List<string>();
            InternalSingleExtrusion = new List<string>();
            Support = new List<string>();
            Infill = new List<string>();
            SolidLayer = new List<string>();
            ExternalSingleExtrusion = new List<string>();
            Bridge = new List<string>();
            DenseSupport = new List<string>();

            int recordValue = 0;

            foreach (var line in data)
            {
                if (line.StartsWith(";"))
                {
                    if (line == "; skirt")
                        recordValue = 1;
                    else if (line == "; inner perimeter")
                        recordValue = 2;
                    else if (line == "; outer perimeter")
                        recordValue = 3;
                    else if (line == "; internal single extrusion")
                        recordValue = 4;
                    else if (line == "; support")
                        recordValue = 5;
                    else if (line == "; infill")
                        recordValue = 6;
                    else if (line == "; solid layer")
                        recordValue = 7;
                    else if (line == "; external single extrusion")
                        recordValue = 8;
                    else if (line == "; bridge")
                        recordValue = 9;
                    else if (line == "; dense support")
                        recordValue = 10;
                }

                switch (recordValue)
                {
                    case 1:
                        Skirt.Add(line);
                        break;
                    case 2:
                        InnerPerimeter.Add(line);
                        break;
                    case 3:
                        OuterPerimeter.Add(line);
                        break;
                    case 4:
                        InternalSingleExtrusion.Add(line);
                        break;
                    case 5:
                        Support.Add(line);
                        break;
                    case 6:
                        Infill.Add(line);
                        break;
                    case 7:
                        SolidLayer.Add(line);
                        break;
                    case 8:
                        ExternalSingleExtrusion.Add(line);
                        break;
                    case 9:
                        Bridge.Add(line);
                        break;
                    case 10:
                        DenseSupport.Add(line);
                        break;
                    case 0:
                        break;
                }
            }

            BoundingBox = new CLIRectangle(OuterPerimeter);
        }

        private void SetZHeight()
        {
            double.TryParse(RawData[0].Substring(RawData[0].IndexOf("=", StringComparison.Ordinal) + 2), out ZHeight);
        }
    }
}
