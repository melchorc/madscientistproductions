using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace MadScience
{
    public class StreamHelpers
    {

		#region Stream functions
		public static bool isValidStream(Stream input)
		{
			if (input == null) return false;
			if (input == Stream.Null) return false;
			if (input.Length == 0) return false;

			return true;
		}

		public static void CopyStream(Stream readStream, Stream writeStream)
		{
			CopyStream(readStream, writeStream, false);
		}
		public static void CopyStream(Stream readStream, Stream writeStream, bool fromStart)
		{

			if (fromStart)
			{
				readStream.Seek(0, SeekOrigin.Begin);
			}

			int Length = 256;
			Byte[] buffer = new Byte[Length];
			int bytesRead = readStream.Read(buffer, 0, Length);
			// write the required bytes
			while (bytesRead > 0)
			{
				writeStream.Write(buffer, 0, bytesRead);
				bytesRead = readStream.Read(buffer, 0, Length);
			}
		}
		#endregion

        public static Single ReadValueF32(Stream stream)
        {
            return ReadValueF32(stream, true);
        }

        public static Single ReadValueF32(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 4);

            if (ShouldSwap(littleEndian))
            {
                return BitConverter.ToSingle(BitConverter.GetBytes(MadScience.NumberHelpers.Swap(BitConverter.ToInt32(data, 0))), 0);
            }
            else
            {
                return BitConverter.ToSingle(data, 0);
            }
        }

        public static void WriteValueF32(Stream stream, Single value)
        {
            WriteValueF32(stream, value, true);
        }

        public static void WriteValueF32(Stream stream, Single value, bool littleEndian)
        {
            byte[] data;
            if (ShouldSwap(littleEndian))
            {
                data = BitConverter.GetBytes(MadScience.NumberHelpers.Swap(BitConverter.ToInt32(BitConverter.GetBytes(value), 0)));
            }
            else
            {
                data = BitConverter.GetBytes(value);
            }
            stream.Write(data, 0, 4);
        }


        public static UInt64 ReadValueU64(Stream stream)
        {
            return ReadValueU64(stream, true);
        }

        public static UInt64 ReadValueU64(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[8];
            int read = stream.Read(data, 0, 8);
            //Debug.Assert(read == 8);
            UInt64 value = BitConverter.ToUInt64(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueU64(Stream stream, UInt64 value)
        {
            WriteValueU64(stream, value, true);
        }

        public static void WriteValueU64(Stream stream, UInt64 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 8);
            stream.Write(data, 0, 8);
        }


        public static Int16 ReadValueS16(Stream stream)
        {
            return ReadValueS16(stream, true);
        }

        public static Int16 ReadValueS16(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[2];
            int read = stream.Read(data, 0, 2);
            //Debug.Assert(read == 2);
            Int16 value = BitConverter.ToInt16(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueS16(Stream stream, Int16 value)
        {
            WriteValueS16(stream, value, true);
        }

        public static void WriteValueS16(Stream stream, Int16 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 2);
            stream.Write(data, 0, 2);
        }

        public static Int64 ReadValueS64(Stream stream)
        {
            return ReadValueS64(stream, true);
        }

        public static Int64 ReadValueS64(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[8];
            int read = stream.Read(data, 0, 8);
            //Debug.Assert(read == 8);
            Int64 value = BitConverter.ToInt64(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueS64(Stream stream, Int64 value)
        {
            WriteValueS64(stream, value, true);
        }

        public static void WriteValueS64(Stream stream, Int64 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 8);
            stream.Write(data, 0, 8);
        }

        public static Int32 ReadValueS32(Stream stream)
        {
            return ReadValueS32(stream, true);
        }

        public static Int32 ReadValueS32(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 4);
            //Debug.Assert(read == 4);
            Int32 value = BitConverter.ToInt32(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueS32(Stream stream, Int32 value)
        {
            WriteValueS32(stream, value, true);
        }

        public static void WriteValueS32(Stream stream, Int32 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 4);
            stream.Write(data, 0, 4);
        }


        public static T ReadStructure<T>(Stream stream)
        {
            GCHandle handle;
            int structureSize;
            byte[] buffer;

            structureSize = Marshal.SizeOf(typeof(T));
            buffer = new byte[structureSize];

            if (stream.Read(buffer, 0, structureSize) != structureSize)
            {
                throw new InvalidOperationException("could not read all of data for structure");
            }

            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            handle.Free();

            return structure;
        }

        public static T ReadStructure<T>(Stream stream, int size)
        {
            GCHandle handle;
            int structureSize;
            byte[] buffer;

            structureSize = Marshal.SizeOf(typeof(T));

            if (size > structureSize)
            {
                throw new InvalidOperationException("read size cannot be greater than structure size");
            }

            buffer = new byte[structureSize];

            if (stream.Read(buffer, 0, size) != size)
            {
                throw new InvalidOperationException("could not read all of data for structure");
            }

            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            T structure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));

            handle.Free();

            return structure;
        }

        public static void WriteStructure<T>(Stream stream, T structure)
        {
            GCHandle handle;
            int size;
            byte[] buffer;

            size = Marshal.SizeOf(typeof(T));
            buffer = new byte[size];
            handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);

            Marshal.StructureToPtr(structure, handle.AddrOfPinnedObject(), false);

            handle.Free();

            stream.Write(buffer, 0, buffer.Length);
        }


        public static string ReadStringUTF16(Stream stream, uint size)
        {
            return ReadStringInternalStatic(stream, Encoding.Unicode, size, false);
        }

        public static string ReadStringUTF16(Stream stream, uint size, bool trailingNull)
        {
            return ReadStringInternalStatic(stream, Encoding.Unicode, size, trailingNull);
        }

        public static string ReadStringUTF16(Stream stream, bool littleEndian, uint size)
        {
            return ReadStringInternalStatic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, size, false);
        }

        public static string ReadStringUTF16(Stream stream, bool littleEndian, uint size, bool trailingNull)
        {
            return ReadStringInternalStatic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, size, trailingNull);
        }

        public static string ReadStringUTF16Z(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.Unicode, '\0');
        }

        public static string ReadStringUTF16Z(Stream stream, bool littleEndian)
        {
            return ReadStringInternalDynamic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, '\0');
        }

        public static string ReadStringUTF16NL(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.Unicode, '\n');
        }

        public static string ReadStringUTF16NL(Stream stream, bool littleEndian)
        {
            return ReadStringInternalDynamic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, '\n');
        }

        public static void WriteStringUTF16(Stream stream, string value)
        {
            WriteStringInternalStatic(stream, Encoding.Unicode, value);
        }

        public static void WriteStringUTF16(Stream stream, bool littleEndian, string value)
        {
            WriteStringInternalStatic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value);
        }

        public static void WriteStringUTF16Z(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.Unicode, value, '\0');
        }

        public static void WriteStringUTF16Z(Stream stream, bool littleEndian, string value)
        {
            WriteStringInternalDynamic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value, '\0');
        }

        public static void WriteStringUTF16NL(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.Unicode, value, '\n');
        }

        public static void WriteStringUTF16NL(Stream stream, bool littleEndian, string value)
        {
            WriteStringInternalDynamic(stream, littleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, value, '\n');
        }

        public static string ReadStringASCII(Stream stream, uint size, bool trailingNull)
        {
            return ReadStringInternalStatic(stream, Encoding.ASCII, size, trailingNull);
        }

        public static string ReadStringASCII(Stream stream, uint size)
        {
            return ReadStringInternalStatic(stream, Encoding.ASCII, size, false);
        }

        public static string ReadStringASCIIZ(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.ASCII, '\0');
        }

        public static string ReadStringASCIINL(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.ASCII, '\n');
        }

        public static void WriteStringASCII(Stream stream, string value)
        {
            WriteStringInternalStatic(stream, Encoding.ASCII, value);
        }

        public static void WriteStringASCIIZ(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.ASCII, value, '\0');
        }

        public static void WriteStringASCIINL(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.ASCII, value, '\n');
        }

        public static string ReadStringUTF8(Stream stream, uint size, bool trailingNull)
        {
            return ReadStringInternalStatic(stream, Encoding.UTF8, size, trailingNull);
        }

        public static string ReadStringUTF8(Stream stream, uint size)
        {
            return ReadStringInternalStatic(stream, Encoding.UTF8, size, false);
        }

        public static string ReadStringUTF8Z(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.UTF8, '\0');
        }

        public static string ReadStringUTF8NL(Stream stream)
        {
            return ReadStringInternalDynamic(stream, Encoding.UTF8, '\n');
        }

        public static void WriteStringUTF8(Stream stream, string value)
        {
            WriteStringInternalStatic(stream, Encoding.UTF8, value);
        }

        public static void WriteStringUTF8Z(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.UTF8, value, '\0');
        }

        public static void WriteStringUTF8NL(Stream stream, string value)
        {
            WriteStringInternalDynamic(stream, Encoding.UTF8, value, '\n');
        }

        internal static string ReadStringInternalStatic(Stream stream, Encoding encoding, uint size, bool trailingNull)
        {
            byte[] data = new byte[size];
            stream.Read(data, 0, data.Length);

            string value = encoding.GetString(data, 0, data.Length);

            if (trailingNull)
            {
                value = value.TrimEnd('\0');
            }

            return value;
        }

        internal static void WriteStringInternalStatic(Stream stream, Encoding encoding, string value)
        {
            byte[] data = encoding.GetBytes(value);
            stream.Write(data, 0, data.Length);
        }

        internal static string ReadStringInternalDynamic(Stream stream, Encoding encoding, char end)
        {
            int characterSize = encoding.GetByteCount("e");
            //Debug.Assert(characterSize == 1 || characterSize == 2 || characterSize == 4);
            string characterEnd = end.ToString();

            int i = 0;
            byte[] data = new byte[128 * characterSize];

            while (true)
            {
                if (i + characterSize > data.Length)
                {
                    Array.Resize(ref data, data.Length + (128 * characterSize));
                }

                int read = stream.Read(data, i, characterSize);
                //Debug.Assert(read == characterSize);

                if (encoding.GetString(data, i, characterSize) == characterEnd)
                {
                    break;
                }

                i += characterSize;
            }

            if (i == 0)
            {
                return "";
            }

            return encoding.GetString(data, 0, i);
        }

        internal static void WriteStringInternalDynamic(Stream stream, Encoding encoding, string value, char end)
        {
            byte[] data;

            data = encoding.GetBytes(value);
            stream.Write(data, 0, data.Length);

            data = encoding.GetBytes(end.ToString());
            stream.Write(data, 0, data.Length);
        }


        public static UInt16 ReadValueU16(Stream stream)
        {
            return ReadValueU16(stream, true);
        }

        public static UInt16 ReadValueU16(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[2];
            int read = stream.Read(data, 0, 2);
            //Debug.Assert(read == 2);
            UInt16 value = BitConverter.ToUInt16(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueU16(Stream stream, UInt16 value)
        {
            WriteValueU16(stream, value, true);
        }

        public static void WriteValueU16(Stream stream, UInt16 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 2);
            stream.Write(data, 0, 2);
        }

        public static UInt32 ReadValueU24(Stream stream)
        {
            return ReadValueU24(stream, true);
        }

        public static UInt32 ReadValueU24(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 3);
            //Debug.Assert(read == 3);
            UInt32 value = BitConverter.ToUInt32(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap24(value);
                //value = value.Swap24();
            }

            return value & 0xFFFFFF;
        }

        public static void WriteValueU24(Stream stream, UInt32 value)
        {
            WriteValueU24(stream, value, true);
        }

        public static void WriteValueU24(Stream stream, UInt32 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap24(value);
                //value = value.Swap24();
            }

            value &= 0xFFFFFF;

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 4);
            stream.Write(data, 0, 3);
        }

        public static UInt32 ReadValueU32(Stream stream)
        {
            return ReadValueU32(stream, true);
        }

        public static UInt32 ReadValueU32(Stream stream, bool littleEndian)
        {
            byte[] data = new byte[4];
            int read = stream.Read(data, 0, 4);
            //Debug.Assert(read == 4);
            UInt32 value = BitConverter.ToUInt32(data, 0);

            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            return value;
        }

        public static void WriteValueU32(Stream stream, UInt32 value)
        {
            WriteValueU32(stream, value, true);
        }

        public static void WriteValueU32(Stream stream, UInt32 value, bool littleEndian)
        {
            if (ShouldSwap(littleEndian))
            {
                value = MadScience.NumberHelpers.Swap(value);
                //value = value.Swap();
            }

            byte[] data = BitConverter.GetBytes(value);
            //Debug.Assert(data.Length == 4);
            stream.Write(data, 0, 4);
        }

        public static byte ReadValueU8(Stream stream)
        {
            return (byte)stream.ReadByte();
        }

        public static void WriteValueU8(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        internal static bool ShouldSwap(bool littleEndian)
        {
            if (littleEndian == true && BitConverter.IsLittleEndian == false)
            {
                return true;
            }
            else if (littleEndian == false && BitConverter.IsLittleEndian == true)
            {
                return true;
            }

            return false;
        }
    }

}
