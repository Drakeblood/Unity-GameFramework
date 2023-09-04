using System;
using System.Collections;

using UnityEngine;

using GameFramework.System;

namespace GameFramework.AbilitySystem
{
    [Serializable]
    public partial class GameplayAbility : object
    {
        [SerializeField, HideInInspector]
        public GameplayAbilityData AbilityData;

        public AbilitySystemComponent OwningAbilitySystemComponent { get; private set; }

        private bool IsActive = false;

        public delegate void AbilityEnded(bool WasCanceled);
        public event AbilityEnded OnAbilityEnded;

        public partial void StartCoroutine(IEnumerator Routine);

        public virtual partial void OnGiveAbility(AbilitySystemComponent InAbilitySystemComponent);
        public virtual partial bool CanActivateAbility();
        public virtual partial void ActivateAbility();
        public virtual partial void EndAbility(bool WasCanceled);
    }

    public partial class GameplayAbility : object
    {
        public partial void StartCoroutine(IEnumerator Routine)
        {
            if (OwningAbilitySystemComponent == null) { Debug.LogError("OwningAbilitySystemComponent is not valid"); return; }

            OwningAbilitySystemComponent.StartCoroutine(Routine);
        }

        public virtual partial void OnGiveAbility(AbilitySystemComponent InAbilitySystemComponent)
        {
            OwningAbilitySystemComponent = InAbilitySystemComponent;
        }

        public virtual partial bool CanActivateAbility()
        {
            if (IsActive) return false;
            if (OwningAbilitySystemComponent == null) { Debug.LogError("OwningAbilitySystemComponent is not valid"); return false; }

            if (AbilityData.ActivationBlockedTags.Length > 0 || AbilityData.ActivationRequiredTags.Length > 0)
            {
                GameplayTag[] AbilitySystemComponentTags = OwningAbilitySystemComponent.GetExplicitGameplayTags();

                if (GameplayTag.HasAny(AbilitySystemComponentTags, AbilityData.ActivationBlockedTags)) return false;
                if (!GameplayTag.HasAll(AbilitySystemComponentTags, AbilityData.ActivationRequiredTags)) return false;
            }

            return true;
        }

        public virtual partial void ActivateAbility()
        {
            if (OwningAbilitySystemComponent == null) { Debug.LogError("OwningAbilitySystemComponent is not valid"); return; }

            IsActive = true;

            for (int i = 0; i < AbilityData.ActivationOwnedTags.Length; i++)
            {
                OwningAbilitySystemComponent.UpdateTags(AbilityData.ActivationOwnedTags[i], 1);
            }
        }

        public virtual partial void EndAbility(bool WasCanceled)
        {
            if (OwningAbilitySystemComponent == null) { Debug.LogError("OwningAbilitySystemComponent is not valid"); return; }

            IsActive = false;

            for (int i = 0; i < AbilityData.ActivationOwnedTags.Length; i++)
            {
                OwningAbilitySystemComponent.UpdateTags(AbilityData.ActivationOwnedTags[i], -1);
            }
            OnAbilityEnded?.Invoke(WasCanceled);
        }
    }
}