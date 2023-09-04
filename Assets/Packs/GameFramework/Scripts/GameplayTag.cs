using System;

using UnityEngine;

namespace GameFramework.System
{
    [Serializable]
    public class GameplayTag : object, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string TagName;
        public string GetTagName() => TagName;

        [SerializeField, HideInInspector]
        private int TagId = -1;
        public int GetTagId() => TagId;

        /// <summary>
        /// Use GameplayTags.GetTag instead.
        /// </summary>
        public GameplayTag(string InTagName, int InTagId)
        {
            TagName = InTagName;
            TagId = InTagId;
        }

        /// <summary>
        /// "A.B".MatchesTag("A") = True
        /// </summary>
        public bool MatchesTag(GameplayTag TagToCheck)
        {
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

            for (int i = 0; i < InTags2.Length; i++)
            {
                for (int j = 0; j < InTags1.Length; j++)
                {
                    if (InTags1[j].MatchesTag(InTags2[i])) return true;
                }
            }
            return false;
        }

        public static bool HasAll(GameplayTag[] InTags1, GameplayTag[] InTags2)
        {
            if(InTags2.Length < 1) return true;

            for (int i = 0; i < InTags2.Length; i++)
            {
                for (int j = 0; j < InTags1.Length; j++)
                {
                    if (!InTags1[j].MatchesTag(InTags2[i])) return false;
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
            if (X is null || Y is null) return false;
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

            GameplayTag Tag;           
            if (TagId != -1)
            {
                Tag = GameplayTagsManager.GetTag(TagId);
                if(Tag != null)
                {
                    if (Tag.TagName == TagName) return;
                }
            }
            
            Tag = GameplayTagsManager.GetTag(TagName);
            if (Tag == null) return;
            
            TagId = Tag.TagId;
        }
    }
}