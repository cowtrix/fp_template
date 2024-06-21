using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using FPTemplate;
using FPTemplate.Utilities;

namespace FPTemplate.World
{
    public class DependentLevels : ExtendedMonoBehaviour
    {
        public List<SceneReference> Levels = new List<SceneReference>();

#if UNITY_EDITOR
        [ContextMenu("Load Dependent Levels")]
        public void LoadInEditor()
        {
            foreach (var s in Levels)
            {
                var loadedScene = UnityEditor.SceneManagement.EditorSceneManager.GetSceneByPath(s.ScenePath);
                if (loadedScene == null || !loadedScene.isLoaded)
                    UnityEditor.SceneManagement.EditorSceneManager.OpenScene(s, UnityEditor.SceneManagement.OpenSceneMode.Additive);
            }
        }
#endif

        private void Awake()
        {
            foreach (var s in Levels)
            {
                var loadedScene = SceneManager.GetSceneByPath(s.ScenePath);
                if (loadedScene == null && !loadedScene.isLoaded)
                    SceneManager.LoadScene(s, LoadSceneMode.Additive);
            }
        }
    }
}