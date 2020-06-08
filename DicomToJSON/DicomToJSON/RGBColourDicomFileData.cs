using System;

namespace DicomToJSON
{
    [Serializable]
    class RGBColourDicomFileData : U8bitDicomFileData
    {
        public RGBColourDicomFileData()
        {
        }

        public RGBColourDicomFileData(byte[] pixelBuffer) : base(pixelBuffer) {  }

        public override long[] GetDataAslongs()
        {
            long[] output = new long[pixelBuffer.Length / 3];
            byte[] temp = new byte[8];

            // Set the unused values to zero
            temp[3] = 0;
            temp[4] = 0;
            temp[5] = 0;
            temp[6] = 0;
            temp[7] = 0;

            for (int index = 0, cb = 0; index < pixelBuffer.Length; index++)
            {
                temp[0] = pixelBuffer[cb++];
                temp[1] = pixelBuffer[cb++];
                temp[2] = pixelBuffer[cb++];

                output[index] = BitConverter.ToInt64(temp, 0);
            }

            return output;
        }
    }
}
