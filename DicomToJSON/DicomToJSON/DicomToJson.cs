using System;
using System.Collections;
using System.Collections.Generic;
using Dicom;
using Dicom.Imaging;
using Dicom.Imaging.Render;

namespace DicomToJSON
{
    public class DicomToJson
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePathToDir"></param>
        /// <returns></returns>
        public string[] DicomToGraphicalJSONArray(string filePathToDir)
        {
            string[] allFilePaths = System.IO.Directory.GetFiles(filePathToDir);

            // store all of the dicoms as a list of dicom files
            List<DicomFile> dicoms = new List<DicomFile>();

            // the output datastructure for this method
            string[] jsonStrings = null;

            // loop though all of the files and open them 
            foreach (string filePath in allFilePaths)
            {
                try
                {
                    dicoms.Add(DicomFile.Open(filePath));
                }
                catch (Exception)
                {
                    Console.WriteLine("file at \"" + filePath + "\" readable as a dicom file");
                }
            }

            // set the amount of output files to the amount of Json Strings
            jsonStrings = new string[dicoms.Count];

            // sort the order of all the dicom objects so they work in the correct order
            dicoms.Sort((d1, d2) => d1.Dataset.GetSingleValue<int>(DicomTag.InstanceNumber).CompareTo(d2.Dataset.GetSingleValue<int>(DicomTag.InstanceNumber)));

            // loop though all of the dicoms in order now
            for (int index = 0; index < jsonStrings.Length; index++)
            {
                int bytesUsed = -1;
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.BitsAllocated, out bytesUsed))
                {
                    // this is required for all dicom images so if this code has been reached the code is invalid
                    Console.WriteLine("A invalid file was found at index " + index);
                    continue;
                }
                Console.WriteLine("Amount of Bytes Required: " + bytesUsed);

                // get the amount of data 
                DicomFileData dicomFile = null;

                // get the pixel data out as much as possible
                DicomPixelData header = DicomPixelData.Create(dicoms[index].Dataset);
                var pixelData = PixelDataFactory.Create(header, 0);

                // Build the correct file for this data depending on what is stored in the dicom files
                if (pixelData is GrayscalePixelDataU16)
                {
                    dicomFile = new U16DicomFileData(((GrayscalePixelDataU16)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataS16)
                {
                    dicomFile = new S16DicomFileData(((GrayscalePixelDataS16)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataU8)
                {
                    dicomFile = new U8bitDicomFileData(((GrayscalePixelDataU8)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataS32)
                {
                    dicomFile = new S32DicomDataFile(((GrayscalePixelDataS32)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataU32)
                {
                    dicomFile = new U32DicomDataFile(((GrayscalePixelDataU32)pixelData).Data);
                }
                else if (pixelData is ColorPixelData24)
                {
                    // note that this one will take some work
                    // 0 == red, 1 == Green, 2 == Blue and so on
                    dicomFile = new RGBColourDicomFileData(((ColorPixelData24)pixelData).Data);
                }
                else if (pixelData is SingleBitPixelData)
                {
                    dicomFile = new RGBColourDicomFileData(((SingleBitPixelData)pixelData).Data);
                }

                // get the amount of rows the image data set has out of the dicom file.
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.Rows, out dicomFile.width))
                {
                    if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.NumberOfVerticalPixels, out dicomFile.width))
                    {
                        // if the value for rows dosn't exist mark the entry as -1
                        dicomFile.width = -1;
                    }
                }

                // get the amount of coloumns in the image
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.Columns, out dicomFile.height))
                {
                    // try getting this data  another way then
                    if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.NumberOfHorizontalPixels, out dicomFile.height))
                    {
                        // if the value for rows dosn't exist mark the entry as -1
                        dicomFile.height = -1;
                    }
                }

