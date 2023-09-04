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
        private List<GameplayAbilityData> StartupAbilities = new();
        protected List<GameplayAbility> ActivatableAbilities = new();

        protected Dictionary<GameplayTag, int> GameplayTagCountArray = new();
        protected List<GameplayTag> ExplicitGameplayTags = new();
        protected Dictionary<GameplayTag, GameplayTagDelegate> GameplayTagEventArray = new();

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

            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (ActivatableAbilities[i].AbilityData == null)
                {
                    Debug.LogError("AbilityData is not valid. " + ActivatableAbilities[i].ToString());
                    continue;
                }

                if (ActivatableAbilities[i].AbilityData.AbilityClass.Type == AbilityClass)
                {
                    if (!ActivatableAbilities[i].CanActivateAbility()) return false;

                    ActivatableAbilities[i].ActivateAbility();
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
                    Delegate[] Delegates = GameplayTagEventArray[Tag].GetInvocationList();
                    for (int i = 0; i < Delegates.Length; i++)
                    {
                        if (Delegates[i] is not GameplayTagDelegate TagDelegate) continue;
                        
                        if (TagDelegate.Target is MonoBehaviour Behaviour)
                        {
                            if (Behaviour == null)
                            {
                                InvalidDelegates ??= new List<GameplayTagDelegate>();
                                InvalidDelegates.Add(TagDelegate);
                                continue;
                            }
                        }

                        TagDelegate.Invoke(Tag, GameplayTagCountArray[Tag]);
                    }

                    if (InvalidDelegates == null) return;

                    for (int i = 0; i < InvalidDelegates.Count; i++)
                    {
                        GameplayTagEventArray[Tag] -= InvalidDelegates[i];
                    }
                }
            }
        }

        #endregion
    }
}