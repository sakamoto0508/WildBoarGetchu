using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpTask : ITutorialTask
{
    public string GetTitle()
    {
        return "基本操作 ジャンプ";
    }

    public string GetText()
    {
        return "Spaceでジャンプができる"
            + Environment.NewLine + "ジャンプしてみよう！！";
    }

    public void OnTaskSetting()
    {
        // 今は特に何もしていない（将来的に案内表示や効果音などを追加してもOK）
    }

    public bool CheckTask()
    {
        // スペースキーが押されたかをチェック
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
