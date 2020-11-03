using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantasticMusicPlayer.dbo.Model
{
    public class SongEntry
    {

        public int ID { get; set; } = 0;
        public String Path { get; set; }
        public String Name { get; set; }
        public SongEntry(int iD, string path)
        {
            ID = iD;
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public SongEntry(string path)
        {
            Path = path;
            Name = System.IO.Path.GetFileNameWithoutExtension(Path);
        }

        public override bool Equals(object obj)
        {
            return obj is SongEntry entry &&
                   ID == entry.ID &&
                   Path == entry.Path;
        }

        public override int GetHashCode()
        {
            int hashCode = 867923638;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Path);
            return hashCode;
        }
    }
}
