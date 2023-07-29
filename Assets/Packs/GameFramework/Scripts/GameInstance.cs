using UnityEngine;

namespace GameFramework.System
{
    public class GameInstance : object
    {
        public GameInstance()
        {
            GameplayTagList TagList = Resources.Load<GameplayTagList>(ProjectStatics.ProjectTagsAssetPath);
            if (TagList == null)
            {
                Debug.LogError($"Resources/{ProjectStatics.ProjectTagsAssetPath} file is not valid, create proper asset using Tool/GameFramework/InitProject");
                return;
            }

            TagList.Init();
        }
    }
}