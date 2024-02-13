using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reversi;
using Reversi.Persistance;

namespace ReversiMAUI.Persistance
{
    public class ReversiStore : IStore
    {
        public async Task<IEnumerable<String>> GetFilesAsync()
        {
            return await Task.Run(() => Directory.GetFiles(FileSystem.AppDataDirectory)
                .Select(Path.GetFileName)
                .Where(name => name?.EndsWith(".json") ?? false)
                .OfType<String>());
        }
        public async Task<DateTime> GetModifiedTimeAsync(String name)
        {
            var info = new FileInfo(Path.Combine(FileSystem.AppDataDirectory, name));

            return await Task.Run(() => info.LastWriteTime);
        }
    }
}
