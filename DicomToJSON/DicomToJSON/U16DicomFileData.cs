using System;
using System.Collections;

namespace DicomToJSON
{
    [Serializable]
    class U16DicomFileData : DicomFileData
    {
        public ushort[] pixelBuffer;

        public U16DicomFileData(ushort[] pixelBuffer)
        {
            this.pixelBuffer = pixelBuffer;
        }

        public U16DicomFileData()
        {
        }

        public override int Length()
        {
            return pixelBuffer.Length;
        }

        public ushort[] GetData()
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

        public override void SetPixelBuffer(ArrayList arraylist)
        {
            pixelBuffer = new ushort[arraylist.Count];
            for(int index = 0; index < pixelBuffer.Length; index++)
            {
                pixelBuffer[index] = (ushort)arraylist[index];
            }
        }

        public override string GetJSON()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
