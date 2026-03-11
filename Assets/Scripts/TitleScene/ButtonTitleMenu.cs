using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonTitleMenu : MonoBehaviour
{
    public void PushStartButton()
    {
        SceneManager.LoadScene("SampleScene");
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
