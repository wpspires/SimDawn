using System;
using System.IO;
using System.Text;

namespace SimDawn
{
    public class GDFileReader
    {
        private uint _key;
        private uint[] _table = new uint[256];
        public BinaryReader ByteReader { get; set; }

        public void ReadKey()
        {
            uint k = ByteReader.ReadUInt32();

            k ^= 0x55555555;

            _key = k;

            for (uint i = 0; i < 256; i++)
            {
                k = (k >> 1) | (k << 31);
                k *= 39916801;
                _table[i] = k;
            }
        }

        public void UpdateKey(byte[] b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                _key ^= _table[b[i]];
            }
        }

        public byte ReadByte()
        {
            var val = ByteReader.ReadByte();
            byte output = (byte)(val ^ _key);
            UpdateKey(new byte[]{ val });
            return output;
        }

        public uint ReadInt()
        {
            var val = ByteReader.ReadUInt32();
            uint output = val ^ _key;
            UpdateKey(BitConverter.GetBytes(val));
            return output;
        }

        public uint NextInt()
        {
            var val = ByteReader.ReadUInt32();
            val ^= _key;
            return val;
        }

        public float ReadFloat()
        {
            var val = ReadInt();
            byte[] bytes = BitConverter.GetBytes(val);
            return  BitConverter.ToSingle(bytes, 0);
        }

        public string ReadString()
        {
            uint length = ReadInt();
            if (length == 0) throw new InvalidDataException("ReadString - Failed reading length of string");
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = ReadByte();
            }
            return Encoding.UTF8.GetString(bytes);
        }

        public string ReadWideString()
        {
            uint length = ReadInt();
            if (length == 0) throw new InvalidDataException("ReadString - Failed reading length of string");
            StringBuilder builder = new StringBuilder((int)length);
            for (int i = 0; i < length * 2; i += 2)
            {
                var b1 = ReadByte();
                short s = ReadByte();
                s = (short)(s << 8);

                char c = (char)(b1 | s);

                builder.Append(c);
            }
            return builder.ToString();
        }

        public uint ReadBlockStart(ref ByteBlock block)
        {
            var output = ReadInt();
            block.length = NextInt();
            block.end = (uint)ByteReader.BaseStream.Position + block.length;
            return output;
        }

        public void ReadBlockEnd(ref ByteBlock block)
        {
            if ((uint)ByteReader.BaseStream.Position != block.end) throw new IOException("Mismatch on block end");

            if (NextInt() != 0) throw new IOException("Invalid block end");
        }
    }
}