using Parcel.Neo.Base.DataTypes;
using Parcel.Types;
using System;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Parcel.Neo.Base.Serialization
{
    public static class SerializationHelper
    {
        #region Serialization
        public static byte[] Serialize(string value)
            => Encoding.UTF8.GetBytes(value);
        public static byte[] Serialize(bool value)
            => BitConverter.GetBytes(value);
        public static byte[] Serialize(int value)
            => BitConverter.GetBytes(value);
        public static byte[] Serialize(double value)
            => BitConverter.GetBytes(value);
        public static byte[] Serialize(Vector2D value)
        {
            byte[] x = BitConverter.GetBytes(value.X);
            byte[] y = BitConverter.GetBytes(value.Y);
            byte[] result = new byte[x.Length + y.Length];
            Buffer.BlockCopy(x, 0, result, 0, x.Length);
            Buffer.BlockCopy(y, 0, result, x.Length, y.Length);

            return result;
        }
        public static byte[] Serialize(Vector2 value)
        {
            byte[] x = BitConverter.GetBytes(value.X);
            byte[] y = BitConverter.GetBytes(value.Y);
            byte[] result = new byte[x.Length + y.Length];
            Buffer.BlockCopy(x, 0, result, 0, x.Length);
            Buffer.BlockCopy(y, 0, result, x.Length, y.Length);

            return result;
        }
        public static byte[] Serialize(Size value)
        {
            byte[] width = BitConverter.GetBytes(value.Width);
            byte[] height = BitConverter.GetBytes(value.Height);
            byte[] result = new byte[width.Length + height.Length];
            Buffer.BlockCopy(width, 0, result, 0, width.Length);
            Buffer.BlockCopy(height, 0, result, width.Length, height.Length);

            return result;
        }
        public static byte[] Serialize(Types.Color value)
            => Serialize(value.ToString());
        public static byte[] Serialize(string[] value)
        {
            byte[][] buffers = value.Select(v => Encoding.UTF8.GetBytes(v)).ToArray();
            byte[] buffer = new byte[sizeof(int) + buffers.Length * sizeof(int) + buffers.Sum(b => b.Length)];
            Buffer.BlockCopy(BitConverter.GetBytes(value.Length), 0, buffer, 0, sizeof(int));
            int offset = sizeof(int);
            for (int i = 0; i < value.Length; i++)
            {
                Buffer.BlockCopy(buffers[i], 0, BitConverter.GetBytes(buffers[i].Length), offset, sizeof(int));
                Buffer.BlockCopy(buffers[i], 0, buffer, sizeof(int) + offset, buffers[i].Length);
                offset += sizeof(int) + buffers[i].Length;
            }
            return buffer;
        }
        public static byte[] Serialize(DateTime dateTime)
            => Serialize(dateTime.ToString());
        #endregion

        #region Deserialization
        public static string GetString(byte[] bytes)
            => Encoding.UTF8.GetString(bytes);
        public static bool GetBool(byte[] bytes)
            => BitConverter.ToBoolean(bytes);
        public static int GetInt(byte[] bytes)
            => BitConverter.ToInt32(bytes);
        public static double GetDouble(byte[] bytes)
            => BitConverter.ToDouble(bytes);
        public static Vector2D GetVector2D(byte[] bytes)
            => new(BitConverter.ToDouble(bytes, 0), BitConverter.ToDouble(bytes, bytes.Length / 2));
        public static Vector2 GetVector2(byte[] bytes)
            => new(BitConverter.ToSingle(bytes, 0), BitConverter.ToSingle(bytes, bytes.Length / 2));
        public static string[] GetStrings(byte[] bytes)
        {
            int count = BitConverter.ToInt32(bytes, 0);
            string[] strings = new string[count];
            int offset = sizeof(int);
            for (int i = 0; i < count; i++)
            {
                string value = Encoding.UTF8.GetString(bytes, sizeof(int) + offset, BitConverter.ToInt32(bytes, offset));
                strings[i] = value;
                offset += value.Length;
            }
            return strings;
        }
        public static DateTime GetDateTime(byte[] bytes)
            => DateTime.Parse(GetString(bytes));
        public static Size GetSize(byte[] bytes)
            => new(BitConverter.ToInt32(bytes, 0), BitConverter.ToInt32(bytes, bytes.Length / 2));
        public static Types.Color GetColor(byte[] bytes)
            => Types.Color.Parse(GetString(bytes));
        #endregion
    }
}
