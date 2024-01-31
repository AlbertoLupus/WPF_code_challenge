using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace wscad_coding_challenge;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
    }

    private void Application_Startup(object sender, StartupEventArgs e)
    {
        string? path = null;
        if (e.Args.Length < 1)
        {
            MessageBox.Show("No Path specified!");
        }
        else
        {
            path = e.Args[0];
        }

        MainWindow wnd = new MainWindow(path);
        wnd.Show();
    }
}
