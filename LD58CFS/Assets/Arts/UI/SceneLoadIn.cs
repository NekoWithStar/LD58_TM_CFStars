using Cysharp.Threading.Tasks;
using EggFramework.UIUtil;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadIn : MonoBehaviour
{
    [Tooltip("Ҫ��ת�ĳ����������˳���ťʱ��Ч��")]
    public string targetSceneName;

    [Tooltip("�Ƿ�Ϊ�˳���ť")]
    public bool isExitButton = false;

    // UI��ť���ʱ���ô˷���
    public async void OnButtonClick()
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
                BlackScreenManager.Inst.EnterBlack(1f);
                await UniTask.Delay(1000);
                await UniTask.WhenAll(UniTask.Delay(600), SceneManager.LoadSceneAsync(targetSceneName).ToUniTask());
                BlackScreenManager.Inst.ExitBlack(1f);
            }
            else
            {
                Debug.LogWarning("δ����Ŀ�곡������");
            }
        }
    }
}
