using Autofac;
using AutoMapper.Contrib.Autofac.DependencyInjection;
using Clean.Architecture.Template.Application.PipelineBehaviors;
using MediatR;
using System.Reflection;
using Module = Autofac.Module;

namespace Clean.Architecture.Template.Application
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var assembly = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(assembly).AsImplementedInterfaces();
            builder.RegisterAutoMapper(assembly);

            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder.RegisterGeneric(typeof(LoggingBehaviour<,>)).AsImplementedInterfaces();
            builder.RegisterGeneric(typeof(ValidationBehavior<,>)).AsImplementedInterfaces();

            // request & notification handlers
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });
        }
    }
}
