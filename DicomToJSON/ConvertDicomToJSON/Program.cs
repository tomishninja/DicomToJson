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

        public enum Operation
        {
            //Help,//Not used due to the type of operation that this is
            Test,
            Array,
            Volume
        };

        private static readonly Operation DefaultOperation = Operation.Test;

        private const string DefaultFileName = "VolumetricData";



        static void Main(string[] args)
        {
            // this verible will tell the system were the data is comming from
            string input = "";

            // this verible will tell the system were to leave the excess data
            string output = "";

            // this verible will detail the name of the file.
            string fileName = DefaultFileName;

            // this will determine the behaviour of the application. it is set ot the default 
            Operation operation = DefaultOperation;

            // validate the user input
            if (args.Length > 1)
            {
                if (args[0].Trim().ToLower().Equals("help"))
                {
                    // help info below

                    // don't go though the rest just return to the console
                    return;
                }
                else if (args[0].Trim().ToLower().Equals("array"))
                {
                    operation = Operation.Array;
                    ParseInput(args, ref input, ref output, ref fileName, 1);
                }
                else if (args[0].Trim().ToLower().Equals("volume"))
                {
                    operation = Operation.Volume;
                    ParseInput(args, ref input, ref output, ref fileName, 1);
                }
                else if (args[0].Trim().ToLower().Equals("test"))
                {
                    operation = Operation.Volume;
                    ParseInput(args, ref input, ref output, ref fileName, 1);
                }
                else if (Directory.Exists(args[0]))
                {
                    ParseInput(args, ref input, ref output, ref fileName);
                }
                else
                {
                    throw new ArgumentException("Invalid argmuments entered into the command line, The first argmuent must be a valid directory");
                }
            }
            else
            {
                // try to help the user run the system if they don't input any extra arguments.
                throw new ArgumentException("No arguments given. please input a valid directory to get data from and write data to or for more information call the \"help\" command");
            }

            // if the last couple of letters are not the json subscript add them to the end file names
            
            // Create the object that will load in all of the JSON files
            DicomToJSON.DicomToJson converserionFactory = new DicomToJSON.DicomToJson();

            switch (operation)
            {
                case Operation.Test:

                    // store each file in 
                    Console.WriteLine("All Files as JSON will be printed below: ");

                    string[] Graphicaldata = converserionFactory.DicomToGraphicalJSONArray(input);

                    foreach (string sample in Graphicaldata)
                    {
                        Console.WriteLine(sample);
                    }

                    Console.WriteLine("All files printed as a single condensed JSON will be writent below: ");

                    Console.WriteLine(converserionFactory.DicomToGraphicalJSON(input));
                    break;
                case Operation.Array:
                    Graphicaldata = converserionFactory.DicomToGraphicalJSONArray(input);

                    // TODO if the file name ends with .json then we need to remove it from the string because it will need to beplaced on to the end of the file

                    for (int index = 0; index < Graphicaldata.Length; index++)
                    {
                        // TODO fill this in with writing all the files to the output dir
                    }
                    break;
                case Operation.Volume:
                    // TODO write output of method below to a file.  
                    //converserionFactory.DicomToGraphicalJSON(input);
                    break;
            }


            Console.WriteLine("successfuly parsed files.");
            //Console.ReadKey();
        }

        public static void ParseInput(string[] inputs, ref string input, ref string output, ref string fileName, int startindex = 0)
        {
            // validate start index
            if (startindex < 0 || startindex >= inputs.Length) throw new ArgumentOutOfRangeException("start Index must be a valid index within the array");

            // depending on the amount of values assign the correct data to the file names 
            if (inputs.Length - startindex == 1)
            {
                if (Directory.Exists(inputs[0]) && Directory.Exists(inputs[1]))
                {
                    // if only one argument is given then assume that 
                    input = inputs[startindex];
                    output = inputs[startindex];
                }
                else
                {
                    // invalid input so throw an exception
                    throw new ArgumentException("Invalid argmuments entered into the command line, The two first argmuent must be a valid directory");
                }
            }
            else if (inputs.Length - startindex == 1)
            {
                if (Directory.Exists(inputs[0]) && Directory.Exists(inputs[1]))
                {
                    // the user provided an input and output dir
                    input = inputs[startindex];
                    output = inputs[++startindex];
                }
                else if (Directory.Exists(inputs[0]) && inputs[1] != null && inputs[1].Length > 0)
                {
                    // if there is something in the second value but it isn't a dir than assume it was supposed to be a file name
                    // input and output dirs are the same but the file name is different
                    input = inputs[startindex];
                    output = inputs[startindex];
                    fileName = inputs[++startindex];
                }
                else
                {
                    // invalid input so throw an exception
                    throw new ArgumentException("Invalid argmuments entered into the command line, The two first argmuent must be a valid directory");
                }
            }
            else if (inputs.Length >= 3)
            {
                // all data was provided use each arg as normal if it is invalid then throw an exception
                if (Directory.Exists(inputs[startindex]) && Directory.Exists(inputs[1 + startindex]) && inputs[2 + startindex] != null && inputs[2 + startindex].Length > 0)
                {
                    input = inputs[startindex];
                    output = inputs[++startindex];
                    fileName = inputs[++startindex];
                }
                else
                {
                    throw new ArgumentException("Invalid argmuments entered into the command line, The two first argmuent must be a valid directory");
                }
            }
        }
    }
}
