using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ddd_column.Framework
{
    /// <summary>
    /// For when you need a value, but it doesn't matter what it is.
    /// </summary>
    public static class Some
    {
        public static Guid Guid()
        {
            return System.Guid.NewGuid();
        }

        public static string String()
        {
            return String(16);
        }

        public static string AlphanumericString()
        {
            return AlphanumericString(16);
        }

        private const string AlphanumericCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        public static string AlphanumericString(int length)
        {
            return String(length, AlphanumericCharacters);
        }

        private const string StringCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 -.,/";
        public static string String(int length, string characters = StringCharacters)
        {
            int size = length;
            StringBuilder sb = new StringBuilder();
            while (size > 0)
            {
                int value = Random.Next(characters.Length - 1);
                sb.Append(characters[value]);
                size--;
            }
            return sb.ToString();
        }


        public static int RandomInteger(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static int Integer()
        {
            return Random.Next(int.MaxValue);
        }

        public static int SmallInteger()
        {
            return Random.Next(10000);
        }

        public static uint UnsignedInteger()
        {
            return (uint)Random.Next(int.MaxValue);
        }

        public static byte Byte()
        {
            return (byte)(Random.Next(byte.MaxValue));
        }

        public static byte[] ByteArray()
        {
            return Enumerable.Range(0, 12).Select(i => Byte()).ToArray();
        }

        public static T Enum<T>()
            where T : struct
        {
            Type type = typeof(T);
            if (!type.IsEnum)
                return default(T);

            Array values = type.GetEnumValues();

            return (T)values.GetValue(Random.Next(values.Length));
        }

        public static T ElementIn<T>(IReadOnlyList<T> collection)
        {
            return collection[RandomInteger(collection.Count - 1)];
        }

        public static DateTime Date()
        {
            return new DateTime(Integer() % 100 + 2000, Integer() % 12 + 1, Integer() % 28 + 1);
        }

        private static Random _random;
        private static Random Random
        {
            get { return _random ?? (_random = new Random()); }
        }
    }
}
