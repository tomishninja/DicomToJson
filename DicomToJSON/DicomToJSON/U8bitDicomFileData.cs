using System;
using System.Collections;

namespace DicomToJSON
{
    [Serializable]
    class U8bitDicomFileData : DicomFileData
    {
        public byte[] pixelBuffer;

        public U8bitDicomFileData(byte[] pixelBuffer)
        {
            this.pixelBuffer = pixelBuffer;
        }

        public U8bitDicomFileData()
        {
        }

        public override int Length()
        {
            return pixelBuffer.Length;
        }

        public byte[] GetData()
        {
            return pixelBuffer;
        }

        public override long[] GetDataAslongs()
        {
            long[] output = new long[pixelBuffer.Length];

            for(int index = 0; index < pixelBuffer.Length; index++)
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
            pixelBuffer = new byte[arraylist.Count];
            for (int index = 0; index < pixelBuffer.Length; index++)
            {
                pixelBuffer[index] = (byte)arraylist[index];
            }
        }
    }
}
