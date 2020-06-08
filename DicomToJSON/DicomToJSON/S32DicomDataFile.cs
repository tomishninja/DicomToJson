using System;
using System.Collections;

namespace DicomToJSON
{
    [Serializable]
    class S32DicomDataFile : DicomFileData
    {
        public int[] pixelBuffer;

        public S32DicomDataFile(int[] pixelBuffer)
        {
            this.pixelBuffer = pixelBuffer;
        }

        public S32DicomDataFile()
        {
        }

        public override int Length()
        {
            return pixelBuffer.Length;
        }

        public int[] GetData()
        {
            return pixelBuffer;
        }

        public override long[] GetDataAslongs()
        {
            long[] output = new long[pixelBuffer.Length];

            for (int index = 0; index < pixelBuffer.Length; index++)
            {
                output[index] = pixelBuffer[index];
            }

            return output;
        }

        public override string GetJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }

        public override void SetPixelBuffer(ArrayList arraylist)
        {
            pixelBuffer = new int[arraylist.Count];
            for (int index = 0; index < pixelBuffer.Length; index++)
            {
                pixelBuffer[index] = (int)arraylist[index];
            }
        }
    }
}
