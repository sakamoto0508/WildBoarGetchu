using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTask : ITutorialTask
{
    public string GetTitle()
    {
        return "��{���� �W�����v";
    }

    public string GetText()
    {
        return "Space�ŃW�����v���ł���"
            + Environment.NewLine + "�W�����v���Ă݂悤�I�I";
    }

    public void OnTaskSetting()
    {
        // ���͓��ɉ������Ă��Ȃ��i�����I�Ɉē��\������ʉ��Ȃǂ�ǉ����Ă�OK�j
    }

    public bool CheckTask()
    {
        // �X�y�[�X�L�[�������ꂽ�����`�F�b�N
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }

        return false;
    }

    public float GetTransitionTime()
    {
        return 2f;
    }
}
