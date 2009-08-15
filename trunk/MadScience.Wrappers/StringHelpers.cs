using System;
using System.Collections.Generic;
using System.Text;

namespace MadScience
{
    public class StringHelpers
    {
        // FNV hash that EA loves to use :-)
        public static UInt32 HashFNV24(string input)
        {
            UInt32 hash = HashFNV32(input);
            return (hash >> 24) ^ (hash & 0xFFFFFF);
        }

        public static UInt32 HashFNV32(string input)
        {
            string lower = input.ToLowerInvariant();
            UInt32 hash = 0x811C9DC5;

            for (int i = 0; i < lower.Length; i++)
            {
                hash *= 0x1000193;
                hash ^= (char)(lower[i]);
            }

            return hash;
        }

        public static UInt64 HashFNV64(string input)
        {
            string lower = input.ToLowerInvariant();
            UInt64 hash = 0xCBF29CE484222325;

            for (int i = 0; i < lower.Length; i++)
            {
                hash *= 0x00000100000001B3;
                hash ^= (char)(lower[i]);
            }

            return hash;
        }

        public static UInt64 Hash1003F(string input)
        {
            UInt64 hash = 0;
            for (int i = 0; i < input.Length; i++)
            {
                hash = (hash * 65599) ^ (char)(input[i]);
            }
            return hash;
        }

        public static UInt32 ParseHex32(string input)
        {
            if (input.StartsWith("0x"))
            {
                return UInt32.Parse(input.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return UInt32.Parse(input);
        }

        public static UInt64 ParseHex64(string input)
        {
            if (input.StartsWith("0x"))
            {
                return UInt64.Parse(input.Substring(2), System.Globalization.NumberStyles.AllowHexSpecifier);
            }

            return UInt64.Parse(input);
        }
    }

}
