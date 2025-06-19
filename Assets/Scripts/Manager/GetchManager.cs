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
        // ���Ԃ��X���[��
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // Getchu�p�J�����ɓG��ݒ�
        _getchuCamera.LookAt = enemyTransform;
        _getchuCamera.Follow = enemyTransform;

        // �J�����؂�ւ��i�D��x�j
        _mainCamera.Priority = 5;
        _getchuCamera.Priority = 20;

        // UI�\��
        //_getchuUI.SetActive(true);

        // SE�Đ�
        //AudioSource.PlayClipAtPoint(_getchuSE, Camera.main.transform.position);

        // ���o���I�������߂�
        StartCoroutine(ResetAfterGetchu());
    }

    private IEnumerator ResetAfterGetchu()
    {
        yield return new WaitForSecondsRealtime(2f);

        // ���Ԃ�߂�
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        // �J������߂�
        _mainCamera.Priority = 20;
        _getchuCamera.Priority = 5;

        // UI������
        //_getchuUI.SetActive(false);
    }
}