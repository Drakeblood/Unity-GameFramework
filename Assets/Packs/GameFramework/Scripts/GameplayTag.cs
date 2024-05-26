using System;
using System.Collections.Generic;

using UnityEngine;

namespace GameFramework.System
{
    [Serializable]
    public class GameplayTag : object, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string tagName;
        public string TagName { get => tagName; }

        [SerializeField, HideInInspector]
        private int tagId = -1;
        public int TagId { get => tagId; }

        /// <summary>
        /// Use GameplayTags.GetTag instead.
        /// </summary>
        public GameplayTag(string stateName, int stateId)
        {
            this.tagName = stateName;
            this.tagId = stateId;
        }

        /// <summary>
        /// "A.B".MatchesTag("A") = True
        /// </summary>
        public bool MatchesTag(GameplayTag tagToCheck)
        {
            GameplayTag[] separatedTag = GameplayTagsManager.GetSeparatedTag(this);
            GameplayTag[] separatedTagToCheck = GameplayTagsManager.GetSeparatedTag(tagToCheck);

            if (separatedTag.Length == separatedTagToCheck.Length) return this == tagToCheck;
            if (separatedTagToCheck.Length > separatedTag.Length) return false;

            for (int i = 0; i < separatedTag.Length && i < separatedTagToCheck.Length; i++)
            {
                if (separatedTag[i] != separatedTagToCheck[i]) return false;
            }

            if (separatedTagToCheck.Length == 0 && separatedTag.Length > 0) return separatedTag[0] == tagToCheck;
            
            return true;
        }

        public static bool HasAny(GameplayTag[] tags1, GameplayTag[] tags2)
        {
            if (tags2.Length < 1) return false;

            for (int i = 0; i < tags2.Length; i++)
            {
                for (int j = 0; j < tags1.Length; j++)
                {
                    if (tags1[j].MatchesTag(tags2[i])) return true;
                }
            }
            return false;
        }

        public static bool HasAll(GameplayTag[] tags1, GameplayTag[] tags2)
        {
            if(tags2.Length < 1) return true;

            for (int i = 0; i < tags2.Length; i++)
            {
                for (int j = 0; j < tags1.Length; j++)
                {
                    if (!tags1[j].MatchesTag(tags2[i])) return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            GameplayTag other = obj as GameplayTag;
            if (other == null) return base.Equals(obj);

            return tagId == other.tagId;
        }

        public static bool Equals(GameplayTag x, GameplayTag y)
        {
            if ((object)x == (object)y) return true;
            if (x is null || y is null) return false;
            return x.Equals(y);
        }

        public static bool operator ==(GameplayTag x, GameplayTag y) => Equals(x, y);
        public static bool operator !=(GameplayTag x, GameplayTag y) => !Equals(x, y);

        public override int GetHashCode() => tagId;
        public override string ToString() => tagName;

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if(tagName == null || tagName == "") return;

            GameplayTag tag;           
            if (TagId != -1)
            {
                tag = GameplayTagsManager.GetTag(tagId);
                if(tag != null)
                {
                    if (tag.tagName == tagName) return;
                }
            }
            
            tag = GameplayTagsManager.GetTag(tagName);
            if (tag == null) return;
            
            tagId = tag.tagId;
        }
    }

    public delegate void GameplayTagDelegate(GameplayTag tag, int newCount);

    public class GameplayTagCountContainer
    {
        public Dictionary<GameplayTag, int> GameplayTagCountArray = new();
        public List<GameplayTag> ExplicitGameplayTags = new();

        protected Dictionary<GameplayTag, GameplayTagDelegate> GameplayTagEventArray = new();
        
        public bool UpdateTagCount(GameplayTag tag, int countDelta)
        {
            if (countDelta == 0) return false;

            if (GameplayTagCountArray.ContainsKey(tag))
            {
                GameplayTagCountArray[tag] = Math.Max(GameplayTagCountArray[tag] + countDelta, 0);
            }
            else
            {
                GameplayTagCountArray.Add(tag, Math.Max(countDelta, 0));
            }

            if (ExplicitGameplayTags.Contains(tag))
            {
                if (GameplayTagCountArray[tag] == 0)
                {
                    ExplicitGameplayTags.Remove(tag);
                }
            }
            else if (GameplayTagCountArray[tag] != 0)
            {
                ExplicitGameplayTags.Add(tag);
            }

            if (GameplayTagEventArray.ContainsKey(tag))
            {
                List<GameplayTagDelegate> invalidDelegates = null;
                Delegate[] delegates = GameplayTagEventArray[tag].GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    if (delegates[i] is not GameplayTagDelegate tagDelegate) continue;

                    if (tagDelegate.Target is MonoBehaviour behaviour)
                    {
                        if (behaviour == null)
                        {
                            invalidDelegates ??= new List<GameplayTagDelegate>();
                            invalidDelegates.Add(tagDelegate);
                            continue;
                        }
                    }

                    tagDelegate.Invoke(tag, GameplayTagCountArray[tag]);
                }

                if (invalidDelegates == null) return true;

                for (int i = 0; i < invalidDelegates.Count; i++)
                {
                    GameplayTagEventArray[tag] -= invalidDelegates[i];
                }
            }

            return true;
        }

        public void RegisterGameplayTagEvent(GameplayTag tag, GameplayTagDelegate tagDelegate)
        {
            if (!GameplayTagEventArray.ContainsKey(tag))
            {
                GameplayTagEventArray.Add(tag, tagDelegate);
                return;
            }

            GameplayTagEventArray[tag] += tagDelegate;
        }
    }
}