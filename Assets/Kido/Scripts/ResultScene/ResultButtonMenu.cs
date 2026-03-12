using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultButtonMenu : MonoBehaviour
{
    
    public void PushGoTitle()
    {
        ScoreManager.Instance.Initialize();

        SceneManager.LoadScene("TitleScene");

    }

    public void PushRestart()
    {
        ScoreManager.Instance.Initialize();

        //ƒQ[ƒ€ƒV[ƒ“‚É”ò‚Ô
    }

}
