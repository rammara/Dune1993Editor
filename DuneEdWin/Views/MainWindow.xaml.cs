using DuneEd;
using DuneEdWin.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DuneEdWin.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainVM m_VM = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = m_VM;
            PopupObjectInfo.DataContext = m_VM;
            m_VM.PropertyChanged += ViewModel_PropertyChanged;
        } // MainWindow

        private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MainVM.Game))
                OnFileLoaded();
        } // ViewModel_PropertyChanged

        private double GameToCanvasX(int x)
        {
            double ratio = MapView.ActualWidth / 65536d;
            double offset = x * ratio + MapView.ActualWidth / 2;
            return offset;
        } // GameToCanvasX
        private double GameToCanvasY(int y)
        {
            double ratio = MapView.ActualHeight / 256d;
            double offset = y * ratio + MapView.ActualHeight / 2;
            return offset;
        } // GameToCanvasY
        private double CanvasToGameX(double x)
        {
            double ratio =  65536d / MapView.ActualWidth;
            double offset = x * ratio - 32768d;
            return offset;
        } // CanvasToGameX
        private double CanvasToGameY(double y)
        {
            double ratio = 256d / MapView.ActualHeight;
            double offset = y * ratio - 128d;
            return offset;
        } // CanvasToGameY
        private void OnFileLoaded()
        {
            const double PointSize = 20d;
            const double TroopPointSize = 10d;
            MapView.Children.Clear();
            if (m_VM.Game is null) return;
            m_VM.SelectedObject = null;
            foreach (var sietch in m_VM.Game.Sietches)
            {
                var clr = Brushes.Gray;
                var carthag = Values.FirstNames[2] + Values.LastNames[1];
                var arrakeen = Values.FirstNames[1] + Values.LastNames[2];
                if (sietch.Name == carthag) clr = Brushes.Red;
                if (sietch.Name == arrakeen) clr = Brushes.Blue;
                var point = new Ellipse()
                { Width = PointSize, Height = PointSize, Stroke = clr, StrokeThickness = 1 };
                if (!sietch.IsUndiscovered) point.Fill = clr;
                MapView.Children.Add(point);
                Canvas.SetLeft(point, GameToCanvasX(sietch.Xcoord) - PointSize / 2d);
                Canvas.SetTop(point, GameToCanvasY(sietch.Ycoord) - PointSize / 2d);
                var txt = new TextBlock()
                {
                    Text = sietch.Name, Foreground = clr
                };
                MapView.Children.Add(txt);
                txt.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

                Canvas.SetLeft(txt, GameToCanvasX(sietch.Xcoord) - txt.DesiredSize.Width / 2);
                Canvas.SetTop(txt, GameToCanvasY(sietch.Ycoord) + PointSize);
            }
            foreach (var troop in m_VM.Game.Troops.Where(t => t.IsOffSietch))
            {
                var clr = Brushes.DarkRed;
                var point = new Ellipse()
                { Width = TroopPointSize, Height = TroopPointSize, Stroke = clr, StrokeThickness = 1 };
                MapView.Children.Add(point);
                Canvas.SetLeft(point, GameToCanvasX(troop.Xcoord) - TroopPointSize / 2d);
                Canvas.SetTop(point, GameToCanvasY(troop.Ycoord) - TroopPointSize / 2d);
                var txt = new TextBlock()
                {
                    Text = $"Troop ID {troop.TroopId}",
                    Foreground = clr
                };
                MapView.Children.Add(txt);
                txt.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(txt, GameToCanvasX(troop.Xcoord) - txt.DesiredSize.Width / 2);
                Canvas.SetTop(txt, GameToCanvasY(troop.Ycoord) + TroopPointSize);
            }
            var CarthagCastle = m_VM.Game.Sietches.First();

            if (CarthagCastle is null) return;

            var viewport_w = MapViewScroller.ActualWidth;
            var viewport_h = MapViewScroller.ActualHeight;




            var scroll_x = GameToCanvasX(CarthagCastle.Xcoord) - viewport_w / 2;
            var scroll_y = GameToCanvasY(CarthagCastle.Ycoord) - viewport_h / 2;

            MapViewScroller.ScrollToHorizontalOffset(scroll_x);
            MapViewScroller.ScrollToVerticalOffset(scroll_y);



        } // OnFileLoaded

        private void OpenCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        private void OpenCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog()
            {
                Filter = "Savegame files (.sav)|*.sav|All files|*.*"
            };
            bool? result = dlg.ShowDialog();
            if (true == result)
            {
                string filename = dlg.FileName;
                m_VM.OnOpenFileCommand(filename);
            }
        } // OpenCommand_Executed


        private void SaveUnpackedCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = m_VM.Game is not null;

        private void SaveUnpackedCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.SaveFileDialog()
            {
                Filter = "All files|*.*"
            };
            bool? result = dlg.ShowDialog();
            if (true == result)
            {
                try
                {
                    m_VM.Game?.SaveUnpacked(dlg.FileName);
                    MessageBox.Show("File was saved sucsessfully.", "Save",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                } // try
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving file:\n" + ex.Message, "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                } // catch
            }
        } // SaveUnpackedCommand_Executed

        private void SaveFileCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
            => e.CanExecute = m_VM.Game is not null;


        private void SaveFileCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (m_VM.Game is null) return;
            var result = MessageBox.Show($"Would you like to make a backup copy of the file {m_VM.Game.Filename}?",
                "Save", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
            if (MessageBoxResult.Cancel == result) return;
            if (MessageBoxResult.Yes == result)
            {
                // make backup
                var directoryname = System.IO.Path.GetDirectoryName(m_VM.Game.Filename) ?? string.Empty;
                var backupfilename = System.IO.Path.GetFileNameWithoutExtension(m_VM.Game.Filename) + ".bak";
                var fullbackupfilename = System.IO.Path.Combine(directoryname, backupfilename);
                if (File.Exists(fullbackupfilename)) File.Delete(fullbackupfilename);
                File.Move(m_VM.Game.Filename, fullbackupfilename);
            }
            m_VM.Game.SavePacked(m_VM.Game.Filename);
        } // SaveFileCommand_Executed

        private void ExitCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
        private void ExitCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        } // ExitCommand_Executed

        private IGameObject? GetClosestObject(Point point)
        {
            const int ToleranceX = 400;
            const int ToleranceY = 2;
            if (m_VM.Game is null || !m_VM.Game.Sietches.Any()) return null;
            var game_pt = new Point(CanvasToGameX(point.X), CanvasToGameY(point.Y));
            var closestObject = m_VM.Game.GetLocatableObjects().Where
                (s => (s.Xcoord >= game_pt.X - ToleranceX && s.Xcoord <= game_pt.X + ToleranceX) &&
                      (s.Ycoord >= game_pt.Y - ToleranceY && s.Ycoord <= game_pt.Y + ToleranceY)).FirstOrDefault();
            return closestObject as IGameObject;
        } // GetClosestObject

        private void MapView_MouseMove(object sender, MouseEventArgs e)
        {
            // m_VM.HoveredObject = null;
            if (m_VM.Game is null || !m_VM.Game.Sietches.Any())
            {
                return;
            }

            
            var screen_pt = e.GetPosition(MapView);
            var closestObject = GetClosestObject(screen_pt);
            
            if (closestObject is not null)
            {
                var objPoint = MapView.PointToScreen(
                    new Point(
                        GameToCanvasX(closestObject.Xcoord), 
                        GameToCanvasY(closestObject.Ycoord)
                        )
                    );
                var popupX = objPoint.X;
                var popupY = objPoint.Y;
                PopupObjectInfo.HorizontalOffset = popupX + 10;
                PopupObjectInfo.VerticalOffset = popupY + 10;
                m_VM.HoveredObject = closestObject;
                PopupObjectInfo.IsOpen = true;
                return;
            }
            else
            {
                m_VM.HoveredObject = null;
                PopupObjectInfo.IsOpen = false;
            }
        } // MapView_MouseMove

        private void MapView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var screen_pt = e.GetPosition(MapView);
                var closestObject = GetClosestObject(screen_pt);
                m_VM.SelectedObject = closestObject;
            }
        } // MapView_MouseDown

        private void MapViewScroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            PopupObjectInfo.IsOpen = false;
        }
    } // class MainWindow
} // namespace
