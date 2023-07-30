using System.Timers;

using UnityEngine;

using GameFramework.AbilitySystem;

public class GA_Test : GameplayAbility
{
    public GameObject G1;
    private Timer T1 = new Timer();

    private Renderer Renderer;

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        Debug.Log("GA_Test activated");

        Renderer = Object.FindObjectOfType<Test>().GetComponent<Renderer>();

        T1.Interval = 2000;
        T1.Enabled = true;
        T1.Elapsed += GA;

        if (G1 != null)
        {
            Debug.Log(G1.name);
            Renderer.material.color = Color.blue;
        }
    }

    public override void EndAbility(bool WasCanceled)
    {
        base.EndAbility(WasCanceled);
    }

    public void GA(object sender, ElapsedEventArgs e)
    {
        T1.Enabled = false;
        T1.Stop();

        EndAbility(false);
    }
}
