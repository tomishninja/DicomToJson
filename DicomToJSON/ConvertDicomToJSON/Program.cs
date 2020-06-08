using DicomToJSON;
using System;

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
            DicomToJSON.DicomToJson converserionFactory = new DicomToJSON.DicomToJson();


            Console.WriteLine("All Files as JSON will be prineted below: ");

            string[] Graphicaldata = converserionFactory.DicomToGraphicalJSONArray(PathToFiles);

            foreach(string sample in Graphicaldata)
            {
                Console.WriteLine(sample);
            }

            Console.WriteLine("All files printed as a single condensed JSON will be writent below: ");

            Console.WriteLine(converserionFactory.DicomToGraphicalJSON(PathToFiles));

            Console.ReadKey();
        }
    }
}
