namespace LightInject.Tests
{
    public static class ErrorMessages
    {
        public const string InvalidFuncFactoryExpression = "Only the new operator is supported in a function factory";
        public const string UnknownConstructorDependency = "Unresolved dependency [Target Type: LightInject.SampleLibrary.FooWithDependency], [Parameter: bar(LightInject.SampleLibrary.IBar)], [Requested dependency: ServiceType:LightInject.SampleLibrary.IBar, ServiceName:]";
        public const string UnableToResolveType = "Unable to resolve type: LightInject.SampleLibrary.IFoo, service name: ";
        public const string UnknownGenericDependency = "Unable to resolve type: LightInject.SampleLibrary.IFoo`1[System.Int32], service name: ";
        public const string UnknownPropertyDependency = "Unresolved dependency [Target Type: LightInject.SampleLibrary.FooWithProperyDependency], [Property: Bar(LightInject.SampleLibrary.IBar)], [Requested dependency: ServiceType:LightInject.SampleLibrary.IBar, ServiceName:]";
        public const string RecursiveDependency = "Recursive dependency detected: ServiceType:LightInject.SampleLibrary.IFoo, ServiceName:]";
        public const string GetInstanceOutSideScope = "Attempt to create a scoped instance";

        public const string DisposableOutsideScope = "The disposable instance (LightInject.SampleLibrary.DisposableFoo) was created outside a scope.";

        public const string RecursivePropertyDependency =
            "Unresolved dependency [Target Type: LightInject.SampleLibrary.FooWithRecursiveDependency], [Parameter: foo(LightInject.SampleLibrary.IFoo)], [Requested dependency: ServiceType:LightInject.SampleLibrary.IFoo, ServiceName:]";

        public const string UnknownMockService =
            "No mocked service found. ServiceType: LightInject.SampleLibrary.IBar, ServiceName: ";
    }
}
