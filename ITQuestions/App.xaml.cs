using ITQuestions.DB;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ITQuestions
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnExit(ExitEventArgs e)
        {
            SyncLocalAndRemote.Instance.SyncAsync().GetAwaiter().GetResult();
            base.OnExit(e);
        }
    }
}
