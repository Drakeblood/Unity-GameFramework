using System.Timers;

using UnityEngine;

using GameFramework.AbilitySystem;

public class GA_Test : GameplayAbility
{
    public GameObject G1;
    private Timer T1 = new Timer();

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        Debug.Log("GA_Test activated");

        T1.Interval = 2000;
        T1.Enabled = true;
        T1.Elapsed += GA;
        Debug.Log(G1 != null ? G1.name : "null");
    }

    public void GA(object sender, ElapsedEventArgs e)
    {
        T1.Enabled = false;
        T1.Stop();
        EndAbility(false);
    }
}
