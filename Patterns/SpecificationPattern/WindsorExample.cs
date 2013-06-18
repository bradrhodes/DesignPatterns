using System.Linq;
using System.Collections.Generic;
using Castle.Core;
using Castle.DynamicProxy;
using Castle.MicroKernel;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Facilities;

namespace Patterns.SpecificationPattern
{
    public class WindsorExample
    {
        [TestClass]
        public class TaxStrategyTests
        {
            [TestMethod]
            public void InjectUsingCollectionResolver()
            {
                var container = new WindsorContainer();
                
                container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel));

                container.Register(Component.For<ITaxStrategy>().ImplementedBy<USTaxStrategy>());
                container.Register(Component.For<ITaxStrategy>().ImplementedBy<CanadaTaxStrategy>());
                container.Register(Component.For<IOrderProcessor>().ImplementedBy<OrderProcessor>());
                container.Register(Component.For<ITaxStrategyFactory>().ImplementedBy<TaxStrategyFactory>());

                var order = new Order { Country = "US" };
                container.Resolve<IOrderProcessor>().Process(order);
                Assert.AreEqual(10, order.Tax);
            }

            [TestMethod]
            public void InjectWithSpecificRegistration()
            {
                var container = new WindsorContainer();

                container.Register(Component.For<ITaxStrategy>().ImplementedBy<USTaxStrategy>());
                container.Register(Component.For<ITaxStrategy>().ImplementedBy<CanadaTaxStrategy>());
                container.Register(Component.For<IOrderProcessor>().ImplementedBy<OrderProcessor>());

                container.Register(
                    Component.For<ITaxStrategyFactory>()
                             .ImplementedBy<TaxStrategyFactory>()
                             .DependsOn(Dependency.OnValue<ITaxStrategy[]>(container.ResolveAll<ITaxStrategy>())));


                var order = new Order { Country = "US" };
                container.Resolve<IOrderProcessor>().Process(order);
                Assert.AreEqual(10, order.Tax);

                var order2 = new Order { Country = "CA" };
                container.Resolve<IOrderProcessor>().Process(order2);
                Assert.AreEqual(5, order2.Tax);
            }

            [TestMethod]
            public void InjectWithFactoryMethod()
            {
                var container = new WindsorContainer();
                container.Register(Component.For<ITaxStrategy>().ImplementedBy<USTaxStrategy>().Named("USTaxStrategy"));
                container.Register(Component.For<ITaxStrategy>().ImplementedBy<CanadaTaxStrategy>().Named("CanadaTaxStrategy"));
                container.Register(Component.For<IOrderProcessor>().ImplementedBy<OrderProcessor>());
                container.Register(Component.For<ITaxStrategyFactory>()
                                            .UsingFactoryMethod(
                                                kernel => new TaxStrategyFactory(new ITaxStrategy[]
                                                    {
                                                        kernel.Resolve<ITaxStrategy>("USTaxStrategy"),
                                                        kernel.Resolve<ITaxStrategy>("CanadaTaxStrategy"),
                                                    })));
                
                
                var order = new Order {Country = "US"};
                container.Resolve<IOrderProcessor>().Process(order);
                Assert.AreEqual(10, order.Tax);
            }
        }
    }
}