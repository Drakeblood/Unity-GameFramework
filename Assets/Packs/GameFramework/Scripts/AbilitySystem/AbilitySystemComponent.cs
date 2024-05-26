using System;
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

using SolidUtilities.UnityEngineInternals;

using GameFramework.System;

namespace GameFramework.AbilitySystem
{
    public partial class AbilitySystemComponent : MonoBehaviour
    {
        [SerializeField] 
        private List<GameplayAbilityDefinition> startupAbilities = new();
        protected List<GameplayAbility> ActivatableAbilities = new();

        protected GameplayTagCountContainer GameplayTagCountContainer = new();
        protected GameplayTagCountContainer BlockedAbilityTags = new();

        private partial void Awake();

        #region Abilities

        public partial void GiveAbility(GameplayAbilityDefinition abilityDefinition, object sourceObject = null);
        public partial void ClearAbility(GameplayAbilityDefinition abilityDefinition);

        public partial bool TryActivateAbility(Type abilityClass);
        public partial void CancelAbilitiesWithTags(GameplayTag[] tags);

        #endregion

        #region GameplayTags

        public GameplayTag[] GetExplicitGameplayTags() => GameplayTagCountContainer.ExplicitGameplayTags.ToArray();
        public GameplayTag[] GetBlockedAbilityTags() => BlockedAbilityTags.ExplicitGameplayTags.ToArray();
        public partial void UpdateTagMap(GameplayTag tag, int countDelta);
        public partial void RegisterGameplayTagEvent(GameplayTag tag, GameplayTagDelegate tagDelegate);
        public partial void UpdateBlockedAbilityTags(GameplayTag tag, int countDelta);
        
        #endregion

        #region Input

        public partial void AbilityInputPressed(CallbackContext callbackContext);
        public partial void AbilityInputReleased(CallbackContext callbackContext);

        #endregion
    }

    public partial class AbilitySystemComponent : MonoBehaviour
    {
        private partial void Awake()
        {
            for (int i = 0; i < startupAbilities.Count; i++)
            {
                GiveAbility(startupAbilities[i]);
            }
        }

        #region Abilities

        public partial void GiveAbility(GameplayAbilityDefinition abilityDefinition, object sourceObject)
        {
            if (abilityDefinition == null) { Debug.LogError("AbilityDefinition is not vaild"); return; }

            GameplayAbility ability = abilityDefinition.GameplayAbility.ShallowCopy();
            ActivatableAbilities.Add(ability);

            if (ability.AbilityDefinition.InputActionReference != null)
            {
                ability.AbilityDefinition.InputActionReference.action.started += AbilityInputPressed;
                ability.AbilityDefinition.InputActionReference.action.canceled += AbilityInputReleased;
                ability.AbilityDefinition.InputActionReference.action.Enable();
            }

            ability.SetupAbility(this, sourceObject);
            ability.OnGiveAbility();
        }

        public partial void ClearAbility(GameplayAbilityDefinition abilityDefinition)
        {
            if (abilityDefinition == null) { Debug.LogError("AbilityDefinition is not vaild"); return; }

            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (ActivatableAbilities[i].AbilityDefinition == abilityDefinition)
                {
                    if (ActivatableAbilities[i].IsActive)
                    {
                        ActivatableAbilities[i].OnAbilityEnded += (bool wasCanceled) =>
                        {
                            ClearAbility(abilityDefinition);
                        };
                        return;
                    }

                    if (ActivatableAbilities[i].AbilityDefinition.InputActionReference != null)
                    {
                        ActivatableAbilities[i].AbilityDefinition.InputActionReference.action.started -= AbilityInputPressed;
                        ActivatableAbilities[i].AbilityDefinition.InputActionReference.action.canceled -= AbilityInputReleased;
                        ActivatableAbilities[i].AbilityDefinition.InputActionReference.action.Disable();
                    }

                    ActivatableAbilities.RemoveAt(i);
                    return;
                }
            }
        }

        public partial bool TryActivateAbility(Type abilityClass)
        {
            if (abilityClass == null) { Debug.LogError("AbilityClass is not valid"); return false; }

            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (ActivatableAbilities[i].AbilityDefinition == null)
                {
                    Debug.LogError("AbilityData is not valid. " + ActivatableAbilities[i].ToString());
                    continue;
                }

                if (ActivatableAbilities[i].AbilityDefinition.AbilityClass.Type == abilityClass)
                {
                    if (!ActivatableAbilities[i].CanActivateAbility()) return false;

                    ActivatableAbilities[i].ActivateAbility();
                    return true;
                }
            }
            return false;
        }

        public partial void CancelAbilitiesWithTags(GameplayTag[] tags)
        {
            if (tags.Length == 0) return;

            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (!ActivatableAbilities[i].IsActive) continue;

                if (GameplayTag.HasAny(ActivatableAbilities[i].AbilityDefinition.AbilityTags, tags))
                {
                    ActivatableAbilities[i].EndAbility(true);
                }
            }
        }

        #endregion

        #region GameplayTags

        public partial void RegisterGameplayTagEvent(GameplayTag tag, GameplayTagDelegate tagDelegate)
        {
            GameplayTagCountContainer.RegisterGameplayTagEvent(tag, tagDelegate);
        }

        public partial void UpdateTagMap(GameplayTag tag, int countDelta)
        {
            if (GameplayTagCountContainer.UpdateTagCount(tag, countDelta))
            {
                //OnTagUpdated
            }
        }

        public partial void UpdateBlockedAbilityTags(GameplayTag tag, int countDelta)
        {
            BlockedAbilityTags.UpdateTagCount(tag, countDelta);
        }

        #endregion

        #region Input

        public partial void AbilityInputPressed(CallbackContext callbackContext)
        {
            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (ActivatableAbilities[i].AbilityDefinition.InputActionReference.action == callbackContext.action)
                {
                    ActivatableAbilities[i].IsInputPressed = true;

                    if (ActivatableAbilities[i].IsActive)
                    {
                        ActivatableAbilities[i].InputPressed();
                    }
                    else
                    {
                        TryActivateAbility(ActivatableAbilities[i].GetType());
                    }
                }
            }
        }

        public partial void AbilityInputReleased(CallbackContext callbackContext)
        {
            for (int i = 0; i < ActivatableAbilities.Count; i++)
            {
                if (ActivatableAbilities[i].AbilityDefinition.InputActionReference.action == callbackContext.action)
                {
                    ActivatableAbilities[i].IsInputPressed = false;

                    if (ActivatableAbilities[i].IsActive)
                    {
                        ActivatableAbilities[i].InputReleased();
                    }
                }
            }
        }

        #endregion
    }
}