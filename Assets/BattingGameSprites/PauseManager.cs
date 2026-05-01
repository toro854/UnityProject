using UnityEngine;
using UnityEngine.SceneManagement; // 👈 シーン移動に必要！

public class PauseManager : MonoBehaviour
{
    [Header("UI設定")]
    public GameObject pausePanel; // ポーズ画面のパネル

    private bool isPaused = false;

    void Start()
    {
        // ゲーム開始時は必ずパネルを隠し、時間を通常スピードにする
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // 👇 ポーズボタンから呼ばれるメソッド
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // 完全に時間を止める

        if (pausePanel != null)
        {
            pausePanel.SetActive(true); // パネルを表示する
        }
    }

    // 👇 再開（Resume）ボタンから呼ばれるメソッド
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // 時間を通常スピードに戻す

        if (pausePanel != null)
        {
            pausePanel.SetActive(false); // パネルを隠す
        }
    }

    // 👇 リタイアボタンから呼ばれるメソッド
    public void RetireGame()
    {
        // ⚠️ 超重要：シーンを移動する前に、必ず時間を動かしておく！
        // これを忘れると、タイトル画面が「時間停止したまま」始まってバグります。
        Time.timeScale = 1f;

        // タイトル画面（TitleScene）へ移動
        SceneManager.LoadScene("TitleScene");
    }
}