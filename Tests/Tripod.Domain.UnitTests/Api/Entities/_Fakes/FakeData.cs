using System;
using System.Linq;

namespace Tripod
{
    public static class FakeData
    {
        private static readonly Random Random = new Random(Guid.NewGuid().GetHashCode());

        public static string String()
        {
            return Guid.NewGuid().ToString();
        }

        public static string Email()
        {
            return string.Format("{0}@domain.tld", String());
        }

        public static int Id(params int[] canNotBe)
        {
            var id = Random.Next(1, int.MaxValue);
            while (canNotBe.Contains(id))
                id = Random.Next(1, int.MaxValue);
            return id;
        }

        public static T OneOf<T>(params T[] options)
        {
            var index = Random.Next(0, options.Length - 1);
            return options[index];
        }

        public static int Int(int minValue, int maxValue)
        {
            var integer = Random.Next(minValue, maxValue);
            return integer;
        }

        public static short Short(short minValue, short maxValue)
        {
            var shortInteger = Random.Next(minValue, maxValue);
            return (short)shortInteger;
        }

        public static byte Byte(byte minValue, byte maxValue)
        {
            var byteInteger = Random.Next(minValue, maxValue);
            return (byte)byteInteger;
        }

        public static double Double(int minValue, int maxValue)
        {
            int integer = Random.Next(minValue, maxValue - 1);
            double numerator = Random.Next(1, 99);
            double denominator = Random.Next(100, int.MaxValue);
            double remainder = numerator / denominator;
            return integer + remainder;
        }

        public static float Float(int minValue, int maxValue)
        {
            int integer = Random.Next(minValue, maxValue - 1);
            float numerator = Random.Next(1, 99);
            float denominator = Random.Next(100, int.MaxValue);
            float remainder = numerator / denominator;
            return integer + remainder;
        }

        public static decimal Decimal(int minValue, int maxValue)
        {
            int integer = Random.Next(minValue, maxValue - 1);
            decimal numerator = Random.Next(1, 99);
            decimal denominator = Random.Next(100, int.MaxValue);
            decimal remainder = numerator / denominator;
            return integer + remainder;
        }
    }
}
