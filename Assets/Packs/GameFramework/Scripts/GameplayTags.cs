using System;
using System.Collections.Generic;

namespace GameFramework.System
{
    public static class GameplayTags
    {
        private static string[] TagsNames =
{
"Character,",
"Character.Movement,",
"Character.Movement.Falling,",
"Character.Movement.Walking,",
"Character.Status,",
"Character.Status.Death,",
"Character.Status.Death.Dead,Applied when character is dead",
"Character.Status.Death.Dying,Applied when character is started dying",
"Character.Status.Stunned,Applied when character is stunned",
};
        public static string[] GetTagsNames() => TagsNames;

        private static GameplayTag[] Tags = new GameplayTag[0];
        private static Tuple<GameplayTag, GameplayTag[]>[] TagsWithSubTags = new Tuple<GameplayTag, GameplayTag[]>[0];

        static GameplayTags()
        {
            for(int i = 0; i < TagsNames.Length; i++)
            {
                TagsNames[i] = TagsNames[i].Split(',')[0];
            }

            Tags = new GameplayTag[TagsNames.Length];
            TagsWithSubTags = new Tuple<GameplayTag, GameplayTag[]>[TagsNames.Length];

            for (int i = 0; i < TagsNames.Length; i++)
            {
                List<GameplayTag> ParentTags = new List<GameplayTag>();
                {
                    string Tag = TagsNames[i];
                    for (int Index = Tag.LastIndexOf('.'); Index != -1; Index = Tag.LastIndexOf('.'))
                    {
                        Tag = Tag.Substring(0, Index);
                        ParentTags.Add(GetTag(Tag));
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

                Tags[i] = new GameplayTag(TagsNames[i], i);
                TagsWithSubTags[i] = new Tuple<GameplayTag, GameplayTag[]>(Tags[i], ParentTagsArray);
            }
        }

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

        public static GameplayTag GetTag(int InId)
        {
            if (Tags.Length > InId)
            {
                return Tags[InId];
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
            return new GameplayTag[0];
        }
    }
}
