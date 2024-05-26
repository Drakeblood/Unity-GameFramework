using System;
using System.Collections;

using UnityEngine;

using GameFramework.System;
using System.Collections.Generic;

namespace GameFramework.AbilitySystem
{
    [Serializable]
    public partial class GameplayAbility : object
    {
        [SerializeField, HideInInspector]
        public GameplayAbilityDefinition AbilityDefinition;

        public AbilitySystemComponent AbilitySystemComponent { get; private set; }

        public bool IsActive { get; private set; }
        public bool IsInputPressed { get; set; }
        public object SourceObject { get; private set; }

        private List<Coroutine> coroutines = new List<Coroutine>();

        public delegate void AbilityEnded(bool wasCanceled);
        public event AbilityEnded OnAbilityEnded;

        public partial void StartCoroutine(IEnumerator routine);
        public partial void SetupAbility(AbilitySystemComponent abilitySystemComponent, object sourceObject = null);

        public virtual void OnGiveAbility() { }
        public virtual partial bool CanActivateAbility();
        public virtual partial void ActivateAbility();
        public virtual partial void EndAbility(bool wasCanceled = false);

        public virtual void InputPressed() { }
        public virtual void InputReleased() { }
    }

    public partial class GameplayAbility : object
    {
        public partial void StartCoroutine(IEnumerator routine)
        {
            if (AbilitySystemComponent == null) { Debug.LogError("OwningAbilityManager is not valid"); return; }

            coroutines.Add(AbilitySystemComponent.StartCoroutine(routine));
        }

        public partial void SetupAbility(AbilitySystemComponent abilitySystemComponent, object sourceObject)
        {
            AbilitySystemComponent = abilitySystemComponent;
            SourceObject = sourceObject;
        }

        public virtual partial bool CanActivateAbility()
        {
            if (IsActive) return false;
            if (AbilityDefinition == null)
            {
                Debug.LogError("AbilityDefinition is not valid");
                return false;
            }
            if (AbilitySystemComponent == null)
            {
                Debug.LogError("OwningAbilityManager is not valid");
                return false;
            }

            if (GameplayTag.HasAny(AbilitySystemComponent.GetBlockedAbilityTags(),
                AbilityDefinition.AbilityTags))
            {
                return false;
            }

            if (AbilityDefinition.ActivationBlockedTags.Length > 0 || AbilityDefinition.ActivationRequiredTags.Length > 0)
            {
                GameplayTag[] AbilityManagerStates = AbilitySystemComponent.GetExplicitGameplayTags();

                if (GameplayTag.HasAny(AbilityManagerStates, AbilityDefinition.ActivationBlockedTags))
                {
                    return false;
                }
                if (!GameplayTag.HasAll(AbilityManagerStates, AbilityDefinition.ActivationRequiredTags))
                {
                    return false;
                }
            }

            return true;
        }

        public virtual partial void ActivateAbility()
        {
            if (AbilitySystemComponent == null) { Debug.LogError("OwningAbilityManager is not valid"); return; }

            for (int i = 0; i < AbilityDefinition.ActivationOwnedTags.Length; i++)
            {
                AbilitySystemComponent.UpdateTagMap(AbilityDefinition.ActivationOwnedTags[i], 1);
            }

            for (int i = 0; i < AbilityDefinition.BlockAbilitiesWithTag.Length; i++)
            {
                AbilitySystemComponent.UpdateBlockedAbilityTags(AbilityDefinition.BlockAbilitiesWithTag[i], 1);
            }

            AbilitySystemComponent.CancelAbilitiesWithTags(AbilityDefinition.CancelAbilitiesWithTag);

            IsActive = true;
        }

        public virtual partial void EndAbility(bool wasCanceled)
        {
            if (AbilitySystemComponent == null) { Debug.LogError("OwningAbilityManager is not valid"); return; }

            IsActive = false;

            for (int i = 0; i < AbilityDefinition.ActivationOwnedTags.Length; i++)
            {
                AbilitySystemComponent.UpdateTagMap(AbilityDefinition.ActivationOwnedTags[i], -1);
            }

            for (int i = 0; i < AbilityDefinition.BlockAbilitiesWithTag.Length; i++)
            {
                AbilitySystemComponent.UpdateBlockedAbilityTags(AbilityDefinition.BlockAbilitiesWithTag[i], -1);
            }

            for (int i = 0; i < coroutines.Count; i++)
            {
                AbilitySystemComponent.StopCoroutine(coroutines[i]);
            }

            OnAbilityEnded?.Invoke(wasCanceled);
        }
    }
}