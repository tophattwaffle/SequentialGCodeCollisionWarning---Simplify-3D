using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;


//.\SequentialGCodeCollisionWarning.exe "/S 15" "/E 20" "/W 25"
namespace SequentialGCodeCollisionWarning
{
    class Program
    {
        public static string Path;
        public static int South = -1;
        public static int East = -1;
        public static int West = -1;
        public static bool Sequential = true;
        public static bool IssuesFound = false;

        static void Main(string[] args)
        {


            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.White;
            if (args.Length == 0)
            {
                DisplayHelp();
            }

            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "/H":
                        DisplayHelp();
                        break;
                    case "/F":
                        Path = args[i+1];
                        break;
                    case "/S":
                        if (Int32.TryParse(args[i+1], out South))
                            Console.WriteLine($"South: {South}");
                        else
                            InvalidCardinalDirection(args[i]);
                        break;
                    case "/E":
                        if (Int32.TryParse(args[i+1], out East))
                            Console.WriteLine($"East: {East}");
                        else
                            InvalidCardinalDirection(args[i]);
                        break;
                    case "/W":
                        if (Int32.TryParse(args[i+1], out West))
                            Console.WriteLine($"West: {West}");
                        else
                            InvalidCardinalDirection(args[i]);
                        break;
                }
            }

            //Path = @"C:\TEMP\Ender3_CR10_x3_Feet.gcode";
            //Path = @"C:\TEMP\X3_CableManagement.gcode";
            //Path = @"C:\TEMP\Ender3_CR10_Feet.gcode";
            //Path = @"C:\TEMP\CR10_115%_4x_Error_Seq_04mmHeight.gcode";
            //Path = @"C:\TEMP\Top Hat.gcode";

            if (Path == null || South == -1 || East == -1 || West == -1)
                DisplayHelp();

            Console.WriteLine("File: " + Path);

            var fileRawList = new List<string>();
            var fileStream = new FileStream(Path, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    fileRawList.Add(line);
                }
            }

            Console.WriteLine(fileRawList.Count + " Lines in GCODE file\nParsing out Processes...");

            var processes = Process.CreateProcesses(fileRawList);

            Console.WriteLine(processes.Count + " Processes Found!");

            foreach (var process in processes)
            {
                process.PopulateData();

                //We have reason to believe this is just multiple processes, not sequential printing
                //If layer height is over 1, that has to mean we are no sequential.
                if (process.Layers[0].ZHeight > 1)
                    SetNotSequential();
            }

            if(processes.Count > 1 && Sequential)
                FindColissions(processes);
            else
                Console.WriteLine("Collision Detection will be skipped, only 1 process was found, or this is not a sequential print.");

            if (IssuesFound)
            {
                var simpleSound = new SoundPlayer(@"C:\Windows\Media\chimes.wav");
                simpleSound.PlaySync();
                Console.ReadLine();
            }
            else
                System.Threading.Thread.Sleep(5000);


            System.Environment.Exit(0);
        }

        public static void FindColissions(List<Process> proc)
        {
            Console.WriteLine("Finding Potential Collisions...");

            //Loop though backwards until we are done with the list.
            for (int i = proc.Count; i --> 0;)
            {
                //Bail from for loop, we don't need to compare at process 0.
                if (i == 0)
                    break;

                for (int j = 0; j < i; j++)
                {
                    if (proc[i].BoundingBox.Intersects(proc[j].BoundingBox))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"POSSIBLE CARRIAGE COLISSION BETWEEN {proc[i].Name} AND {proc[j].Name}!!!");
                        IssuesFound = true;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"No issues found between {proc[i].Name} and {proc[j].Name}.");
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void InvalidCardinalDirection(string direction)
        {
            Console.WriteLine($"Invalid cardinal direction. You input {direction} which I cannot find an int value in.\n\n");
            DisplayHelp();
        }

        public static void DisplayHelp()
        {
            Console.WriteLine("\nHelp\n/H - Displays this message.\n\n" +
                              "/F [FULL PATH TO FILE] - REQUIRED GCODE file to analyze.\nExample: /F C:\\SlicedFiles\\Object.gcode\n\n" +
                              "/S # - REQUIRED When looking top down, the carriage clearance distance from the nozzle to the min Y (Nozzle to front of bed) value\n" +
                              "/E # - REQUIRED When looking top down, the carriage clearance distance from the nozzle to the max X (Nozzle to right of X gantry) value\n" +
                              "/W # - REQUIRED When looking top down, the carriage clearance distance from the nozzle to the max X (Nozzle to left of X gantry) value\n");
            Console.ReadLine();
            System.Environment.Exit(0);
        }

        public static void SetNotSequential()
        {
            Sequential = false;
        }
    }
}
