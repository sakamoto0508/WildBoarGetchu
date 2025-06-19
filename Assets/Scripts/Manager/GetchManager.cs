using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.AI;
public class GetchuManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _getchuCamera;
    //[SerializeField] private GameObject *getchuUI;
    [SerializeField] private AudioClip _getchuSE;
    public void PlayGetchuSequence(Transform enemyTransform, Transform teleportPoint, NavMeshAgent agent)
    {
        StartCoroutine(GetchuSequenceCoroutine(enemyTransform, teleportPoint, agent));
    }
    private IEnumerator GetchuSequenceCoroutine(Transform enemyTransform, Transform teleportPoint, NavMeshAgent agent)
    {
        // Getchu用カメラに敵を設定
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;
        // カメラ切り替え（SetActive）
        _mainCamera.gameObject.SetActive(false);
        _getchuCamera.gameObject.SetActive(true);
        // 時間をスローに
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        // UI表示
        //_getchuUI.SetActive(true);
        // SE再生
        //AudioSource.PlayClipAtPoint(_getchuSE, Camera.main.transform.position);

        // カメラが敵を映している間、少し待つ
        yield return new WaitForSecondsRealtime(2f);
     
        // 時間を戻す
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // カメラを戻す
        _getchuCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);

        // 敵をテレポート
        enemyTransform.position = teleportPoint.position;
        // NavMeshAgent を再び有効化
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(teleportPoint.position); // NavMesh に合わせるなら Warp 推奨
        }
        // UIを消す
        //_getchuUI.SetActive(false);
    }
}