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
        // Getchu�p�J�����ɓG��ݒ�
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;
        // �J�����؂�ւ��iSetActive�j
        _mainCamera.gameObject.SetActive(false);
        _getchuCamera.gameObject.SetActive(true);
        // ���Ԃ��X���[��
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;
        // UI�\��
        //_getchuUI.SetActive(true);
        // SE�Đ�
        //AudioSource.PlayClipAtPoint(_getchuSE, Camera.main.transform.position);

        // �J�������G���f���Ă���ԁA�����҂�
        yield return new WaitForSecondsRealtime(2f);
     
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
            agent.Warp(teleportPoint.position); // NavMesh �ɍ��킹��Ȃ� Warp ����
        }
        // UI������
        //_getchuUI.SetActive(false);
    }
}