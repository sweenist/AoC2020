using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;

namespace AdventOfCode.Utils
{
    public static class BinaryExtensions
    {
        public static IEnumerable<bool> ToBinaryCollection(this string source, char highValue)
        {
            return source.Select(c => c.Equals(highValue));
        }

        public static int ToInt32(this IEnumerable<bool> source)
        {
            var result = 0;
            var queue = new Queue<bool>(source);
            while (queue.Count() > 0)
            {
                if (queue.Dequeue())
                    result += (int)Math.Pow((double)2, (double)queue.Count()); //barf
            }
            return result;
        }

        public static string ToBinary(this long x, int bitDepth)
        {
            var maxIndex = bitDepth - 1;
            char[] buffer = new char[bitDepth];

            for (var i = maxIndex; i >= 0; i--)
            {
                var mask = 1L << i;
                buffer[maxIndex - i] = (x & mask) != 0 ? '1' : '0';
            }

            return new string(buffer);
        }

        public static long FromBinary(this string value)
        {
            var returnValue = 0L;
            var index = 0;
            foreach (var bit in value.Reverse())
            {
                if (bit == '1')
                    returnValue += (long)Math.Pow(2, index);
                index++;
            }
            return returnValue;
        }
    }

    public class BinaryExtensionTests
    {
        public BinaryExtensionTests()
        {
            ShouldConvertStringToBinary();
            ShouldConvertBinaryCollectionToInt32();
        }
        public void ShouldConvertStringToBinary()
        {
            var testString = "AABBABAB";
            testString.ToBinaryCollection('B').Should().ContainInOrder(new[] { false, false, true, true, false, true, false, true });
        }

        public void ShouldConvertBinaryCollectionToInt32()
        {
            var input = new[] { false, false, true, true, false, true, false, true };
            input.ToInt32().Should().Be(53);
        }
    }
}