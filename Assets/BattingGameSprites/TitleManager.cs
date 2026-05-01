using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    // 👇 ボタンが押された時に呼ばれるメソッド
    public void GoToGameScene()
    {
        // "SampleScene" という名前のシーンを読み込む
        // ※もしゲーム画面のシーン名を変更している場合は、ここを書き換えてください
        SceneManager.LoadScene("SampleScene");
    }
}