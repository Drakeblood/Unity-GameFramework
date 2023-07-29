using System;

using UnityEngine;

using System.Collections.Generic;

namespace GameFramework.System
{
    public sealed partial class World : MonoBehaviour
    {
        private static GameInstance GameInstance;

        private partial void Awake();

        #region GameMode

        [SerializeField] private GameModeSettings GameModeSettingsOverride;
        private GameMode GameMode;

        private partial void InitGameMode(GameModeSettings GameModeSettings);

        public GameMode GetGameMode() => GameMode;
        public T GetGameMode<T>() where T : GameMode => GetGameMode() as T;

        #endregion

        #region Player

        [HideInInspector] public List<GameObject> PlayerArray;
        [SerializeField] private List<GameObject> PlayerStarts;

        #endregion
    }

    public sealed partial class World : MonoBehaviour
    {
        private partial void Awake()
        {
            ProjectStatics.World = this;

            ProjectSettings Settings = Resources.Load<ProjectSettings>(ProjectStatics.ProjectSettingsAssetPath);
            if (Settings == null)
            {
                Debug.LogError($"Resources/{ProjectStatics.ProjectSettingsAssetPath} file is not valid, create proper asset using Tool/GameFramework/InitProject");
                return;
            }

            if (GameInstance == null)
            {
                GameInstance = (GameInstance)Activator.CreateInstance(Settings.GameInstanceClass.Type);
            }

            InitGameMode(Settings.DefaultGameModeSettings);
        }

        #region GameMode

        private partial void InitGameMode(GameModeSettings GameModeSettings)
        {
            if (GameModeSettingsOverride == null)
            {
                if (GameModeSettings.GameModeClass.Type == null)
                {
                    Debug.LogError("DefaultGameModeClass is not valid");
                    return;
                }

                GameMode = gameObject.AddComponent(GameModeSettings.GameModeClass.Type) as GameMode;
            }
            else
            {
                GameModeSettings = GameModeSettingsOverride;
                GameMode = gameObject.AddComponent(GameModeSettingsOverride.GameModeClass.Type) as GameMode;
            }

            GameMode.InitGameMode(GameModeSettings);
            GameMode.CreatePlayer(PlayerStarts);
        }

        #endregion

        #region Player

        

        #endregion
    }
}