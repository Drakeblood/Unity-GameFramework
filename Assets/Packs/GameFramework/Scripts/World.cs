using System;

using UnityEngine;

using System.Collections.Generic;

namespace GameFramework.System
{
    public sealed partial class World : MonoBehaviour
    {
        public static World Instance { get; private set; }

        private static GameInstance gameInstance;
        public T GetGameInstance<T>() where T : GameMode => gameInstance as T;

        private partial void Awake();

        #region GameMode

        [SerializeField] 
        private GameModeSettings gameModeSettingsOverride;

        private GameMode gameMode;
        public T GetGameMode<T>() where T : GameMode => gameMode as T;

        private partial void InitGameMode(GameModeSettings gameModeSettings);

        #endregion

        #region Player

        [HideInInspector]
        public List<GameObject> PlayerArray;

        [SerializeField]
        private List<GameObject> playerStarts;

        #endregion
    }

    public sealed partial class World : MonoBehaviour
    {
        private partial void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            ProjectSettings settings = Resources.Load<ProjectSettings>(ProjectStatics.ProjectSettingsAssetPath);
            if (settings == null)
            {
                Debug.LogError($"Resources/{ProjectStatics.ProjectSettingsAssetPath} file is not valid, create proper asset using Tool/GameFramework/InitProject");
                return;
            }

            gameInstance ??= (GameInstance)Activator.CreateInstance(settings.GameInstanceClass.Type);

            InitGameMode(settings.DefaultGameModeSettings);
        }

        #region GameMode

        private partial void InitGameMode(GameModeSettings gameModeSettings)
        {
            if (gameModeSettingsOverride == null)
            {
                if (gameModeSettings.GameModeClass.Type == null)
                {
                    Debug.LogError("DefaultGameModeClass is not valid");
                    return;
                }

                gameMode = gameObject.AddComponent(gameModeSettings.GameModeClass.Type) as GameMode;
            }
            else
            {
                gameModeSettings = gameModeSettingsOverride;
                gameMode = gameObject.AddComponent(gameModeSettingsOverride.GameModeClass.Type) as GameMode;
            }

            gameMode.InitGameMode(gameModeSettings);
            gameMode.CreatePlayer(playerStarts);
        }

        #endregion

        #region Player

        

        #endregion
    }
}