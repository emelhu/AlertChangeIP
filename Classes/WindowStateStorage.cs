using Microsoft.UI.Windowing;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using Windows.Graphics;

namespace AlertChangeIP.Classes
{
    public static class WindowStateStorage
    {
        private static string filePath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AlertChangeIP", "WindowState.json");            // %LOCALAPPDATA%\AlertChangeIP\WindowState.json

        private static void Save(WindowState state)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);

            if (directoryPath is not null && !Directory.Exists(directoryPath))
            { 
                Directory.CreateDirectory(directoryPath);
            }

            try
            { 
                var json = JsonSerializer.Serialize(state);
                File.WriteAllText(filePath, json);
            }
            catch  
            {
                // Handle JSON serialization or file write error (e.g., log the error, etc.)
            }
        }

        public static void Save(AppWindow appWindow)
        {
            var state = GetCurrentWindowState(appWindow);

            if (state != null)
            {
                Save(state);
            }
        }

        private static WindowState? Load()
        {
            if (!File.Exists(filePath))
            { 
                return null;
            }

            var json = File.ReadAllText(filePath);

            try
            { 
                return JsonSerializer.Deserialize<WindowState>(json);
            }
            catch (Exception)
            {                
                return null;                                                                                                                // Handle JSON deserialization error (e.g., log the error, return null, etc.)
            }
        }

        private static WindowState? GetCurrentWindowState(AppWindow appWin)
        {
            //var appWin = this.AppWindow;

            
            if (appWin.Presenter is OverlappedPresenter p && p.State == OverlappedPresenterState.Minimized)
            {   // Minimalizált ablak esetén NEM mentünk pozíciót 
                return null;
            }

            return new WindowState
            {
                X       = appWin.Position.X,
                Y       = appWin.Position.Y,
                Width   = appWin.Size.Width,
                Height  = appWin.Size.Height
            };
        }

        public static void Restore(AppWindow appWin)
        {
            var state = WindowStateStorage.Load();
            if (state == null)
                return;

            CorrectPositionForMultiMonitor(state);

            //var appWin = this.AppWindow;

            appWin.Move(new PointInt32(state.X, state.Y));
            appWin.Resize(new SizeInt32(state.Width, state.Height));
        }

        private static void CorrectPositionForMultiMonitor(WindowState state)
        {
            var displayInfo = DisplayArea.GetFromPoint(
                new PointInt32(state.X, state.Y),
                DisplayAreaFallback.Primary);

            var area = displayInfo.WorkArea;

            // Ha az ablak teljesen kívül esne, visszahelyezzük a fő monitorra
            if (state.X < area.X || state.Y < area.Y ||
                state.X > area.X + area.Width ||
                state.Y > area.Y + area.Height)
            {
                state.X = area.X + 50;
                state.Y = area.Y + 50;
            }

            // Ha túl nagy lenne
            if (state.Width > area.Width)
                state.Width = area.Width;

            if (state.Height > area.Height)
                state.Height = area.Height;
        }
    }

    internal class WindowState
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
