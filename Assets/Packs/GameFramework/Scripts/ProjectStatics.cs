using UnityEngine;

namespace GameFramework.System
{
    public static class ProjectStatics
    {
        public static readonly string ProjectSettingsAssetPath = "GameFramework/ProjectSettings";
        public static readonly string GameModeSettingsAssetPath = "GameFramework/GameModeSettings";
        public static readonly string GameplayTagsAssetPath = "GameFramework/GameplayTags";
        
        public static GameObject GetPlayer(int Index = 0)
        {
            if (World.Instance != null)
            {
                if (World.Instance.PlayerArray.Count > Index)
                {
                    return World.Instance.PlayerArray[Index];
                }
            }
            return null;
        }
    }
}