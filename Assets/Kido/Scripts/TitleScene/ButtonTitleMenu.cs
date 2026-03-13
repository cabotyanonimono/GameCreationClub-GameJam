using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTitleMenu : MonoBehaviour
{
    public void PushStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PushQuitButton()
    {
        #if UNITY_EDITOR

        UnityEditor.EditorApplication.isPlaying = false;

        #else

        Application.Quit();

        #endif
    }

}
