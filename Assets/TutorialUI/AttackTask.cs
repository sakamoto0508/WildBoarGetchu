using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTask : ITutorialTask
{
    private static bool enemyCaught = false;
    private static bool isActive = false; // ���̃^�X�N���A�N�e�B�u���ǂ���

    public string GetTitle()
    {
        return "��{���� �U��";
    }

    public string GetText()
    {
        return "���N���b�N�ŃQ�b�`���A�~��U�邱�Ƃ��ł���" 
            + Environment.NewLine + "�C�m�V�V�����ׂĕ߂܂���I�I";
    }

    public void OnTaskSetting()
    {
        // �`���[�g���A���J�n���Ƀt���O�����Z�b�g
        enemyCaught = false;
        isActive = true; // �^�X�N���A�N�e�B�u�ɂ���
    }

    public bool CheckTask()
    {
        // �G���߂܂������ǂ������m�F
        if (enemyCaught)
        {
            isActive = false; // �^�X�N�������ɔ�A�N�e�B�u�ɂ���
            return true;
        }
        return false;
    }

    public float GetTransitionTime()
    {
        return 2f;
    }

    // �G���߂܂����Ƃ��ɌĂяo�����static���\�b�h
    public static void OnEnemyCaught()
    {
        // ���̃^�X�N���A�N�e�B�u�ȏꍇ�̂݃t���O��ݒ�
        if (isActive)
        {
            enemyCaught = true;
        }
    }
}