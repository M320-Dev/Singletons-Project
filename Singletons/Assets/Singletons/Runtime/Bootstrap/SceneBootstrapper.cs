using UnityEngine;
using UnityEngine.SceneManagement;

namespace M320.Singletons
{
    public static class SceneBootstrapper
    {
        public const string SceneName = "Bootstrap Scene";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Load() 
        {
            if (!Application.CanStreamedLevelBeLoaded(SceneName)) return;

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var candiate = SceneManager.GetSceneAt(i);

                if (candiate.name == SceneName) return;
            }

            SceneManager.LoadScene(SceneName, LoadSceneMode.Additive);
        }
    }
}
