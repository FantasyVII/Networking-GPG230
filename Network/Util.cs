using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Network
{
    public static class Util
    {
        public static ConsoleColor ConvertStringToColor(string colorName)
        {
            switch (colorName)
            {
                case "Black":
                    return ConsoleColor.Black;
                case "DarkBlue":
                    return ConsoleColor.DarkBlue;
                case "DarkGreen":
                    return ConsoleColor.DarkGreen;
                case "DarkCyan":
                    return ConsoleColor.DarkCyan;
                case "DarkRed":
                    return ConsoleColor.DarkRed;
                case "DarkMagenta":
                    return ConsoleColor.DarkMagenta;
                case "DarkYellow":
                    return ConsoleColor.DarkYellow;
                case "Gray":
                    return ConsoleColor.Gray;
                case "DarkGray":
                    return ConsoleColor.DarkGray;
                case "Blue":
                    return ConsoleColor.Blue;
                case "Green":
                    return ConsoleColor.Green;
                case "Cyan":
                    return ConsoleColor.Cyan;
                case "Red":
                    return ConsoleColor.Red;
                case "Magenta":
                    return ConsoleColor.Magenta;
                case "Yellow":
                    return ConsoleColor.Yellow;
                case "White":
                    return ConsoleColor.White;
                default:
                    return ConsoleColor.White;
            }
        }

        public static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public static Object ByteArrayToObject(byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }
    }
}