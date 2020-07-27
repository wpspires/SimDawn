using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SimDawn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var charFilePath = @"C:\Users\Guy\Documents\Path of Building\player.gdc";

            Character charInfo = ReadFile(charFilePath);
        }

        private Character ReadFile(string path)
        {
            var reader = new GDFileReader();
            var theTaken = new Character();
            using (reader.ByteReader = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read)))
            {
                reader.ReadKey();

                if (reader.ReadInt() != 0x58434447) throw new IOException();
                var temp = reader.ReadInt();
                if (temp != 1 && temp != 2) throw new IOException();

                //header
                theTaken.Name = reader.ReadWideString();
                theTaken.isMale = reader.ReadByte() == 1;
                var tag = reader.ReadString();
                theTaken.Level = (int)reader.ReadInt();
                var hardcore = reader.ReadByte();

                //We're off by 1 byte somehow
                reader.ReadByte();

                Console.WriteLine(reader.NextInt());

                Console.WriteLine(reader.ReadInt());
                
                //Read ID
                byte[] id = new byte[16];
                for (int i = 0; i < 16; i++)
                {
                    id[i] = reader.ReadByte();
                }

                
                //Read character info
                ReadCharacterInfo(ref theTaken, ref reader);

                //Read bio
                ReadCharacterBio(ref theTaken, ref reader);

                //Read inventory
                ReadInventory(ref theTaken, ref reader);
                //Read stash

                //Read respawns

                //Read teleports

                //Read markers

                //Read shrines

                //Read skills

                return new Character();
            }
        
        
        }

        private void ReadCharacterInfo(ref Character theTaken, ref GDFileReader reader)
        {
            ByteBlock block = new ByteBlock();
            if (reader.ReadBlockStart(ref block) != 1) throw new Exception();
                var version = reader.ReadInt();
                if (version < 5) throw new Exception();
                reader.ReadByte();
                reader.ReadByte();
                var difficulty = reader.ReadByte();
                var greatestDifficulty = reader.ReadByte();
                var money = reader.ReadInt();
                if (version > 4)
                {
                    reader.ReadByte();
                    reader.ReadInt();
                }
                reader.ReadByte();
                reader.ReadInt();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadByte();
                reader.ReadString();
                reader.ReadBlockEnd(ref block);
                Console.WriteLine($"Position at end of char info: {reader.ByteReader.BaseStream.Position}");
        }

        private void ReadCharacterBio(ref Character theTaken, ref GDFileReader reader)
        {
            ByteBlock block = new ByteBlock();
            if (reader.ReadBlockStart(ref block) != 2) throw new Exception();
            var version = reader.ReadInt();
            if (version < 8) throw new Exception();

            theTaken.Level = (int)reader.ReadInt();
            reader.ReadInt();
            Console.WriteLine($"Modifier points: {reader.ReadInt()}");
            theTaken.SkillPoints = (int) reader.ReadInt();
            Console.WriteLine($"Devotion points: {reader.ReadInt()}");
            theTaken.DevotionPoints = (int) reader.ReadInt();
            theTaken.Physique = (int)reader.ReadFloat();
            theTaken.Cunning = (int)reader.ReadFloat();
            theTaken.Spirit = (int)reader.ReadFloat();
            theTaken.Health = (int)reader.ReadFloat();
            theTaken.Energy = (int)reader.ReadFloat();
            reader.ReadBlockEnd(ref block);
        }

        private void ReadInventory(ref Character theTaken, ref GDFileReader reader)
        {
            ByteBlock block = new ByteBlock();
            if (reader.ReadBlockStart(ref block) != 3) throw new Exception();
            var version = reader.ReadInt();
            if (version < 4) throw new IOException("Invalid version");

            byte flag = reader.ReadByte();
            if (flag == 1)
            {
                uint numBags = reader.ReadInt();
                reader.ReadInt(); //Focused
                reader.ReadInt(); //Selected

                List<Bag> bags = new List<Bag>((int)numBags);
                for (int i = 0; i < numBags; i++)
                {
                    Bag bag = new Bag();

                    bag.Index = i;
                }
            }
            reader.ReadBlockEnd(ref block);
        }
    }
}
