using UnityEngine;

namespace GameFramework.System
{
    public static class ProjectStatics
    {
        public static readonly string ProjectSettingsAssetPath = "GameFramework/ProjectSettings";
        public static readonly string GameModeSettingsAssetPath = "GameFramework/GameModeSettings";
        public static readonly string GameplayTagsAssetPath = "GameFramework/GameplayTags";
        
        public static GameObject GetPlayer(int index = 0)
        {
            if (World.Instance != null)
            {
                if (World.Instance.PlayerArray.Count > index)
                {
                    return World.Instance.PlayerArray[index];
                }
            }
            return null;
        }
    }
}