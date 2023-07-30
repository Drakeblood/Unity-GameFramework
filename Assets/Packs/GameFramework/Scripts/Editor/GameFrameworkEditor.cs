using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

using GameFramework.System;

namespace GameFramework.Editor
{
    public static class GameFrameworkEditor
    {
        [MenuItem("Tools/GameFramework/Init Project", priority = 1)]
        public static void Init()
        {
            CreateGameModeSettingsAsset();
            CreateProjectSettingsAsset();
            CreateWorldAsset();
        }

        private static void CreateProjectSettingsAsset()
        {
            if (Resources.Load(ProjectStatics.ProjectSettingsAssetPath) != null)
            {
                Debug.Log("ProjectSettings asset is already exists");
                return;
            }

            string Path = "Assets/Resources/GameFramework";
            CheckFolders(ref Path);

            string FileName = ProjectStatics.ProjectSettingsAssetPath.Substring(ProjectStatics.ProjectSettingsAssetPath.LastIndexOf('/'));

            var Asset = ScriptableObject.CreateInstance<ProjectSettings>();
            AssetDatabase.CreateAsset(Asset, Path + $"/{FileName}.asset");

            GameModeSettings DefaultGameModeSettings = Resources.Load<GameModeSettings>(ProjectStatics.GameModeSettingsAssetPath);
            if (DefaultGameModeSettings != null)
            {
                Asset.DefaultGameModeSettings = DefaultGameModeSettings;
                EditorUtility.SetDirty(Asset);
            }
            
            AssetDatabase.SaveAssets();

            Debug.Log($"{FileName} created at Resources/GameFramework");
        }

        private static void CreateGameModeSettingsAsset()
        {
            if (Resources.Load(ProjectStatics.GameModeSettingsAssetPath) != null)
            {
                Debug.Log("GameModeSettings asset is already exists");
                return;
            }

            string Path = "Assets/Resources/GameFramework";
            CheckFolders(ref Path);

            string FileName = ProjectStatics.GameModeSettingsAssetPath.Substring(ProjectStatics.GameModeSettingsAssetPath.LastIndexOf('/'));

            var Asset = ScriptableObject.CreateInstance<GameModeSettings>();
            AssetDatabase.CreateAsset(Asset, Path + $"/{FileName}.asset");
            AssetDatabase.SaveAssets();

            Debug.Log($"{FileName} created at Resources/GameFramework");
        }

        private static void CreateWorldAsset()
        {
            if (AssetDatabase.LoadAssetAtPath<GameObject>("Packs/GameFramework/World") != null)
            {
                Debug.Log("World prefab is already exists");
                return;
            }

            GameObject WorldGameObject = new GameObject();
            WorldGameObject.name = "World";
            WorldGameObject.isStatic = true;
            WorldGameObject.AddComponent<World>();

            string Path = "Assets/Packs/GameFramework/" + WorldGameObject.name + ".prefab";

            PrefabUtility.SaveAsPrefabAsset(WorldGameObject, Path, out bool bSuccess);
            if (!bSuccess)
            {
                Debug.LogError("Could not created World prefab");
            }

            Debug.Log($"World prefab created at {Path}");
            UnityEngine.Object.DestroyImmediate(WorldGameObject);
        }

        private static void CheckFolders(ref string Path)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/GameFramework"))
            {
                string Guid = AssetDatabase.CreateFolder("Assets/Resources", "GameFramework");
                Path = AssetDatabase.GUIDToAssetPath(Guid);
            }
        }

        [InitializeOnLoadMethod]
        private static void InitializeEditor()
        {
            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        private static void OnNewSceneCreated(Scene InScene, NewSceneSetup InSetup, NewSceneMode InMode)
        {
            CheckScene(InScene, false);
        }

        private static void OnSceneOpened(Scene InScene, OpenSceneMode InMode)
        {
            CheckScene(InScene);
        }

        private static void CheckScene(Scene InScene, bool bSaveScene = true)
        {
            var World = UnityEngine.Object.FindObjectOfType<World>();
            if (World == null)
            {
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Packs/GameFramework/World.prefab"), InScene);
                if(bSaveScene)
                {
                    EditorSceneManager.SaveScene(InScene);
                }
            }
        }
    }
}