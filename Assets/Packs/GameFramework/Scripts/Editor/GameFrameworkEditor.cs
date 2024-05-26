using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using GameFramework.System;
using System;
using System.IO;

namespace GameFramework.Editor
{
    public static class GameFrameworkEditor
    {
        [MenuItem("Tools/GameFramework/Init Project", priority = 1)]
        public static void Init()
        {
            CreateGameModeSettingsAsset();
            CreateProjectSettingsAsset();
            CreateGameplayTagsAsset();
            CreateWorldAsset();
        }

        private static void CreateProjectSettingsAsset()
        {
            if (Resources.Load(ProjectStatics.ProjectSettingsAssetPath) != null)
            {
                Debug.Log("ProjectSettings asset is already exists");
                return;
            }

            string path = "Assets/Resources/GameFramework";
            CheckFolders(ref path);

            string fileName = ProjectStatics.ProjectSettingsAssetPath.Substring(ProjectStatics.ProjectSettingsAssetPath.LastIndexOf('/'));

            var asset = ScriptableObject.CreateInstance<ProjectSettings>();
            AssetDatabase.CreateAsset(asset, path + $"/{fileName}.asset");

            GameModeSettings defaultGameModeSettings = Resources.Load<GameModeSettings>(ProjectStatics.GameModeSettingsAssetPath);
            if (defaultGameModeSettings != null)
            {
                asset.DefaultGameModeSettings = defaultGameModeSettings;
                EditorUtility.SetDirty(asset);
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"{fileName} created at Resources/GameFramework");
        }

        private static void CreateGameModeSettingsAsset()
        {
            if (Resources.Load(ProjectStatics.GameModeSettingsAssetPath) != null)
            {
                Debug.Log("GameModeSettings asset is already exists");
                return;
            }

            string path = "Assets/Resources/GameFramework";
            CheckFolders(ref path);

            string fileName = ProjectStatics.GameModeSettingsAssetPath.Substring(ProjectStatics.GameModeSettingsAssetPath.LastIndexOf('/'));

            var asset = ScriptableObject.CreateInstance<GameModeSettings>();
            AssetDatabase.CreateAsset(asset, path + $"/{fileName}.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"{fileName} created at Resources/GameFramework");
        }

        private static void CreateGameplayTagsAsset()
        {
            if (Resources.Load(ProjectStatics.GameplayTagsAssetPath) != null)
            {
                Debug.Log("GameplayTags asset is already exists");
                return;
            }

            File.WriteAllText("Assets/Resources/" + ProjectStatics.GameplayTagsAssetPath + ".txt", "");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("GameplayTagsAsset created at Resources/GameFramework");
        }

        private static void CreateWorldAsset()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Packs/GameFramework/World.prefab") != null)
            {
                Debug.Log("World prefab is already exists");
                return;
            }

            GameObject worldGameObject = new GameObject();
            worldGameObject.name = "World";
            worldGameObject.isStatic = true;
            worldGameObject.AddComponent<World>();

            string path = "Assets/Packs/GameFramework/" + worldGameObject.name + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(worldGameObject, path, out bool bSuccess);
            if (!bSuccess)
            {
                Debug.LogError("Could not created World prefab");
            }

            Debug.Log($"World prefab created at {path}");
            UnityEngine.Object.DestroyImmediate(worldGameObject);
        }

        private static void CheckFolders(ref string path)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/GameFramework"))
            {
                string guid = AssetDatabase.CreateFolder("Assets/Resources", "GameFramework");
                path = AssetDatabase.GUIDToAssetPath(guid);
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        private static void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            CheckScene(scene, false);
        }

        private static void OnSceneOpened(Scene scene, OpenSceneMode mode)
        {
            CheckScene(scene);
        }

        private static void CheckScene(Scene scene, bool bSaveScene = true)
        {
            var world = UnityEngine.Object.FindObjectOfType<World>();
            if (world == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Packs/GameFramework/World.prefab"), scene);
                if(bSaveScene)
                {
                    EditorSceneManager.SaveScene(scene);
                }
            }
        }
    }
}