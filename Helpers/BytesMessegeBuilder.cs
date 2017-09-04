namespace NowMine.Helpers
{
    class BytesMessegeBuilder
    {
        public static byte[] MergeBytesArray(byte[] firstArray, byte[] secondArray)
        {
            byte[] result = new byte[firstArray.Length + secondArray.Length];
            System.Buffer.BlockCopy(firstArray, 0, result, 0, firstArray.Length);
            System.Buffer.BlockCopy(secondArray, 0, result, firstArray.Length, secondArray.Length);
            return result;
        }
    }
}
