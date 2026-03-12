using System;
using UnityEngine;

public sealed class ScoreManager
{

    private static readonly Lazy<ScoreManager> instance = 
        new Lazy<ScoreManager>(() => new ScoreManager());

    public static ScoreManager Instance => instance.Value;

    private ScoreManager()
    {
        Initialize();
    }

    public void Initialize()
    {
        score = 0;
        elapseTime = 0.0f;
    }


    private DateTime startTime;
    private float elapseTime;
    public void StartTimeMeasure()
    {

        //時間の初期化(現在時間を代入する)など
        startTime = DateTime.Now;

    }

    public void StopTimeMeasure()
    {

        //終了時間を取得し、クリアに掛かった時間を求める
        elapseTime = (float)(DateTime.Now - startTime).TotalSeconds;

    }

    public float GetElapesTime()
    {
        return elapseTime;
    }

    public float GetCurrentElapseTime()
    {
        return (float)(DateTime.Now - startTime).TotalSeconds;
    }

    private const int defaultScore = 500;
    private const int addScoreValue = 1000;
    private int score = defaultScore;

    public void OnGetScoreItem()
    {
        //スコアアイテムを取得した時の処理
        //基礎スコア変動を行う

        score += addScoreValue;

    }

    public void AddScore(int value)
    {
        score += value;
    }

    public int GetScore()
    {
        return score;
    }


}
