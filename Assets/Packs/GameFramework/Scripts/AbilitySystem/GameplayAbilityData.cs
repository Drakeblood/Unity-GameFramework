using System;

using UnityEngine;

using TypeReferences;

using GameFramework.System;
using GameFramework.System.Attributes;

namespace GameFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "GameFramework/AbilitySystem/GameplayAbilityData")]
    public class GameplayAbilityData : ScriptableObject
    {
        [Inherits(typeof(GameplayAbility), IncludeBaseType = true, ShowNoneElement = false)]
        public TypeReference AbilityClass = new TypeReference(typeof(GameplayAbility));

        [SerializeField, HideInInspector]
        private TypeReference OldAbilityClass = new TypeReference(typeof(GameplayAbility));

        [SerializeReference]
        private GameplayAbility GameplayAbility;
        public GameplayAbility GetGameplayAbility() => GameplayAbility;

        private void OnValidate()
        {
            if(AbilityClass.Type != OldAbilityClass.Type)
            {
                GameplayAbility = (GameplayAbility)Activator.CreateInstance(AbilityClass);
                GameplayAbility.AbilityData = this;

                OldAbilityClass.Type = AbilityClass.Type;
            }
        }

        [Tooltip("GameplayTags that the GameplayAbility owns. These are just GameplayTags to describe the GameplayAbility.")]
        [GameplayTag] public GameplayTag[] AbilityTags;

        [Tooltip("Other GameplayAbilities that have these GameplayTags in their Ability Tags will be canceled when this GameplayAbility is activated.")]
        [GameplayTag] public GameplayTag[] CancelAbilitiesWithTag;

        [Tooltip("Other GameplayAbilities that have these GameplayTags in their Ability Tags are blocked from activating while this GameplayAbility is active.")]
        [GameplayTag] public GameplayTag[] BlockAbilitiesWithTag;

        [Tooltip("These GameplayTags are given to the GameplayAbility's owner while this GameplayAbility is active.")]
        [GameplayTag] public GameplayTag[] ActivationOwnedTags;

        [Tooltip("This GameplayAbility can only be activated if the owner has all of these GameplayTags.")]
        [GameplayTag] public GameplayTag[] ActivationRequiredTags;

        [Tooltip("This GameplayAbility cannot be activated if the owner has any of these GameplayTags.")]
        [GameplayTag] public GameplayTag[] ActivationBlockedTags;

        //[Tooltip("This GameplayAbility can only be activated if the Source has all of these GameplayTags. The Source GameplayTags are only set if the GameplayAbility is triggered by an event.")]
        //[GameplayTag] public GameplayTag[] SourceRequiredTags;

        //[Tooltip("This GameplayAbility cannot be activated if the Source has any of these GameplayTags. The Source GameplayTags are only set if the GameplayAbility is triggered by an event.")]
        //[GameplayTag] public GameplayTag[] SourceBlockedTags;

        //[Tooltip("This GameplayAbility can only be activated if the Target has all of these GameplayTags. The Target GameplayTags are only set if the GameplayAbility is triggered by an event.")]
        //[GameplayTag] public GameplayTag[] TargetRequiredTags;

        //[Tooltip("This GameplayAbility cannot be activated if the Target has any of these GameplayTags. The Target GameplayTags are only set if the GameplayAbility is triggered by an event.")]
        //[GameplayTag] public GameplayTag[] TargetBlockedTags;
    }
}