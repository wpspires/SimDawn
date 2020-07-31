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
            var charFilePath2 = @"C:\Program Files (x86)\Steam\userdata\8042019\219990\remote\save\main\_Ohld\player.gdc";

            File.WriteAllText(@"C:\Users\Guy\Documents\Path of Building\SimDawn.txt", "Writing gdc file \r\n"); //Clear out the file
            using var _logger = new StreamWriter(@"C:\Users\Guy\Documents\Path of Building\SimDawn.txt", true);
            Character charInfo = ReadFile(charFilePath, _logger);
        }

        private Character ReadFile(string path, StreamWriter _logger)
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
                _logger.WriteLine($"Name: {theTaken.Name}");
                theTaken.isMale = reader.ReadByte() == 1;
                _logger.WriteLine($"Sex: {(theTaken.isMale ? "Male" : "Female")}");
                var tag = reader.ReadString();
                _logger.WriteLine($"Tag: {tag}");
                theTaken.Level = (int)reader.ReadInt();
                _logger.WriteLine($"Level: {theTaken.Level}");
                var hardcore = reader.ReadByte();
                _logger.WriteLine($"Hardcore: {hardcore}");

                //We're off by 1 byte somehow
                reader.ReadByte();

                _logger.WriteLine($"{reader.NextInt()}");
                _logger.WriteLine($"Header version?: {reader.ReadInt()}"); //version?
                
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


                return theTaken;
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
            Item[] equipment = new Item[12]; //Arrays.InitializeWithDefaultInstances<Item>(12);
            Item[] weaponSet1 = new Item[2];
            Item[] weaponSet2 = new Item[2];
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
                    bag.Read(reader);
                    bag.Index = i;
                    bags.Add(bag);
                }

                var useAlternate = reader.ReadByte();

                for (int x = 0; x < 12; x++)
                {
                    equipment[x].Read(reader);
                    reader.ReadByte();
                }

                var alternate1 = reader.ReadByte();

                for (int y = 0; y < 2; y++)
                {
                    weaponSet1[y].Read(reader);
                    reader.ReadByte();
                }

                var alternate2 = reader.ReadByte();

                for (int z = 0; z < 2; z++)
                {
                    weaponSet2[z].Read(reader);
                    reader.ReadByte();
                }
            }
            reader.ReadBlockEnd(ref block);
        }
    }
}
