using System;

namespace M320.Singletons
{
    public interface ISingleton
    {
        public object Instance { get; }

        public bool InstanceWasInitialized { get; }

        public event Action InstanceInitialized;
    }
    public interface ISingleton<TInstance> : ISingleton
    {
        object ISingleton.Instance => Instance;

        new public TInstance Instance { get; }
    }
}
