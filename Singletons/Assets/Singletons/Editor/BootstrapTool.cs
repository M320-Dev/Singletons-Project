using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace M320.Singletons.Editor
{
    public static class BootstrapTool
    {
        private const string SceneName = SceneBootstrapper.SceneName;
        private const string ScenePath = "Assets/Scenes/" + SceneName + ".unity";

        [MenuItem("Tools/M_320/Singletons/Create Bootstrap Scene")]
        public static void CreateBootstrapScene()
        {
            if (System.IO.File.Exists(ScenePath))
            {
                Debug.LogWarning($"Bootstrap scene already exists at {ScenePath}.");
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            EditorSceneManager.SaveScene(scene, ScenePath);

            AddToBuildSettings(ScenePath);

            Debug.Log($"Created {SceneName}.");
        }

        private static void AddToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                if (scene.path == scenePath) return;
            }

            var updatedScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            scenes.CopyTo(updatedScenes, 0);

            updatedScenes[^1] = new EditorBuildSettingsScene(scenePath, true);

            EditorBuildSettings.scenes = updatedScenes;
        }
    }
}
