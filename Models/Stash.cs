using System;
namespace SimDawn
{
    public class Stash
    {
        internal void Read(GDFileReader reader, Item[] items)
        {
            ByteBlock block = new ByteBlock();
            reader.ReadBlockStart(ref block);
            var version = reader.ReadInt();
            if (version < 5) throw new Exception("Bad Stash version #");

            var width = reader.ReadInt();
            var height = reader.ReadInt();
            foreach (var item in items) item.Read(reader);

            reader.ReadBlockEnd(ref block);
        }
    }
}