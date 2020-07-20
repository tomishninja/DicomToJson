using DicomToJSON;
using System;
using System.IO;

namespace ConsoleTest
{
    class Program
    {
        /// <summary>
        /// This the location of the directory that will be read
        /// </summary>
        private static string PathToFiles = "C:\\Users\\admin\\Downloads\\Head_Axial_DICOM_2461_CTHead10.dcm_332";

        
        static void Main(string[] args)
        {
            // this verible will tell the system were the data is comming from
            string input = "";

            // this verible will tell the system were to leave the excess data
            string output = "";

            // this verible will detail the name of the file.
            string fileName = "Volumetric Data";


            // validate the user input
            if (args.Length == 1)
            {
                if (args[0].Trim().ToLower().Equals("help"))
                {
                    // help info below
                }
                if (Directory.Exists(args[0]))
                {
                    input = args[0];
                    output = args[0];
                }
                else
                {
                    throw new ArgumentException("Invalid argmuments entered into the command line, The first argmuent must be a valid directory");
                }
            }
            else if (args.Length >= 2)
            {
                if (Directory.Exists(args[0]) && Directory.Exists(args[1]))
                {
                    input = args[0];
                    output = args[1];
                }
                else
                {
                    throw new ArgumentException("Invalid argmuments entered into the command line, The two first argmuent must be a valid directory");
                }
            }
            else if (args.Length == 3)
            {
                if (Directory.Exists(args[0]) && Directory.Exists(args[1]))
                {
                    input = args[0];
                    output = args[1];
                    fileName = args[2];
                }
                else
                {
                    throw new ArgumentException("Invalid argmuments entered into the command line, The two first argmuent must be a valid directory");
                }
            }
            else
            {
                //throw new ArgumentException("No arguments given please input a valid directory to get data from");
                // if no arguments are given run the test system
            }

            // Create the object that will load in all of the JSON files
            DicomToJSON.DicomToJson converserionFactory = new DicomToJSON.DicomToJson();

            // store each file in 
            Console.WriteLine("All Files as JSON will be printed below: ");

            string[] Graphicaldata = converserionFactory.DicomToGraphicalJSONArray(input);

            foreach(string sample in Graphicaldata)
            {
                Console.WriteLine(sample);
            }

            Console.WriteLine("All files printed as a single condensed JSON will be writent below: ");

            Console.WriteLine(converserionFactory.DicomToGraphicalJSON(input));



            Console.ReadKey();
        }
    }
}
