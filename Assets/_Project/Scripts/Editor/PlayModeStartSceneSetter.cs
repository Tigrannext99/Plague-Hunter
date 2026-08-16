using UnityEditor;
using UnityEditor.SceneManagement;

namespace PlagueHunter.EditorTools
{
    [InitializeOnLoad]
    public static class PlayModeStartSceneSetter
    {
        private const string BootstrapScenePath = "Assets/_Project/Scenes/Bootstrap.unity";

        static PlayModeStartSceneSetter()
        {
            var bootstrap = AssetDatabase.LoadAssetAtPath<SceneAsset>(BootstrapScenePath);

            if (bootstrap == null)
                return;

            EditorSceneManager.playModeStartScene = bootstrap;
        }
    }
}