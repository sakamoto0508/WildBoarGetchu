using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventCommond : MonoBehaviour
{
    PlayerContainer container;

    public void CheckAttack()
    {
        //Debug.Log("�Ȃ�Ƃ��Ȃ�I�I");
        container = FindAnyObjectByType<PlayerContainer>();
        container.state = PlayerContainer.MyState.Normal;
    }
}
