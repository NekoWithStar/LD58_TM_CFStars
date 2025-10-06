using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadIn : MonoBehaviour
{
    [Tooltip("Ҫ��ת�ĳ����������˳���ťʱ��Ч��")]
    public string targetSceneName;

    [Tooltip("�Ƿ�Ϊ�˳���ť")]
    public bool isExitButton = false;

    // UI��ť���ʱ���ô˷���
    public void OnButtonClick()
    {
        if (isExitButton)
        {
#if UNITY_EDITOR
            // �༭���йر�����
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // �༭�����˳�����
            Application.Quit();
#endif
        }
        else
        {
            if (!string.IsNullOrEmpty(targetSceneName))
            {
                SceneManager.LoadScene(targetSceneName);
            }
            else
            {
                Debug.LogWarning("δ����Ŀ�곡������");
            }
        }
    }
}
