using System;
using System.Collections;

namespace DicomToJSON
{
    [Serializable]
    class S16DicomFileData : DicomFileData
    {
        public short[] pixelBuffer;

        public S16DicomFileData(short[] pixelBuffer)
        {
            this.pixelBuffer = pixelBuffer;
        }

        public S16DicomFileData()
        {
        }

        public override int Length()
        {
            return pixelBuffer.Length;
        }

        public short[] GetData()
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
            pixelBuffer = new short[arraylist.Count];
            for (int index = 0; index < pixelBuffer.Length; index++)
            {
                pixelBuffer[index] = (short)arraylist[index];
            }
        }
    }
}
