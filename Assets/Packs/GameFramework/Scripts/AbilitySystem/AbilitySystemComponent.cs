using System;
using System.Collections.Generic;

using UnityEngine;

using SolidUtilities.UnityEngineInternals;

using GameFramework.System;

namespace GameFramework.AbilitySystem
{
    public partial class AbilitySystemComponent : MonoBehaviour
    {
        [SerializeField] 
        private List<GameplayAbilityData> StartupAbilities = new List<GameplayAbilityData>();
        protected List<GameplayAbility> ActivatableAbilities = new List<GameplayAbility>();

        protected Dictionary<GameplayTag, int> GameplayTagCountArray = new Dictionary<GameplayTag, int>();
        protected List<GameplayTag> ExplicitGameplayTags = new List<GameplayTag>();
        protected Dictionary<GameplayTag, GameplayTagDelegate> GameplayTagEventArray = new Dictionary<GameplayTag, GameplayTagDelegate>();

        public delegate void GameplayTagDelegate(GameplayTag Tag, int NewCount);

        private partial void Awake();

        #region Abilities

        public partial void GiveAbility(GameplayAbilityData AbilityData);
        public partial bool TryActivateAbility(Type AbilityClass);

        #endregion

        #region GameplayTags

        public GameplayTag[] GetExplicitGameplayTags() => ExplicitGameplayTags.ToArray();
        public partial void UpdateTags(GameplayTag Tag, int CountDelta);
        public partial void RegisterGameplayTagEvent(GameplayTag InTag, GameplayTagDelegate InDelegate);
        
        #endregion
    }

    public partial class AbilitySystemComponent : MonoBehaviour
    {
        private partial void Awake()
        {
            for (int i = 0; i < StartupAbilities.Count; i++)
            {
                GiveAbility(StartupAbilities[i]);
            }
        }

        #region Abilities

        public partial void GiveAbility(GameplayAbilityData AbilityData)
        {
            if (AbilityData == null) { Debug.LogError("AbilityData is not vaild"); return; }

            GameplayAbility GameplayAbility = AbilityData.GetGameplayAbility().ShallowCopy();
            ActivatableAbilities.Add(GameplayAbility);
            GameplayAbility.OnGiveAbility(this);
        }

        public partial bool TryActivateAbility(Type AbilityClass)
        {
            if (AbilityClass == null) { Debug.LogError("AbilityClass is not valid"); return false; }

            foreach (var Ability in ActivatableAbilities)
            {
                if(Ability.AbilityData == null)
                {
                    Debug.LogError("AbilityData is not valid. " + Ability.ToString());
                    continue;
                }

                if (Ability.AbilityData.AbilityClass.Type == AbilityClass)
                {
                    if (!Ability.CanActivateAbility()) return false;

                    Ability.ActivateAbility();
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region GameplayTags

        public partial void RegisterGameplayTagEvent(GameplayTag InTag, GameplayTagDelegate InDelegate)
        {
            if(!GameplayTagEventArray.ContainsKey(InTag))
            {
                GameplayTagEventArray.Add(InTag, InDelegate);
                return;
            }

            GameplayTagEventArray[InTag] += InDelegate;
        }

        public partial void UpdateTags(GameplayTag Tag, int CountDelta)
        {
            if (CountDelta != 0)
            {
                if(GameplayTagCountArray.ContainsKey(Tag))
                {
                    GameplayTagCountArray[Tag] = Math.Max(GameplayTagCountArray[Tag] + CountDelta, 0);
                }
                else
                {
                    GameplayTagCountArray.Add(Tag, Math.Max(CountDelta, 0));
                }

                if(ExplicitGameplayTags.Contains(Tag))
                {
                    if (GameplayTagCountArray[Tag] == 0)
                    {
                        ExplicitGameplayTags.Remove(Tag);
                    }
                }
                else
                {
                    if (GameplayTagCountArray[Tag] != 0)
                    {
                        ExplicitGameplayTags.Add(Tag);
                    }
                }

                if (GameplayTagEventArray.ContainsKey(Tag))
                {
                    List<GameplayTagDelegate> InvalidDelegates = null;
                    foreach (GameplayTagDelegate TagDelegate in GameplayTagEventArray[Tag].GetInvocationList())
                    {
                        if (TagDelegate.Target is MonoBehaviour)
                        {
                            if ((MonoBehaviour)TagDelegate.Target == null)
                            {
                                if (InvalidDelegates == null) InvalidDelegates = new List<GameplayTagDelegate>();
                                InvalidDelegates.Add(TagDelegate);
                                continue;
                            }
                        }

                        TagDelegate.Invoke(Tag, GameplayTagCountArray[Tag]);
                    }

                    if (InvalidDelegates == null) return;
                    foreach (GameplayTagDelegate InvalidDelegate in InvalidDelegates)
                    {
                        GameplayTagEventArray[Tag] -= InvalidDelegate;
                    }
                }
            }
        }

        #endregion
    }
}