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

        //���Ԃ̏�����(���ݎ��Ԃ�������)�Ȃ�
        startTime = DateTime.Now;

    }

    public void StopTimeMeasure()
    {

        //�I�����Ԃ��擾���A�N���A�Ɋ|���������Ԃ����߂�
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
        //�X�R�A�A�C�e�����擾�������̏���
        //��b�X�R�A�ϓ����s��

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
