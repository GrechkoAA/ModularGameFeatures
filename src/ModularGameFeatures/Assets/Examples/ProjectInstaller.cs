using Modules.Utilities;
using Zenject;

namespace Examples
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .Bind<Logger>()
                .AsSingle()
                .WithArguments(new ILoggerSink[]
                {
                    new UnityConsoleSink(),
                    new FileLoggerSink()
                })
                .NonLazy();
        }
    }
}