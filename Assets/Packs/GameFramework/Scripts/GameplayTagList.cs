using UnityEngine;

namespace GameFramework.System
{
    public class GameplayTagList : ScriptableObject
    {
        [SerializeField] private string[] Tags;
        public string[] GetTags() => Tags;

        public void Init()
        {
            GameplayTagsManager.Init(ref Tags);
        }
    }
}