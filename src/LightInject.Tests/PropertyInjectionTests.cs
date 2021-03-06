namespace LightInject.Tests
{
    using System;
    using System.Reflection;
    using System.Text;
    using LightInject;
    using LightInject.SampleLibrary;
    using Xunit;

    public class PropertyInjectionTests : TestBase
    {
        [Fact]
        public void GetInstance_KnownDependency_InjectsPropertyDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithPropertyDependency>();
            var instance = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            Assert.IsAssignableFrom<Bar>(instance.Bar);
        }

        [Fact]
        public void GetInstance_UnKnownDependency_ReturnsInstanceWithoutDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithPropertyDependency>();
            var instance = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            Assert.Null(instance.Bar);
        }

        [Fact]
        public void GetInstance_OpenGenericPropertyDependency_InjectsPropertyDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register(typeof(IFoo<>), typeof(FooWithGenericPropertyDependency<>));
            var instance = (FooWithGenericPropertyDependency<IBar>)container.GetInstance<IFoo<IBar>>();
            Assert.IsAssignableFrom<Bar>(instance.Dependency);
        }

        [Fact]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithPropertyDependency>();
            var instance1 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            Assert.NotEqual(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithPropertyDependency>();
            FooWithPropertyDependency instance1;
            FooWithPropertyDependency instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            }
            Assert.NotEqual(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingleonDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Register<IFoo, FooWithPropertyDependency>();
            var instance1 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            var instance2 = (FooWithPropertyDependency)container.GetInstance<IFoo>();
            Assert.Equal(instance1.Bar, instance2.Bar);
        }

        [Fact]
        public void GetInstance_DependencyWithTransientLifeCycle_InjectsTransientDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.NotEqual(instance.Bar1, instance.Bar2);
        }

        [Fact]
        public void GetInstance_DependencyWithSingletonLifeCycle_InjectsSingletonDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerContainerLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            Assert.Equal(instance.Bar1, instance.Bar2);
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsSameDependenciesForSingleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();
            using (container.BeginScope())
            {
                var instance = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
                Assert.Equal(instance.Bar1, instance.Bar2);
            }
        }

        [Fact]
        public void GetInstance_DependencyWithRequestLifeCycle_InjectsTransientDependenciesForMultipleRequest()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>(new PerScopeLifetime());
            container.Register<IFoo, FooWithSamePropertyDependencyTwice>();

            FooWithSamePropertyDependencyTwice instance1;
            FooWithSamePropertyDependencyTwice instance2;
            using (container.BeginScope())
            {
                instance1 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            }
            using (container.BeginScope())
            {
                instance2 = (FooWithSamePropertyDependencyTwice)container.GetInstance<IFoo>();
            }
            Assert.NotEqual(instance1.Bar1, instance2.Bar2);
        }

        [Fact]
        public void GetInstance_ValueTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(42);
            container.Register<IFoo, FooWithValueTypePropertyDependency>();
            var instance = (FooWithValueTypePropertyDependency)container.GetInstance<IFoo>();
            Assert.Equal(42, instance.Value);
        }

        [Fact]
        public void GetInstance_EnumDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance(Encoding.UTF8);
            container.Register<IFoo, FooWithEnumPropertyDependency>();
            var instance = (FooWithEnumPropertyDependency)container.GetInstance<IFoo>();
            Assert.Equal(Encoding.UTF8, instance.Value);
        }

        [Fact]
        public void GetInstance_ReferenceTypeDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.RegisterInstance("SomeValue");
            container.Register<IFoo, FooWithReferenceTypePropertyDependency>();
            var instance = (FooWithReferenceTypePropertyDependency)container.GetInstance<IFoo>();
            Assert.Equal("SomeValue", instance.Value);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithInitializer_ReturnsInstanceWithDependencies()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register<IFoo>(f => new FooWithPropertyDependency { Bar = f.GetInstance<IBar>() });
            var instance = (FooWithPropertyDependency)container.GetInstance(typeof(IFoo));
            Assert.NotNull(instance.Bar);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithoutInitializer_ReturnsInstanceWithoutDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo>(f => new FooWithPropertyDependency());
            var instance = (FooWithPropertyDependency)container.GetInstance(typeof(IFoo));
            Assert.Null(instance.Bar);
        }

        [Fact]
        public void GetInstance_FuncFactoryWithStringConstantInitializer_ReturnsInstanceWithDependency()
        {
            var container = CreateContainer();
            container.Register(typeof(IBar), typeof(Bar));
            container.Register<IFoo>(f => new FooWithReferenceTypePropertyDependency { Value = "SomeValue" });
            var instance = (FooWithReferenceTypePropertyDependency)container.GetInstance(typeof(IFoo));
            Assert.Equal("SomeValue", instance.Value);
        }

        [Fact]
        public void GetInstance_RequestLifeCycle_CallConstructorsOnDependencyOnlyOnce()
        {
            var container = CreateContainer();
            Bar.InitializeCount = 0;
            container.Register(typeof(IBar), typeof(Bar), new PerScopeLifetime());
            container.Register(typeof(IFoo), typeof(FooWithSamePropertyDependencyTwice));
            using (container.BeginScope())
            {
                container.GetInstance<IFoo>();
                Assert.Equal(1, Bar.InitializeCount);
            }
        }

        [Fact]
        public void GetInstance_StaticDependency_DoesNotInjectDependency()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithStaticDependency>();
            container.GetInstance<IFoo>();
            Assert.Null(FooWithStaticDependency.Bar);
        }

        [Fact]
        public void InjectProperties_KnownClassWithPropertyDependency_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<FooWithPropertyDependency>();
            container.Register<IBar, Bar>();
            var fooWithProperyDependency = new FooWithPropertyDependency();

            var result = (FooWithPropertyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsAssignableFrom<Bar>(result.Bar);
        }

        [Fact]
        public void InjectProperties_FuncDependency_InjectsDependency()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            // container.Register<IBar, Bar>((factory, bar) => new Bar());
            var fooWithFuncPropertyDependency = new FooWithFuncPropertyDependency();

            var result = (FooWithFuncPropertyDependency)container.InjectProperties(fooWithFuncPropertyDependency);
            Assert.NotNull(result.BarFunc);
        }



        [Fact]
        public void InjectProperties_UnknownClassWithPropertyDependency_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            var fooWithProperyDependency = new FooWithPropertyDependency();

            var result = (FooWithPropertyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsAssignableFrom<Bar>(result.Bar);
        }

        [Fact]
        public void InjectProperties_UnknownClassWithPropertyDependencyRegisteredAsInstance_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.RegisterInstance<IBar>(new Bar());
            var fooWithProperyDependency = new FooWithPropertyDependency();

            var result = (FooWithPropertyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsAssignableFrom<Bar>(result.Bar);
        }
        [Fact]
        public void InjectProperties_FuncFactory_InjectsPropertyDependencies()
        {
            var container = CreateContainer();
            container.Register<IBar, Bar>();
            container.Register<IBar, AnotherBar>("AnotherBar");
            container.Register(f => new FooWithPropertyDependency() { Bar = f.GetInstance<IBar>("AnotherBar") });
            var fooWithProperyDependency = new FooWithPropertyDependency();

            var result = (FooWithPropertyDependency)container.InjectProperties(fooWithProperyDependency);

            Assert.IsAssignableFrom<Bar>(result.Bar);
        }

        [Fact]
        public void InjectProperties_RecursiveDependency_ThrowsException()
        {
            var container = CreateContainer();
            container.Register<IFoo, FooWithRecursiveDependency>();

            var barWithPropertyDependency = new BarWithPropertyDependency();
            var exception =
                Assert.Throws<InvalidOperationException>(() => container.InjectProperties(barWithPropertyDependency));
            Assert.Equal(ErrorMessages.RecursivePropertyDependency, exception.Message);
        }

        [Fact]
        public void InjectProperties_InstanceWithUnknownConstructorDependencyAndMultipleConstructors_ReturnsInstance()
        {
            var container = new ServiceContainer();
            var fooWithDependency = new FooWithTwoConstructors(42);
            var result = container.InjectProperties(fooWithDependency);
        }

        [Fact]
        public void GetInstance_ClassWithIndexer_CanGetInstance()
        {
            var container = new ServiceContainer();
            container.Register<object, Foo>();
            container.Register<FooWithIndexer>();
            var instance = container.GetInstance<FooWithIndexer>();
            Assert.NotNull(instance);
        }

        [Fact]
        public void GetInstance_ClassWithObjectPropertyCanGetInstance()
        {
            var container = new ServiceContainer();
            container.Register<object, Foo>();
            container.Register<FooWithObjectProperty>();
            var instance = container.GetInstance<FooWithObjectProperty>();
            Assert.NotNull(instance.Property);
        }

        [Fact]
        public void ToString_PropertyDependency_ReturnsDescriptiveDescription()
        {
            PropertyDependency propertyDependency = new PropertyDependency();
            propertyDependency.Property = typeof(FooWithPropertyDependency).GetTypeInfo().GetProperty("Bar");
            var description = propertyDependency.ToString();
            Assert.StartsWith("[Target Type", description);
        }

        [Fact]
        public void Setting_EnablePropertyInjection_false_disables_property_injection()
        {
            var container = new ServiceContainer(new ContainerOptions
            {
                EnablePropertyInjection = false
            });
            container.Register<object, Foo>();
            container.Register<FooWithObjectProperty>();

            var instance = container.GetInstance<FooWithObjectProperty>();

            Assert.Null(instance.Property);
        }
    }
}