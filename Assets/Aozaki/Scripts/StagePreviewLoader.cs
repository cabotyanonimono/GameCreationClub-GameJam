using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StagePreviewLoader : MonoBehaviour
{
    [Header("Build Settingsに登録済みのScene名")]
    [SerializeField] private string stageSceneName = "Stage1";

    [Header("プレビューでは止めたいBehaviourを一括で無効化する")]
    [SerializeField] private bool disableAllMonoBehavioursInStage = false;

    private Scene loadedStageScene;

    private IEnumerator Start()
    {
        // すでにロード済みならスキップ
        if (!SceneManager.GetSceneByName(stageSceneName).isLoaded)
        {
            var op = SceneManager.LoadSceneAsync(stageSceneName, LoadSceneMode.Additive);
            while (!op.isDone) yield return null;
        }

        loadedStageScene = SceneManager.GetSceneByName(stageSceneName);

        // オプション：ステージ側のMonoBehaviourを止める（やや強引）
        if (disableAllMonoBehavioursInStage)
        {
            foreach (var root in loadedStageScene.GetRootGameObjects())
            {
                foreach (var mb in root.GetComponentsInChildren<MonoBehaviour>(true))
                {
                    // このローダ自身などを止めないように、ステージシーン内だけに限定している想定
                    mb.enabled = false;
                }
            }
        }

        // ここで必要なら：ステージ側の特定オブジェクトだけON/OFF、レイヤー変更、カメラ無効化等を行う
        // 例）Stage側にMainCameraがあるなら無効化する、など
    }

    private void OnDestroy()
    {
        // タイトルシーンを抜けるときにプレビュー用ステージをアンロードしたいなら
        if (loadedStageScene.IsValid() && loadedStageScene.isLoaded)
        {
            SceneManager.UnloadSceneAsync(loadedStageScene);
        }
    }
}