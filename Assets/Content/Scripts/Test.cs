using UnityEngine;

using GameFramework.System;
using GameFramework.System.Attributes;
using GameFramework.AbilitySystem;

public class Test : MonoBehaviour
{
    [GameplayTag] 
    public GameplayTag Tag1;

    [GameplayTag]
    public GameplayTag Tag2;

    [GameplayTag] 
    public GameplayTag[] Tags;

    ListenerHandle MessageHandle;

    void Awake()
    {
        MessageHandle = MessageRouter.RegisterListener("Test", (string InChannel, object Data) => Debug.Log(((MessageData)Data).Name));

        if (Tag1 != null)
        {
            GetComponent<Renderer>().material.color = Color.green;
            Debug.Log(Tag1 == Tag2);
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        MessageData Messagee = new()
        {
            Name = "Test Message"
        };

        MessageRouter.BroadcastMessage("Test", Messagee);
        MessageRouter.UnregisterListener(MessageHandle);
        Messagee.Name = "Test Message 2";
        MessageRouter.BroadcastMessage("Test", Messagee);

        var ASC = GetComponent<AbilitySystemComponent>();
        ASC.RegisterGameplayTagEvent(Tag1, (GameplayTag Tag, int Count) =>
        {
            Debug.Log($"{Tag}: {Count} TagEvent");
            if (Tag.GetTagId() != -1)
            {
                GetComponent<Renderer>().material.color = Color.blue;
            }
            else
            {
                GetComponent<Renderer>().material.color = Color.cyan;
            }
            //Debug.Log(Tag.GetTagName() + " | " + Tag.GetTagId() + " | " + Count);
        });
        ASC.UpdateTagMap(Tag1, 1);
        ASC.UpdateTagMap(Tag1, -1);
        ASC.UpdateTagMap(Tag2, 1);

        //for (int i = 0; i < Tags.Length; i++)
        //{
        //    ASC.UpdateTagMap(Tags[i], 1);
        //}
    }

    private void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            var ASC = GetComponent<AbilitySystemComponent>();
            ASC.TryActivateAbility(typeof(GA_Test));
        }
        if (Input.GetButtonDown("Fire2"))
        {
            var ASC = GetComponent<AbilitySystemComponent>();
            ASC.TryActivateAbility(typeof(GA_Test1));
        }
    }
}
