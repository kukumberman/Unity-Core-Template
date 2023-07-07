using System;
using System.Collections.Generic;

namespace Game.Utilities
{
    public static class ArrayUtils
    {
        public static void RemoveAt<T>(ref T[] values, int index)
        {
            if (index == values.Length - 1)
            {
                Array.Resize(ref values, index);
                return;
            }

            T[] result = new T[values.Length - 1];
            if (index != 0)
            {
                Array.Copy(values, 0, result, 0, index);
            }
            Array.Copy(values, index + 1, result, index, values.Length - index - 1);
            values = result;
        }

        public static void Remove<T>(ref T[] values, T value)
        {
            RemoveAt(ref values, Array.IndexOf(values, value));
        }

        public static void Add<T>(ref T[] values, T value)
        {
            Array.Resize(ref values, values.Length + 1);
            values[values.Length - 1] = value;
        }

        public static void Insert<T>(ref T[] values, T value, int index)
        {
            Array.Resize(ref values, values.Length + 1);

            for (int i = index - 1; i >= 0; i--)
            {
                values[i + 1] = values[i];
            }

            values[index] = value;
        }

        public static void AddRange<T>(ref T[] array, int count, IEnumerable<T> values)
        {
            int startCount = array.Length;
            Array.Resize(ref array, array.Length + count);

            foreach (var value in values)
            {
                array[startCount++] = value;
            }
        }
    }
}