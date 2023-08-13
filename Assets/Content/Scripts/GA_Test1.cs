using UnityEngine;

using GameFramework.AbilitySystem;

public class GA_Test1 : GameplayAbility
{
    public string Text = "a";

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        Debug.Log("GA_Test1 activated");
        Debug.Log(Text);
        Finish();
    }

    public void Finish()
    {
        Debug.Log("GA_Test1 ended");
        EndAbility(false);
    }
}
