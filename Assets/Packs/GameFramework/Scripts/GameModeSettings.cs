using UnityEngine;

using TypeReferences;

namespace GameFramework.System
{
    [CreateAssetMenu(menuName = "GameFramework/GameModeSettings")]
    public class GameModeSettings : ScriptableObject
    {
        [Inherits(typeof(GameMode), IncludeBaseType = true)]
        public TypeReference GameModeClass = new TypeReference(typeof(GameMode));

        public GameObject PlayerPrefab;
    }
}