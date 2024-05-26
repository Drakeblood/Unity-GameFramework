using System.Collections;

using UnityEngine;

using GameFramework.AbilitySystem;

public class GA_Test : GameplayAbility
{
    public GameObject G1;

    private Renderer Renderer;

    public override void ActivateAbility()
    {
        base.ActivateAbility();

        if (G1 != null)
        {
            Renderer = Object.FindObjectOfType<Test>().GetComponent<Renderer>();
            Renderer.material.color = Color.blue;
        }

        StartCoroutine(Finish());
    }

    public override void EndAbility(bool WasCanceled)
    {
        Debug.Log($"{this} WasCanceled: {WasCanceled}");
        base.EndAbility(WasCanceled);
    }

    public IEnumerator Finish()
    {
        yield return new WaitForSeconds(2f);

        Renderer.material.color = Color.black;
        Debug.Log("GA_Test1 ended: " + G1.name);
        
        EndAbility(false);
    }
}
