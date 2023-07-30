using System.Collections.Generic;

using UnityEngine;

namespace GameFramework.System
{
    public class GameMode : MonoBehaviour
    {
        private GameModeSettings GameModeSettings = null;

        public virtual void InitGameMode(GameModeSettings InGameModeSettings)
        {
            GameModeSettings = InGameModeSettings;
        }

        public virtual void CreatePlayer(List<GameObject> PlayerStarts)
        {
            if (World.Instance == null) return;
            
            if (GameModeSettings.PlayerPrefab == null) return;
            if (PlayerStarts.Count == 0)
            {
                Debug.LogError("Add player start to scene.");
                return;
            }
            
            int Index = PlayerStarts.Count < World.Instance.PlayerArray.Count ? World.Instance.PlayerArray.Count : 0;

            GameObject PlayerStart = PlayerStarts[Index];
            GameObject NewPlayer = Instantiate(GameModeSettings.PlayerPrefab, PlayerStart.transform.position, PlayerStart.transform.rotation);

            World.Instance.PlayerArray.Add(NewPlayer);
        }
    }
}