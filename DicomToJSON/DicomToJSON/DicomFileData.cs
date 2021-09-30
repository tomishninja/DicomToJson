using System;
using System.Collections;

namespace DicomToJSON
{
    /// <summary>
    /// Abstract class for Dicom data gathered from the various files
    /// Allows for information to be input and retrived from this file
    /// Designed for rendering the image
    /// </summary>
    [Serializable]
    abstract class DicomFileData
    {
        /// <summary>
        /// The amount of pixels in a row
        /// </summary>
        public int width;

        /// <summary>
        /// The amount of pixels in a collumn
        /// </summary>
        public int height;

        /// <summary>
        /// the breath of the grid in voxels
        /// </summary>
        public int breath;

        /// <summary>
        /// The space between each row in mm
        /// </summary>
        public float pixelSpacingX;

        /// <summary>
        /// The space between each column in mm
        /// </summary>
        public float pixelSpacingY;

        /// <summary>
        /// The thickness of each slice in mm
        /// </summary>
        public float sliceThickness;

        /// <summary>
        /// the distance between this slice and the next one in mm
        /// </summary>
        public float spacingBetweenSlices;

        /// <summary>
        /// how many pixels are in the data
        /// </summary>
        abstract public int Length();

        /// <summary>
        /// A generic identifier that all the dicoms should have in common
        /// </summary>
        public string frameOfReferenceId;

        /// <summary>
        /// Get the data out of the object as a series of long ints 
        /// regardless of the underlying type of data
        /// </summary>
        /// <returns>
        /// A array of long values that each hold the values for each
        /// pixel in the image
        /// </returns>
        abstract public long[] GetDataAslongs();

        /// <summary>
        /// Take in an arraylist and save it as the pixel buffer
        /// of the concreate object
        /// </summary>
        /// <param name="arraylist">
        /// A array list containg all of the number values to this array list
        /// </param>
        abstract public void SetPixelBuffer(ArrayList arraylist);

        /// <summary>
        /// Returns a JSON version of object
        /// </summary>
        /// <returns>
        /// a JSON version of object as a string
        /// </returns>
        public abstract string GetJSON();
    }
}
