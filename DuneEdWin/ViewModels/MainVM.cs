using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuneEd;

namespace DuneEdWin.ViewModels
{
    public class MainVM : ViewModelBase
    {
        public SavedGame? Game
        {
            get => GetValue<SavedGame>(nameof(Game));
            set => SetValue(nameof(Game), value);
        } // Game

        public string? Filename
        {
            get => GetValue<string>(nameof(Filename));
            set => SetValue(nameof(Filename), value);
        } // Filename

        public IGameObject? SelectedObject
        {
            get => GetValue<IGameObject>(nameof(SelectedObject));
            set => SetValue(nameof(SelectedObject), value);
        } // SelectedObject

        public IGameObject? HoveredObject
        {
            get => GetValue<IGameObject>(nameof(HoveredObject));
            set => SetValue(nameof(HoveredObject), value);
        } // HoveredObject

        public void OnOpenFileCommand(string filename)
        {
            Filename = filename;
            Game = new(filename);
        } // OnOpenFileCommand


    } // class MainVM
} // namespace
