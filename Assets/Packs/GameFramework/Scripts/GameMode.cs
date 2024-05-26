using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Assertions;

namespace GameFramework.System
{
    public class GameMode : MonoBehaviour
    {
        private GameModeSettings gameModeSettings = null;

        public virtual void InitGameMode(GameModeSettings gameModeSettings)
        {
            this.gameModeSettings = gameModeSettings;
        }

        public virtual void CreatePlayer(List<GameObject> playerStarts)
        {
            Assert.IsNotNull(World.Instance);
            if (gameModeSettings.PlayerPrefab == null) { Debug.LogError("PlayerPrefab is not valid. Set it in GameModeSettings"); return; }
            if (playerStarts.Count == 0) { Debug.LogError("Add player start to scene."); return; }
            
            int Index = playerStarts.Count < World.Instance.PlayerArray.Count ? World.Instance.PlayerArray.Count : 0;

            GameObject PlayerStart = playerStarts[Index];
            GameObject NewPlayer = Instantiate(gameModeSettings.PlayerPrefab, PlayerStart.transform.position, PlayerStart.transform.rotation);

            World.Instance.PlayerArray.Add(NewPlayer);
        }
    }
}