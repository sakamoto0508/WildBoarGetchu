using UnityEngine;
using Cinemachine;
using System.Collections;

public class GetchuManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _getchuCamera;
    //[SerializeField] private GameObject _getchuUI;
    [SerializeField] private AudioClip _getchuSE;

    public void PlayGetchuSequence(Transform enemyTransform)
    {
        // 時間をスローに
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Getchu用カメラに敵を設定
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;

        // カメラ切り替え（優先度）
        _mainCamera.Priority = 5;
        _getchuCamera.Priority = 20;

        // UI表示
        //_getchuUI.SetActive(true);

        // SE再生
        //AudioSource.PlayClipAtPoint(_getchuSE, Camera.main.transform.position);

        // 演出が終わったら戻す
        StartCoroutine(ResetAfterGetchu());
    }

    private IEnumerator ResetAfterGetchu()
    {
        yield return new WaitForSecondsRealtime(2f);

        // 時間を戻す
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // カメラを戻す
        _mainCamera.Priority = 20;
        _getchuCamera.Priority = 5;

        // UIを消す
        //_getchuUI.SetActive(false);
    }
}