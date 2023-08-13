
namespace GameFramework.AbilitySystem
{
    public abstract class AbilityTask : object
    {
        protected GameplayAbility OwningAbility = null;

        public AbilityTask(GameplayAbility InOwningAbility)
        {
            OwningAbility = InOwningAbility;
            OwningAbility.OnAbilityEnded += OnAbilityEnded;
        }

        private void OnAbilityEnded(bool WasCanceled)
        {
            EndTask();
        }

        public void ReadyForActivation()
        {
            Activate();
        }

        protected abstract void Activate();

        protected virtual void EndTask()
        {

        }
    }
}