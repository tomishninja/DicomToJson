using System;
using System.Collections;

namespace DicomToJSON
{
    [Serializable]
    class U32DicomDataFile : DicomFileData
    {
        public uint[] pixelBuffer;

        public U32DicomDataFile(uint[] pixelBuffer)
        {
            this.pixelBuffer = pixelBuffer;
        }

        public U32DicomDataFile()
        {
        }

        public override int Length()
        {
            return pixelBuffer.Length;
        }

        public uint[] GetData()
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
            pixelBuffer = new uint[arraylist.Count];
            for (int index = 0; index < pixelBuffer.Length; index++)
            {
                pixelBuffer[index] = (uint)arraylist[index];
            }
        }
    }
}
