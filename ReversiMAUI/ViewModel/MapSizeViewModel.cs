using Reversi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReversiMAUI.ViewModel
{
    public class MapSizeViewModel : ViewModelBase
    {
        private MapSizeEnum mapSize;
        public MapSizeEnum MapSize
        {
            get => mapSize;
            set
            {
                mapSize = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SelecterText));
            }
        }
        public string SelecterText => mapSize.ToString();
    }
}
