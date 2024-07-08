using Parcel.Neo.Base.DataTypes;
using System;
using System.Linq;
using System.Text;

namespace Parcel.Neo.Base.Serialization
{
    public static class SerializationHelper
    {
        #region Serialization
        public static byte[] Serialize(CacheDataType value)
        {
            return BitConverter.GetBytes((int)value);
        }
        public static byte[] Serialize(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
        public static byte[] Serialize(bool value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] Serialize(int value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] Serialize(double value)
        {
            return BitConverter.GetBytes(value);
        }
        public static byte[] Serialize(Vector2D value)
        {
            byte[] x = BitConverter.GetBytes(value.X);
            byte[] y = BitConverter.GetBytes(value.Y);
            byte[] result = new byte[x.Length + y.Length];
            Buffer.BlockCopy(x, 0, result, 0, x.Length);
            Buffer.BlockCopy(y, 0, result, x.Length, y.Length);

            return result;
        }

        public static byte[] Serialize(CacheDataType[] value)
        {
            byte[] buffer = new byte[sizeof(int) + value.Length * sizeof(int)];
            Buffer.BlockCopy(BitConverter.GetBytes(value.Length), 0, buffer, 0, sizeof(int));
            for (int i = 0; i < value.Length; i++)
                Buffer.BlockCopy(BitConverter.GetBytes((int)value[i]), 0, buffer, sizeof(int) + sizeof(int) * i, sizeof(int));
            return buffer;
        }
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
        #endregion

        #region Deserialization
        public static CacheDataType GetCacheDataType(byte[] bytes)
        {
            return (CacheDataType)BitConverter.ToInt32(bytes);
        }
        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
        public static bool GetBool(byte[] bytes)
        {
            return BitConverter.ToBoolean(bytes);
        }
        public static int GetInt(byte[] bytes)
        {
            return BitConverter.ToInt32(bytes);
        }
        public static double GetDouble(byte[] bytes)
        {
            return BitConverter.ToDouble(bytes);
        }
        public static Vector2D GetVector2D(byte[] bytes)
        {
            return new Vector2D(BitConverter.ToDouble(bytes, 0), BitConverter.ToDouble(bytes, bytes.Length / 2));
        }

        public static CacheDataType[] GetCacheDataTypes(byte[] bytes)
        {
            int count = BitConverter.ToInt32(bytes, 0);
            CacheDataType[] types = new CacheDataType[count];
            for (int i = 0; i < count; i++)
                types[i] = (CacheDataType)BitConverter.ToInt32(bytes, sizeof(int) + sizeof(int) * i);
            return types;
        }
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
        #endregion
    }
}
