using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace M320.Singletons
{
    public abstract class Singleton : MonoBehaviour, IDiagnosableSingleton
    {
        protected enum DuplicateInstanceHandling
        {
            Diagnose,
            Destroy
        }

        public object Instance { get; private set; }
        public bool InstanceWasInitialized { get; private set; }

        public event Action InstanceDiagnosed;
        public event Action InstanceInitialized;

        protected virtual void OnValidate()
        {
            if (TryInitializeInstance(DuplicateInstanceHandling.Diagnose))
            {
                DiagnosedInstance();
            }
        }
        protected virtual void Awake()
        {
            if (TryInitializeInstance(DuplicateInstanceHandling.Destroy))
            {
                InitializedInstance();
            }
        }

        protected virtual void OnDestroy()
        {
            TryNullInstance();
        }

        private void DiagnosedInstance()
        {
            OnDiagnosedInstance();

            InstanceDiagnosed?.Invoke();
        }
        private void InitializedInstance()
        {
            if (this is IScenePersistentSingleton) 
            {
                DontDestroyOnLoad(gameObject);
            }

            OnInitializedInstance();

            InstanceWasInitialized = true;
            InstanceInitialized?.Invoke();
        }

        protected virtual void OnDiagnosedInstance() { }
        protected virtual void OnInitializedInstance() { }

        private bool TryInitializeInstance(DuplicateInstanceHandling handling)
        {
#if UNITY_EDITOR
            if (EditorUtility.IsPersistent(this)) return false;
#endif

            if (Instance != null && (Singleton)Instance != this)
            {
                HandleDuplicateInstance(handling);
                return false;
            }

            if (handling != DuplicateInstanceHandling.Diagnose)
            {
                SetInstance(this);
            }
            return true;
        }
        private void HandleDuplicateInstance(DuplicateInstanceHandling handling)
        {
            switch (handling)
            {
                case DuplicateInstanceHandling.Diagnose: LogDuplicateInstance(); break;
                case DuplicateInstanceHandling.Destroy: DestroyDuplicateInstance(); break;
                default: throw new ArgumentException("Invalid Handle Mode!", nameof(handling));
            }
        }

        private void LogDuplicateInstance()
        {
            Debug.LogWarning(
                   $"Duplicate {typeof(Singleton).Name} detected: {name}",
                   this);
        }
        private void DestroyDuplicateInstance()
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Destroy Duplicate Singleton {name}");
#endif

            if (Application.isPlaying) Destroy(gameObject);
            else DestroyImmediate(gameObject);
        }

        private void TryNullInstance()
        {
            if ((Singleton)Instance != this) return;

            SetInstance(null);
        }

        protected virtual void SetInstance(object instance) 
        {
            Instance = instance;
        }
    }
    public abstract class Singleton<TInstance> : Singleton, IDiagnosableSingleton<TInstance>
        where TInstance : Singleton<TInstance>
    {
        private static TInstance _instance;
        new public static TInstance Instance => _instance;
        TInstance ISingleton<TInstance>.Instance => _instance;

        protected override void SetInstance(object instance)
        {
            base.SetInstance(instance);
            _instance = (TInstance)instance;
        }
    }
}
