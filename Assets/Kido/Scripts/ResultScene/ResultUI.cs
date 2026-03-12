using UnityEngine;
using TMPro;
using System.Collections;
using System.Xml;
using UnityEditor;

public class ResultUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI rankText;

    [SerializeField] private float animationDuration = 1.5f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int finalScore = ScoreManager.Instance.GetScore();
        float finalTime = ScoreManager.Instance.GetElapesTime();

        StartCoroutine(AnimateScore(finalScore));
        StartCoroutine(AnimateTime(finalTime));

        if(finalScore >= 10000)
        {
            rankText.text = "S";
        }else if(finalScore >= 7500)
        {
            rankText.text = "A";
        }else if(finalScore >= 5000)
        {
            rankText.text = "B";
        }else if(finalScore >= 2500)
        {
            rankText.text = "C";
        }
        else
        {
            rankText.text = "D";
        }

    }

    private IEnumerator AnimateScore(int targetScore)
    {
        float timer = 0.0f;
        int startValue = 0;

        while(timer < animationDuration){
            timer += Time.deltaTime;
            float t = timer / animationDuration;

            int ccurrentTime = Mathf.FloorToInt(Mathf.Lerp(startValue, targetScore, t));
            scoreText.text = ccurrentTime.ToString();

            yield return null;
        }

        scoreText.text = $"{targetScore:000}pt";

    }

    private IEnumerator AnimateTime(float targetTime)
    {
        float timer = 0.0f;
        float startValue = 0.0f;

        while(timer < animationDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animationDuration;

            float current = Mathf.Lerp(startValue, targetTime, t);
            timeText.text = FormatTime(current);

            yield return null;

        }

        timeText.text = FormatTime(targetTime);

    }

    private string FormatTime(float time)
    {
        int minutes = (int)(time / 60.0f);
        float seconds = time % 60f;

        return $"{minutes:00}:{seconds:00}";
    }

}
