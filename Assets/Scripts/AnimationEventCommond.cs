using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCommond : MonoBehaviour
{
    PlayerContainer container;

    public void CheckAttack()
    {
        //Debug.Log("Ç»ÇÒÇ∆Ç©Ç»ÇÍÅIÅI");
        container = FindAnyObjectByType<PlayerContainer>();
        container.state = PlayerContainer.MyState.Normal;
    }
}
