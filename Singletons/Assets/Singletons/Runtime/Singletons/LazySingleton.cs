using System;

namespace M320.Singletons
{
    public abstract class LazySingleton : ISingleton
    {
        public object Instance { get; private set; }

        public bool InstanceWasInitialized { get; protected set; }

        public event Action InstanceInitialized;

        protected void InvokeInstanceInitialized()
        {
            InstanceInitialized?.Invoke();
        }

        protected virtual void SetInstance(object instance)
        {
            Instance = instance;
        }
    }
    public abstract class LazySingleton<TInstance> : LazySingleton, ISingleton<TInstance>
        where TInstance : LazySingleton<TInstance>, new()
    {
        private static TInstance _instance;
        new public static TInstance Instance 
        {
            get 
            {
                if (_instance == null) 
                {
                    TInstance instance = new();

                    _instance.SetInstance(instance);
                    instance.InstanceWasInitialized = true;
                    _instance.InvokeInstanceInitialized();
                }
                return _instance;
            }
        }
        TInstance ISingleton<TInstance>.Instance => Instance;

        protected override void SetInstance(object instance)
        {
            base.SetInstance(instance);
            _instance = (TInstance)instance;
        }
    }
}
