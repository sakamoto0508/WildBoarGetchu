using System;
using Unity.VisualScripting;
using UnityEngine;

public class AttackTask : ITutorialTask
{
    private static bool enemyCaught = false;
    private static bool isActive = false; // このタスクがアクティブかどうか

    public string GetTitle()
    {
        return "基本操作 攻撃";
    }

    public string GetText()
    {
        return "左クリックでゲッチュアミを振ることができる" 
            + Environment.NewLine + "イノシシをすべて捕まえろ！！";
    }

    public void OnTaskSetting()
    {
        // チュートリアル開始時にフラグをリセット
        enemyCaught = false;
        isActive = true; // タスクをアクティブにする
    }

    public bool CheckTask()
    {
        // 敵が捕まったかどうかを確認
        if (enemyCaught)
        {
            isActive = false; // タスク完了時に非アクティブにする
            return true;
        }
        return false;
    }

    public float GetTransitionTime()
    {
        return 2f;
    }

    // 敵が捕まったときに呼び出されるstaticメソッド
    public static void OnEnemyCaught()
    {
        // このタスクがアクティブな場合のみフラグを設定
        if (isActive)
        {
            enemyCaught = true;
        }
    }
}