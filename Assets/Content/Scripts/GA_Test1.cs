using System.Timers;

using UnityEngine;

using GameFramework.AbilitySystem;

public class GA_Test1 : GameplayAbility
{
    public string Text = "a";
    private Timer T1 = new Timer();

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        Debug.Log("GA_Test1 activated");

        T1.Interval = 2000;
        T1.Enabled = true;
        T1.Elapsed += GA1;
        Debug.Log(Text);
    }

    public void GA1(object sender, ElapsedEventArgs e)
    {
        T1.Enabled = false;
        T1.Stop();
        EndAbility(false);
    }
}
