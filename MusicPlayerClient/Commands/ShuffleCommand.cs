using MusicPlayerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicPlayerClient.Commands
{
    public class ShuffleCommand
    {
        private readonly IMusicPlayerService _musicService;

        public ShuffleCommand(IMusicPlayerService musicService)
        {
            _musicService = musicService;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _musicService.Shuffle();
        }
    }
}
