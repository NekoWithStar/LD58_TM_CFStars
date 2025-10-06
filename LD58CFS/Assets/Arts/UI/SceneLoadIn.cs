using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadIn : MonoBehaviour
{
    [Tooltip("要跳转的场景名（非退出按钮时有效）")]
    public string targetSceneName;

    [Tooltip("是否为退出按钮")]
    public bool isExitButton = false;

    // UI按钮点击时调用此方法
    public void OnButtonClick()
    {
        if (isExitButton)
        {
#if UNITY_EDITOR
            // 编辑器中关闭运行
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 编辑器外退出运行
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
                Debug.LogWarning("未设置目标场景名！");
            }
        }
    }
}
