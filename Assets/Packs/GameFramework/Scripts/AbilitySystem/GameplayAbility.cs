using System;

using GameFramework.System;

namespace GameFramework.AbilitySystem
{
    [Serializable]
    public class GameplayAbility : object
    {
        private GameplayAbilityData Data;
        public GameplayAbilityData GetGameplayAbilityData() => Data;

        private AbilitySystemComponent OwningAbilitySystemComponent;

        private bool IsActive = false;

        public virtual void OnGiveAbility(AbilitySystemComponent InAbilitySystemComponent)
        {
            OwningAbilitySystemComponent = InAbilitySystemComponent;
        }

        public void SetAbilityData(GameplayAbilityData InData)
        {
            Data = InData;
        }

        public virtual bool CanActivateAbility()
        {
            if (IsActive) return false;

            if (Data.ActivationBlockedTags.Length > 0 || Data.ActivationRequiredTags.Length > 0)
            {
                GameplayTag[] AbilitySystemComponentTags = OwningAbilitySystemComponent.GetExplicitGameplayTags();

                if (GameplayTag.HasAny(AbilitySystemComponentTags, Data.ActivationBlockedTags)) return false;
                if (!GameplayTag.HasAll(AbilitySystemComponentTags, Data.ActivationRequiredTags)) return false;
            }

            return true;
        }

        public virtual void ActivateAbility()
        {
            IsActive = true;

            if (!OwningAbilitySystemComponent) return;

            foreach(GameplayTag Tag in Data.ActivationOwnedTags)
            {
                OwningAbilitySystemComponent.UpdateTags(Tag, 1);
            }
        }

        public virtual void EndAbility(bool WasCanceled)
        {
            IsActive = false;

            if (!OwningAbilitySystemComponent) return;

            foreach (GameplayTag Tag in Data.ActivationOwnedTags)
            {
                OwningAbilitySystemComponent.UpdateTags(Tag, -1);
            }
        }
    }
}