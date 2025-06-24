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
        // null�`�F�b�N�ƍĎ擾
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
        // NavMeshAgent�����S�ɒ�~
        if (agent != null)
        {
            agent.enabled = false;
        }

        // Getchu�p�J�����ɓG��ݒ�
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;

        // �G�t�F�N�g���ɐ����E�ݒ�i�X���[�O�Ɏ��s�j
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

        // �J�����؂�ւ�
        _mainCamera.gameObject.SetActive(false);
        _getchuCamera.gameObject.SetActive(true);

        // ���Ԃ��X���[��
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // �G�t�F�N�g�Đ�
        if (effectInstance != null)
        {
            effectInstance.Play();
            float effectDuration = effectInstance.main.duration;
            StartCoroutine(DestroyEffectAfterDuration(effectInstance.gameObject, effectDuration));
        }

        // �J�������G���f���Ă���ԁA�����҂�
        yield return new WaitForSecondsRealtime(1f);

        // ���Ԃ�߂�
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // �J������߂�
        _getchuCamera.gameObject.SetActive(false);
        _mainCamera.gameObject.SetActive(true);

        // �G���e���|�[�g
        enemyTransform.position = teleportPoint.position;

        // NavMeshAgent ���ĂїL����
        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(teleportPoint.position);
            // �e���|�[�g�����~��Ԃ��ێ�
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