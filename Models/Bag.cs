namespace SimDawn
{
    public class Bag
    {
        public uint Width;
        public uint Height;
        public int Index { get; set; }

        internal Read(GDFileReader reader, Item[] items)
        {
            ByteBlock block = new ByteBlock();
            reader.ReadBlockStart(ref block);
            reader.ReadByte();
            foreach (var item in items) item.Read(reader);
            reader.ReadBlockEnd(ref block);
        }
    }
}