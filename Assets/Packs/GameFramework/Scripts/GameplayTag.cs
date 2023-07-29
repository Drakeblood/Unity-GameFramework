using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework.System
{
    [Serializable]
    public class GameplayTag : object, ISerializationCallbackReceiver
    {
        [SerializeField] private string TagName;
        public string GetTagName() => TagName;

        private int TagId = -1;
        public int GetTagId() => TagId;

        /// <summary>
        /// Use RequestTag instead.
        /// </summary>
        public GameplayTag(string InTagName, int InTagId)
        {
            TagName = InTagName;
            TagId = InTagId;
        }

        public static GameplayTag RequestTag(string InTagName)
        {
            if(InTagName == null || InTagName == "") return null;
            return GameplayTagsManager.GetTag(InTagName);
        }

        public bool MatchesTag(GameplayTag TagToCheck)
        {
            //"A.B".MatchesTag("A") = True
            GameplayTag[] SeparatedTag = GameplayTagsManager.GetSeparatedTag(this);
            GameplayTag[] SeparatedTagToCheck = GameplayTagsManager.GetSeparatedTag(TagToCheck);

            if (SeparatedTag.Length == SeparatedTagToCheck.Length) return this == TagToCheck;
            if (SeparatedTagToCheck.Length > SeparatedTag.Length) return false;

            for (int i = 0; i < SeparatedTag.Length && i < SeparatedTagToCheck.Length; i++)
            {
                if (SeparatedTag[i] != SeparatedTagToCheck[i]) return false;
            }

            if (SeparatedTagToCheck.Length == 0 && SeparatedTag.Length > 0) return SeparatedTag[0] == TagToCheck;
            
            return true;
        }

        public static bool HasAny(GameplayTag[] InTags1, GameplayTag[] InTags2)
        {
            if (InTags2.Length < 1) return false;

            foreach (var Tag2 in InTags2)
            {
                foreach (var Tag1 in InTags1)
                {
                    if(Tag1.MatchesTag(Tag2)) return true;
                }
            }
            return false;
        }

        public static bool HasAll(GameplayTag[] InTags1, GameplayTag[] InTags2)
        {
            if(InTags2.Length < 1) return true;

            foreach (var Tag2 in InTags2)
            {
                foreach (var Tag1 in InTags1)
                {
                    if (!Tag1.MatchesTag(Tag2)) return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            GameplayTag other = obj as GameplayTag;
            if (other == null) return base.Equals(obj);

            return TagId == other.TagId;
        }

        public static bool Equals(GameplayTag X, GameplayTag Y)
        {
            if ((object)X == (object)Y) return true;
            if ((object)X == null || (object)Y == null) return false;
            return X.Equals(Y);
        }

        public static bool operator ==(GameplayTag X, GameplayTag Y) => Equals(X, Y);
        public static bool operator !=(GameplayTag X, GameplayTag Y) => !Equals(X, Y);

        public override int GetHashCode() => TagId;
        public override string ToString() => TagName;

        public void OnBeforeSerialize()
        {
            
        }

        public void OnAfterDeserialize()
        {
            if(TagName == null || TagName == "") return;
            
            GameplayTag Tag = GameplayTagsManager.GetTag(TagName);
            if (Tag == null) return;
            
            TagId = Tag.TagId;
        }
    }

    public static class GameplayTagsManager
    {
        private static GameplayTag[] Tags = new GameplayTag[0];
        private static Tuple<GameplayTag, GameplayTag[]>[] TagsWithSubTags = new Tuple<GameplayTag, GameplayTag[]>[0];

        public static GameplayTag GetTag(string InTagName)
        {
            for (int i = 0; i < Tags.Length; i++)
            {
                if (Tags[i].GetTagName() == InTagName)
                {
                    return Tags[i];
                }
            }
            return null;
        }

        public static GameplayTag[] GetTags() => Tags;
        public static int GetTagsCount() => Tags.Length;

        public static GameplayTag[] GetSeparatedTag(GameplayTag InTag)
        {
            for (int i = 0; i < TagsWithSubTags.Length; i++)
            {
                if (TagsWithSubTags[i].Item1.GetTagName() == InTag.GetTagName())
                {
                    return TagsWithSubTags[i].Item2;
                }
            }
            return null;
        }
        
        public static void Init(ref string[] InTags)
        {
            Tags = new GameplayTag[InTags.Length];
            TagsWithSubTags = new Tuple<GameplayTag, GameplayTag[]>[InTags.Length];

            for (int i = 0; i < InTags.Length; i++)
            {
                List<GameplayTag> ParentTags = new List<GameplayTag>();
                {
                    string Tag = InTags[i];
                    for (int Index = Tag.LastIndexOf('.'); Index != -1; Index = Tag.LastIndexOf('.'))
                    {
                        Tag = Tag.Substring(0, Index);
                        ParentTags.Add(GameplayTag.RequestTag(Tag));
                    }
                }

                GameplayTag[] ParentTagsArray = new GameplayTag[ParentTags.Count];
                {
                    int n = 0;
                    for (int j = ParentTags.Count - 1; j >= 0; j--)
                    {
                        ParentTagsArray[n++] = ParentTags[j];
                    }
                }

                Tags[i] = new GameplayTag(InTags[i], i);
                TagsWithSubTags[i] = new Tuple<GameplayTag, GameplayTag[]>(Tags[i], ParentTagsArray);
            }
        }
    }
}