                float[] temp = new float[2];
                // get the amount of coloumns in the image
                if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PixelSpacing, out temp))
                {
                    // all these methods will try to do it until one works or this feild will be made invalid
                    if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.ImagerPixelSpacing, out temp))
                    {
                        if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.NominalScannedPixelSpacing, out temp))
                        {
                            if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.CompensatorPixelSpacing, out temp))
                            {
                                if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.DetectorElementSpacing, out temp))
                                {
                                    if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PresentationPixelSpacing, out temp))
                                    {
                                        if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PrinterPixelSpacing, out temp))
                                        {
                                            if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.ObjectPixelSpacingInCenterOfBeam, out temp))
                                            {
                                                // if none of the above methods work mark the objects as invalid
                                                temp[0] = -1;
                                                temp[1] = -1;
                                                // This could be valid in a case were only one row of data was colected
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // TODO this dosn't take into account what will happen if the user only recorded one line of pixels since my work dosn't focus on it

                // This should always return a value that is has a length of two
                // This will be the value for each row which should always be the first value found
                dicomFile.pixelSpacingX = temp[0];
                dicomFile.pixelSpacingY = temp[1];

                // try to get the thickness of the the slice or else set it to an invalid number
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.SliceThickness, out dicomFile.sliceThickness))
                {
                    dicomFile.sliceThickness = -1;
                }

                // try to get how much room is between each slice
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.SpacingBetweenSlices, out dicomFile.spacingBetweenSlices))
                {
                    dicomFile.spacingBetweenSlices = -1;
                }

                // get the JSON output as a string and place it to the correct place in the array
                jsonStrings[index] = Newtonsoft.Json.JsonConvert.SerializeObject(dicomFile);
            }

            // return the json strings here
            return jsonStrings;
        }


        public string DicomToGraphicalJSON(string filePathToDir)
        {
            string[] allFilePaths = System.IO.Directory.GetFiles(filePathToDir);

            // store all of the dicoms as a list of dicom files
            List<DicomFile> dicoms = new List<DicomFile>();

            // get the amount of data 
            DicomFileData dicomFile = null;

            ArrayList data = new ArrayList();

            // loop though all of the files and open them 
            foreach (string filePath in allFilePaths)
            {
                try
                {
                    dicoms.Add(DicomFile.Open(filePath));
                }
                catch (Exception)
                {
                    Console.WriteLine("file at \"" + filePath + "\" readable as a dicom file");
                }
            }

            // sort the order of all the dicom objects so they work in the correct order
            dicoms.Sort((d1, d2) => d1.Dataset.GetSingleValue<int>(DicomTag.InstanceNumber).CompareTo(d2.Dataset.GetSingleValue<int>(DicomTag.InstanceNumber)));

            // sort out the intial values based on the first dicom in the list
            // get the pixel data out as much as possible
            DicomPixelData header = DicomPixelData.Create(dicoms[0].Dataset);
            IPixelData pixelData = PixelDataFactory.Create(header, 0);

            // Build the correct file for this data depending on what is stored in the dicom files
            if (pixelData is GrayscalePixelDataU16)
            {
                data.AddRange(((GrayscalePixelDataU16)pixelData).Data);
                dicomFile = new U16DicomFileData();
            }
            else if (pixelData is GrayscalePixelDataS16)
            {
                data.AddRange(((GrayscalePixelDataS16)pixelData).Data);
                dicomFile = new S16DicomFileData();
            }
            else if (pixelData is GrayscalePixelDataU8)
            {
                data.AddRange(((GrayscalePixelDataU8)pixelData).Data);
                dicomFile = new U8bitDicomFileData();
            }
            else if (pixelData is GrayscalePixelDataS32)
            {
                data.AddRange(((GrayscalePixelDataS32)pixelData).Data);
                dicomFile = new S32DicomDataFile();
            }
            else if (pixelData is GrayscalePixelDataU32)
            {
                data.AddRange(((GrayscalePixelDataU32)pixelData).Data);
                dicomFile = new U32DicomDataFile();
            }
            else if (pixelData is ColorPixelData24)
            {
                // note that this one will take some work
                // 0 == red, 1 == Green, 2 == Blue and so on
                data.AddRange(((ColorPixelData24)pixelData).Data);
                dicomFile = new RGBColourDicomFileData();
            }
            else if (pixelData is SingleBitPixelData)
            {
                data.AddRange(((SingleBitPixelData)pixelData).Data);
                dicomFile = new U8bitDicomFileData();
            }

            // get the amount of rows the image data set has out of the dicom file.
            if (!dicoms[0].Dataset.TryGetSingleValue(DicomTag.Rows, out dicomFile.width))
            {
                if (!dicoms[0].Dataset.TryGetSingleValue(DicomTag.NumberOfVerticalPixels, out dicomFile.width))
                {
                    // if the value for rows dosn't exist mark the entry as -1
                    dicomFile.width = -1;
                }
            }

            // get the amount of coloumns in the image
            if (!dicoms[0].Dataset.TryGetSingleValue(DicomTag.Columns, out dicomFile.height))
            {
                // try getting this data  another way then
                if (!dicoms[0].Dataset.TryGetSingleValue(DicomTag.NumberOfHorizontalPixels, out dicomFile.height))
                {
                    // if the value for rows dosn't exist mark the entry as -1
                    dicomFile.height = -1;
                }
            }

            // Datastucture that will hold the final DS
            float[] aspectOfPixel = new float[2];


            // get the amount of coloumns in the image
            if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.PixelSpacing, out aspectOfPixel))
            {
                // all these methods will try to do it until one works or this feild will be made invalid
                if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.ImagerPixelSpacing, out aspectOfPixel))
                {
                    if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.NominalScannedPixelSpacing, out aspectOfPixel))
                    {
                        if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.CompensatorPixelSpacing, out aspectOfPixel))
                        {
                            if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.DetectorElementSpacing, out aspectOfPixel))
                            {
                                if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.PresentationPixelSpacing, out aspectOfPixel))
                                {
                                    if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.PrinterPixelSpacing, out aspectOfPixel))
                                    {
                                        if (!dicoms[0].Dataset.TryGetValues<float>(DicomTag.ObjectPixelSpacingInCenterOfBeam, out aspectOfPixel))
                                        {
                                            // if none of the above methods work mark the objects as invalid
                                            aspectOfPixel[0] = -1;
                                            aspectOfPixel[1] = -1;
                                            // This could be valid in a case were only one row of data was colected
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // TODO this dosn't take into account what will happen if the user only recorded one line of pixels since my work dosn't focus on it

            // Save the data from pixel spacing
            dicomFile.pixelSpacingX = aspectOfPixel[0];
            dicomFile.pixelSpacingY = aspectOfPixel[1];

            // try to get the thickness of the the slice or else set it to an invalid number

            if (dicoms[0].Dataset.TryGetSingleValue(DicomTag.SliceThickness, out dicomFile.sliceThickness))
            {
                dicomFile.sliceThickness = -1;
            }

            // try to get how much room is between each slice
            if (!dicoms[0].Dataset.TryGetSingleValue(DicomTag.SpacingBetweenSlices, out dicomFile.spacingBetweenSlices))
            {
                dicomFile.spacingBetweenSlices = -1;
            }

            dicomFile.breath = dicoms.Count;

            // loop though all of the dicoms in order now
            for (int index = 1; index < dicoms.Count; index++)
            {
                // get the pixel data out as much as possible
                header = DicomPixelData.Create(dicoms[index].Dataset);
                pixelData = PixelDataFactory.Create(header, 0);

                // Build the correct file for this data depending on what is stored in the dicom files
                if (pixelData is GrayscalePixelDataU16)
                {
                    data.AddRange(((GrayscalePixelDataU16)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataS16)
                {
                    data.AddRange(((GrayscalePixelDataS16)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataU8)
                {
                    data.AddRange(((GrayscalePixelDataU8)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataS32)
                {
                    data.AddRange(((GrayscalePixelDataS32)pixelData).Data);
                }
                else if (pixelData is GrayscalePixelDataU32)
                {
                    data.AddRange(((GrayscalePixelDataU32)pixelData).Data);
                }
                else if (pixelData is ColorPixelData24)
                {
                    // note that this one will take some work
                    // 0 == red, 1 == Green, 2 == Blue and so on
                    data.AddRange(((ColorPixelData24)pixelData).Data);
                }
                else if (pixelData is SingleBitPixelData)
                {
                    data.AddRange(((SingleBitPixelData)pixelData).Data);
                }

                int tempDicomRows = -1;
                // get the amount of rows the image data set has out of the dicom file.
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.Rows, out tempDicomRows))
                {
                    if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.NumberOfVerticalPixels, out tempDicomRows))
                    {
                        if (dicomFile.height != tempDicomRows)
                        {
                            throw new NonConsistantDicomDirectoryException("The number of rows at index" + index);
                        }
                    }
                }
                else if (dicomFile.width != tempDicomRows)
                {
                    throw new NonConsistantDicomDirectoryException("The number of rows at index" + index);
                }

                // get the amount of coloumns in the image and ensure it stays the same
                int tempDicomCols = -1;
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.Columns, out tempDicomCols))
                {
                    // try getting this data  another way then
                    if (dicoms[index].Dataset.TryGetSingleValue(DicomTag.NumberOfHorizontalPixels, out tempDicomCols))
                    {
                        if (dicomFile.height != tempDicomCols)
                        {
                            throw new NonConsistantDicomDirectoryException("The number of columns at index" + index);
                        }
                    }
                }
                else if(dicomFile.height != tempDicomCols)
                {
                    throw new NonConsistantDicomDirectoryException("The number of columns at index" + index);
                }


                // TODO Maybe change how this works going forward
                aspectOfPixel = new float[2];
                // get the amount of coloumns in the image
                if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PixelSpacing, out aspectOfPixel))
                {
                    // all these methods will try to do it until one works or this feild will be made invalid
                    if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.ImagerPixelSpacing, out aspectOfPixel))
                    {
                        if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.NominalScannedPixelSpacing, out aspectOfPixel))
                        {
                            if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.CompensatorPixelSpacing, out aspectOfPixel))
                            {
                                if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.DetectorElementSpacing, out aspectOfPixel))
                                {
                                    if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PresentationPixelSpacing, out aspectOfPixel))
                                    {
                                        if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.PrinterPixelSpacing, out aspectOfPixel))
                                        {
                                            if (!dicoms[index].Dataset.TryGetValues<float>(DicomTag.ObjectPixelSpacingInCenterOfBeam, out aspectOfPixel))
                                            {
                                                // if none of the above methods work mark the objects as invalid
                                                aspectOfPixel[0] = -1;
                                                aspectOfPixel[1] = -1;
                                                // This could be valid in a case were only one row of data was colected
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // check input data against the old data and throw an exception if the file isn't exact
                if (dicomFile.pixelSpacingX != aspectOfPixel[0] || dicomFile.pixelSpacingY != aspectOfPixel[1])
                {
                    throw new NonConsistantDicomDirectoryException("Pixel Spacing Dicom Tag at index " + index);
                }

                // check the slice thickness is constisant
                if (dicoms[index].Dataset.TryGetSingleValue(DicomTag.SliceThickness, out float tempSliceThickness))
                {
                    if (dicomFile.sliceThickness == -1)
                    {
                        dicomFile.sliceThickness = tempSliceThickness;
                    }
                    else if (tempSliceThickness != dicomFile.sliceThickness)
                        throw new NonConsistantDicomDirectoryException("Slice Thickness at index" + index);
                }

                // try to get how much room is between each slice
                if (!dicoms[index].Dataset.TryGetSingleValue(DicomTag.SpacingBetweenSlices, out float tempSliceSpacing))
                {
                    if (dicomFile.spacingBetweenSlices == -1)
                    {
                        dicomFile.spacingBetweenSlices = tempSliceSpacing;
                    }
                    else if (tempSliceThickness != dicomFile.sliceThickness)
                        throw new NonConsistantDicomDirectoryException("Spacing between slices at index" + index);
                }
            }

            dicomFile.SetPixelBuffer(data);

            // return the json strings here
            return Newtonsoft.Json.JsonConvert.SerializeObject(dicomFile);
        }
    }
}
