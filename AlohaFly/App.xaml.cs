using System.Windows;
using Telerik.Windows.Controls;

namespace AlohaFly
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {

        App()
        {

        }

        /// <summary>
        /// Application Entry Point.
        /// </summary>
        [System.STAThreadAttribute()]
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public static void Main()
        {
            bool result = true;


#if DEBUG
            //Authorization.DoAuthorization("v.p", "123");
            //Authorization.DoAuthorization("n.p", "123");
            //Authorization.DoAuthorization("oper", "123");


            UI.wndLogin wndLogin = new UI.wndLogin();
            wndLogin.ShowDialog();
            result = wndLogin.Result;


#else
            /*
            UI.wndLogin wndLogin = new UI.wndLogin();
            wndLogin.ShowDialog();
            result = wndLogin.Result;
            */

            Authorization.DoAuthorization("v.p", "123");

#endif

            if (result)
            {

                //mainWindow.Show();
                // StyleManager.ApplicationTheme = new Expression_DarkTheme();
                //StyleManager.ApplicationTheme = new Office2016Theme();
                StyleManager.ApplicationTheme = new CrystalTheme();
                //StyleManager.ApplicationTheme = new FluentTheme();
                //StyleManager.ApplicationTheme = new VisualStudio2013Theme();
                //StyleManager.ApplicationTheme = new Office_BlueTheme();
                //StyleManager.ApplicationTheme = new Windows7Theme();
                //StyleManager.ApplicationTheme = new FluentTheme();
                AlohaFly.App app = new AlohaFly.App();
                app.InitializeComponent();
                //MainWindow mainWindow = new MainWindow();
                //app.Run(mainWindow);
                app.Run();
            }



        }


        protected override void OnStartup(StartupEventArgs e)
        {
            //new UI.CtrlMain().Show();
            var wnd = new MainWindow();
            wnd.Show();
            base.OnStartup(e);


        }
    }
}
