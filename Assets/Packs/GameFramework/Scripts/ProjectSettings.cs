using UnityEngine;

using TypeReferences;

namespace GameFramework.System
{
    public class ProjectSettings : ScriptableObject
    {
        [Inherits(typeof(GameInstance), IncludeBaseType = true, ShowNoneElement = false)]
        public TypeReference GameInstanceClass = new(typeof(GameInstance));

        public GameModeSettings DefaultGameModeSettings;
    }
}