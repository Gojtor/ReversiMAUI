using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace Reversi.Persistance
{
    public class ReversiFileDataAccess : IReversiDataAccess
    {
        private string? dir = String.Empty; 
        public ReversiFileDataAccess(string? saveDir=null) 
        {
            dir = saveDir;
        }
        public async Task<ReversiTable> LoadAsync(String path)
        {
            if (!String.IsNullOrEmpty(dir))
                path = Path.Combine(dir, path);

            string savedGame = await File.ReadAllTextAsync(path);
            return new ReversiTable(JsonSerializer.Deserialize<SaveReversiTable>(savedGame)!);
        }

        public async Task SaveAsync(String path, ReversiTable table)
        {
            if (!String.IsNullOrEmpty(dir))
                path = Path.Combine(dir, path);

            string saveString = JsonSerializer.Serialize(new SaveReversiTable(table));
            await File.WriteAllTextAsync(path, saveString); 
        }
    }
}
