using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SequentialGCodeCollisionWarning
{
    class Process
    {
        public string Name { get; set;}
        public List<string> Content { get; set; }
        public List<Layer> Layers { get; set; }
        public CLIRectangle BoundingBox { get; set; }
        static int _layerCounter = 1;

        public void PopulateData()
        {
            Layers = new List<Layer>();

            bool capture = false;
            var data = new List<string>();
            foreach (var line in Content)
            {
                //Break from loop if we hit the end of the file
                if (line == ";*****ENDING SCRIPT*****")
                    break;

                if (line.Contains("layer") && line.Contains("" + _layerCounter))
                {
                    if (capture)
                    {
                        Layers.Add(new Layer(data));
                        data = new List<string>();
                    }

                    _layerCounter++;
                    capture = true;
                }

                if (capture)
                    data.Add(line);

            }
            Layers.Add(new Layer(data));

            Console.WriteLine($"Process \"{Name}\" was found with {Layers.Count} layers");

            FindLargestBoundingBox();
        }

        private void FindLargestBoundingBox()
        {
            BoundingBox = new CLIRectangle(
                Layers.Min(x => x.BoundingBox.X1),
                Layers.Min(x => x.BoundingBox.Y1),
                Layers.Max(x => x.BoundingBox.X2),
                Layers.Max(x => x.BoundingBox.Y2));
        }

        public static List<Process> CreateProcesses(List<string> gcode)
        {
            var Processes = new List<Process>();
            bool capture = false;
            var content = new List<string>();

            foreach (var line in gcode)
            {
                //We found another process. Close the current process and setup for a new one.
                if (line.StartsWith(@"; process") && content.Count != 0)
                {
                    Processes.Add(new Process()
                    {
                        Name = content[0].Substring(10),
                        Content = content
                    });

                    

                    //Reset list so we can add more things to it
                    content = new List<string>();
                }

                if (line.StartsWith(@"; process"))
                    capture = true;

                //Add to a list
                if(capture)
                    content.Add(line);
            }

            //Add the final process to the list
            Processes.Add(new Process()
            {
                Name = content[0].Substring(10),
                Content = content
            });

            return Processes;
        }
    }
}
