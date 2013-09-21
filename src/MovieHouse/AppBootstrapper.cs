using System;
using System.Dynamic;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Ninject;

namespace MovieHouse
{
    public class AppBootstrapper : Bootstrapper<AppViewModel>
    {
        private IKernel _kernel;

        protected override void Configure()
        {
            _kernel = new StandardKernel();

            _kernel.Bind<IWindowManager>().To<WindowManager>().InSingletonScope();
            _kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();
            _kernel.Bind<MovieConfig>().ToMethod(x => new MovieConfig(@"../../../mconfig.xml"));
            _kernel.Bind<MovieProcessQueue>().ToMethod(x => new MovieProcessQueue(9));
        }

        protected override object GetInstance(System.Type service, string key)
        {
            return _kernel.GetService(service);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowsStartupLocation = WindowStartupLocation.CenterScreen;
            settings.ResizeMode = ResizeMode.NoResize;
            settings.Owner = null;
            settings.ShowInTaskBar = true;
            settings.Title = "Movie House";
            settings.Icon = new BitmapImage(new Uri(@"pack://application:,,,/Icons/app.png"));

            DisplayRootViewFor<AppViewModel>(settings);
        }
    }
}