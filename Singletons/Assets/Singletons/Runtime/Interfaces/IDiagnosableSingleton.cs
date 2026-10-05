using System;

namespace M320.Singletons
{
    public interface IDiagnosableSingleton : ISingleton
    {
        public event Action InstanceDiagnosed;
    }
    public interface IDiagnosableSingleton<TInstance> : IDiagnosableSingleton, ISingleton<TInstance>
    {

    }
}
