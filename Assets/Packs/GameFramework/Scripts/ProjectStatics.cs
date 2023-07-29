using UnityEngine;

namespace GameFramework.System
{
    public static class ProjectStatics
    {
        public static readonly string ProjectSettingsAssetPath = "GameFramework/ProjectSettings";
        public static readonly string GameModeSettingsAssetPath = "GameFramework/GameModeSettings";
        public static readonly string ProjectTagsAssetPath = "GameFramework/ProjectTags";

        public static World World;

        public static GameObject GetPlayer(int Index = 0)
        {
            if (World != null)
            {
                if (World.PlayerArray.Count > Index)
                {
                    return World.PlayerArray[Index];
                }
            }
            return null;
        }
    }
}