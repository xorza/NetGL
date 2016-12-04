using NetGL.Constructor.UI;
using NetGL.Core.Infrastructure;
using NetGL.SceneGraph.Control;
using System;
using System.Threading;
using System.Windows;

namespace NetGL.Constructor {
    internal static class Program {
        [STAThread]
        private static void Main() {
            Log.AddLogger(new DebugLogger());
            try {
                var fileLogger = new FileLogger("log.txt");
                Log.AddLogger(fileLogger);
            }
            catch (Exception ex) {
                Log.Exception(ex);
            }

            new Application().Resources.Source = new Uri("pack://application:,,,/UI/AppResources.xaml");

            var t = new Thread(StartWindow);
            t.SetApartmentState(ApartmentState .STA);
            t.Start();
        }

        private static void StartWindow() {
            var dispatcher = new RenderDispatcher();

            var wnd = new BaseWindow();
            wnd.Title = "NetGL Contructor";
            wnd.Content = new Main();
            wnd.Show();

            wnd.Closed += (s, ea) => {
                dispatcher.Shutdown();
            };

            Log.Info("starting Constructor main loop");
            dispatcher.Run();
        }
    }
}