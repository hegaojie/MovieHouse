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
        }

        protected override object GetInstance(System.Type service, string key)
        {
            return _kernel.GetService(service);
        }
    }
}