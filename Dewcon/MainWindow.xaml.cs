using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace Dewcon
{
  public partial class MainWindow
  {
    //private const int SERVERPORT = 2448;
    //private const string SERVERHOSTNAME = "127.0.0.1";
    private const int Entrycollectionsize = 100;
    private readonly string[] _tempVars = new string[Entrycollectionsize];
    private int _tempCount;

    public MainWindow()
    {
      InitializeComponent();
      if(CheckIfProcessIsRunning("eldorado"))
        AppendDebugLine("ElDorito is running!" + Environment.NewLine + Environment.NewLine, Color.FromRgb(0, 255, 0));
      else
        AppendDebugLine("ElDorito is not running. You can start eldorito with 'start <ElDorito Path>' or by starting ElDorito normally." + Environment.NewLine, Color.FromRgb(255, 255, 0));
    }

    private void AppendDebugLine(string status, Color color, bool updateLabel = true)
    {
      if (updateLabel)
      {
        Dispatcher.Invoke(() =>
        {
          var tr = new TextRange(DebugLog.Document.ContentEnd, DebugLog.Document.ContentEnd)
          {
            Text = status
          };

          tr.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        });
      }
    }

    private static bool CheckIfProcessIsRunning(string nameSubstring)
    {
      return Process.GetProcesses().Any(p => p.ProcessName.Contains(nameSubstring));
    }

    private void UserBox_KeyDown(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Return)
      {
        if (_tempCount < Entrycollectionsize && UserBox.Text != string.Empty && _tempVars[_tempCount] != string.Empty)
        {
          //Console.WriteLine(_tempVars[_tempCount] + @" " + _tempCount);
          while (_tempVars[_tempCount] != null)
            _tempCount++;
            
          _tempVars[_tempCount] = UserBox.Text;
          _tempCount++;
        }

        AppendDebugLine(Environment.NewLine + UserBox.Text, Color.FromRgb(51, 153, 255));

        if (!UserBox.Text.Contains("start"))
          AppendDebugLine(Rcon.DewCmd(UserBox.Text), Color.FromRgb(150, 150, 150));

        if (UserBox.Text.Contains("start"))
        {
          string start = UserBox.Text.Replace("start ", string.Empty) + @"\eldorado.exe";

          var startInfo = new ProcessStartInfo
          {
            FileName = @start,
            Arguments = "-launcher"
          };

          try
          {
            Process.Start(startInfo);
          }
          catch
          {
            AppendDebugLine("Could not find eldorado.exe is the path correct?" + Environment.NewLine, Color.FromRgb(255, 255, 0));
          }
        }

        UserBox.Clear();
        DebugLog.ScrollToEnd();
      }


      if (e.Key == Key.Up)
      {
        int temp = _tempCount - 1;
        if (temp >= 0)
        {
          Console.WriteLine(_tempVars[temp] + @" " + temp);
          UserBox.Clear();
          UserBox.Text = _tempVars[temp];
          UserBox.CaretIndex = UserBox.Text.Length;
          _tempCount--;
        }
        else
        {
          Console.WriteLine(@"Can't go up (Last entry)");
        }
      }

      if (e.Key == Key.Down)
      {
        int temp = _tempCount + 1;
        if (temp < Entrycollectionsize && _tempVars[temp - 1] != null)
        {
          Console.WriteLine(_tempVars[temp] + @" " + temp);
          UserBox.Clear();
          UserBox.Text = _tempVars[temp];
          UserBox.CaretIndex = UserBox.Text.Length;
          _tempCount++;
        }
        else
        {
          Console.WriteLine(@"Can't go down (First entry)");
        }
      }
    }
  }
}