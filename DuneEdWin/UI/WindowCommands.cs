using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DuneEdWin.UI
{
    public class WindowCommands
    {
        public static readonly RoutedUICommand OpenFile =
            new("OpenFileCommand",
                "OpenFileCommand",
                typeof(WindowCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.O, ModifierKeys.Control)
                });
        public static readonly RoutedUICommand SaveUnpacked =
            new("SaveUnpackedCommand",
                "SaveUnpackedCommand",
                typeof(WindowCommands));

        public static readonly RoutedUICommand SaveFile =
            new("SaveFileCommand",
                "SaveFileCommand",
                typeof(WindowCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.S, ModifierKeys.Control)
                });

        public static readonly RoutedUICommand ExitProgram =
            new("ExitCommand",
                "ExitCommand",
                typeof(WindowCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                });
    } // class WindowCommands
} // namespace 
