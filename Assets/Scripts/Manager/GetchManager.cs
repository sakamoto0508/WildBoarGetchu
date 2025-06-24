using UnityEngine;
using Cinemachine;
using System.Collections;
using UnityEngine.AI;

public class GetchuManager : MonoBehaviour
{
    [SerializeField] private CinemachineFreeLook _mainCamera;
    [SerializeField] private CinemachineVirtualCamera _getchuCamera;
    [SerializeField] private ParticleSystem _getchuEffectPrefab;

    private void Start()
    {
        if (_mainCamera == null) ;
        if (_getchuCamera == null) ;
        if (_getchuEffectPrefab == null) ; 
    }
    public void PlayGetchuSequence(Transform enemyTransform, Transform teleportPoint, NavMeshAgent agent)
    {
        StartCoroutine(GetchuSequenceCoroutine(enemyTransform, teleportPoint, agent));
    }

    private IEnumerator GetchuSequenceCoroutine(Transform enemyTransform, Transform teleportPoint, NavMeshAgent agent)
    {
        // nullチェックと再取得
        if (_getchuCamera == null)
        {
            _getchuCamera = FindObjectOfType<CinemachineVirtualCamera>();
            if (_getchuCamera == null)
            {
                yield break;
            }
        }

        if (_mainCamera == null)
        {
            _mainCamera = FindObjectOfType<CinemachineFreeLook>();
            if (_mainCamera == null)
            {
                yield break;
            }
        }
        // NavMeshAgentを完全に停止
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Getchu用カメラに敵を設定
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;

        // エフェクトを先に生成・設定（スロー前に実行）
        ParticleSystem effectInstance = null;
        if (_getchuEffectPrefab != null)
        {
            Vector3 spawnPos = enemyTransform.position;
            Quaternion spawnRot = Quaternion.LookRotation(Vector3.up);
            effectInstance = Instantiate(_getchuEffectPrefab, spawnPos, spawnRot);

            var main = effectInstance.main;
            main.simulationSpace = ParticleSystemSimulationSpace.World;
            main.useUnscaledTime = true;
        }

        // カメラ切り替え
        _mainCamera.gameObject.SetActive(false);
        _getchuCamera.gameObject.SetActive(true);

        // 時間をスローに
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // エフェクト再生
        if (effectInstance != null)
        {
            effectInstance.Play();
            float effectDuration = effectInstance.main.duration;
            StartCoroutine(DestroyEffectAfterDuration(effectInstance.gameObject, effectDuration));
        }

        // カメラが敵を映している間、少し待つ
        yield return new WaitForSecondsRealtime(1f);

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
            agent.Warp(teleportPoint.position);
            // テレポート後も停止状態を維持
            agent.isStopped = true;
            agent.ResetPath();
        }

    }

    private IEnumerator DestroyEffectAfterDuration(GameObject effectObject, float duration)
    {
        yield return new WaitForSecondsRealtime(duration);
        if (effectObject != null)
        {
            Destroy(effectObject);
        }
    }
}