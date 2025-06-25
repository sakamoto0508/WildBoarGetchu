using UnityEngine;

public class MovementTask : ITutorialTask
{
    public string GetTitle()
    {
        return "基本操作 移動";
    }

    public string GetText()
    {
        return "WSADキーで上下左右に移動できる";
    }

    public void OnTaskSetting()
    {
        // 今は特に何もしていない（将来的に案内表示や効果音などを追加してもOK）
    }

    public bool CheckTask()
    {
        float axis_h = Input.GetAxis("Horizontal");
        float axis_v = Input.GetAxis("Vertical");

        if (0 < axis_v || 0 < axis_h)
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