using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace M320.Singletons
{
    public abstract class SingletonSO : ScriptableObject, IDiagnosableSingleton
    {
        protected enum DuplicateInstanceHandling
        {
            Diagnose,
            Abort
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
            if (TryInitializeInstance(DuplicateInstanceHandling.Abort))
            {
                InitializedInstance();
            }
        }

        protected virtual void OnAbort()
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
            OnInitializedInstance();

            InstanceWasInitialized = true;

            InstanceInitialized?.Invoke();
        }

        protected virtual void OnDiagnosedInstance() { }
        protected virtual void OnInitializedInstance() { }

        private bool TryInitializeInstance(DuplicateInstanceHandling handling)
        {
            if (Instance != null && (SingletonSO)Instance != this)
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
                case DuplicateInstanceHandling.Abort: AbortPlaymodeIfDuplicateInstanceExists(); break;
                default: throw new ArgumentException("Invalid Handle Mode!", nameof(handling));
            }
        }

        private void LogDuplicateInstance()
        {
            Debug.LogWarning(
                   $"Duplicate {typeof(SingletonSO).Name} detected: {name}",
                   this);
        }
        private void AbortPlaymodeIfDuplicateInstanceExists()
        {
#if UNITY_EDITOR
            Debug.LogError(
                $"Duplicate {typeof(SingletonSO).Name} detected: {name}. Aborting Play Mode.",
                this);

            EditorApplication.isPlaying = false;
#endif
        }

        protected void TryNullInstance()
        {
            if ((SingletonSO)Instance != this) return;

            SetInstance(null);
        }

        protected virtual void SetInstance(object instance)
        {
            Instance = instance;
        }
    }
    public abstract class SingletonSO<TInstance> : SingletonSO, IDiagnosableSingleton<TInstance>
        where TInstance : SingletonSO<TInstance>
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
