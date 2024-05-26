using System;

using UnityEngine;

using TypeReferences;

using GameFramework.System;
using GameFramework.System.Attributes;
using UnityEngine.InputSystem;

namespace GameFramework.AbilitySystem
{
    [CreateAssetMenu(menuName = "GameFramework/AbilitySystem/GameplayAbilityDefinition")]
    public class GameplayAbilityDefinition : ScriptableObject
    {
        [Inherits(typeof(GameplayAbility), IncludeBaseType = true, ShowNoneElement = false)]
        public TypeReference AbilityClass = new(typeof(GameplayAbility));

        [SerializeField, HideInInspector]
        private TypeReference oldAbilityClass = new(typeof(GameplayAbility));

        [SerializeReference]
        private GameplayAbility gameplayAbility;
        public GameplayAbility GameplayAbility => gameplayAbility;

        public InputActionReference InputActionReference;

        private void OnValidate()
        {
            if(AbilityClass.Type != oldAbilityClass.Type)
            {
                gameplayAbility = (GameplayAbility)Activator.CreateInstance(AbilityClass);
                gameplayAbility.AbilityDefinition = this;

                oldAbilityClass.Type = AbilityClass.Type;
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