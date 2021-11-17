using Autofac;
using System.Reflection;
using Module = Autofac.Module;

namespace Clean.Architecture.Template.Infrastructure
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
                .AsImplementedInterfaces();

            builder.RegisterModule<Application.AutofacModule>();
        }
    }
}
