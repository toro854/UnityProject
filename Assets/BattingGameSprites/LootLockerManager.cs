using UnityEngine;
using LootLocker.Requests; // 👈 LootLockerの機能を使うための呪文
using TMPro;

public class LootLockerManager : MonoBehaviour
{
    // 👇 インスペクターで設定できるようにします
    [Header("リーダーボード設定")]
    public int leaderboardID; 

    [Header("UI設定")]
    public TextMeshProUGUI leaderboardText;



    void Start()
    {
        // ゲームが始まったら、裏側で自動的にゲストログインを行う
        StartGuestSession();
    }

    private void StartGuestSession()
    {
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("LootLocker ログイン大成功！ プレイヤーID: " + response.player_id);
            }
            else
            {
                Debug.Log("ログイン失敗... 理由: " + response.errorData.message);
            }
        });
    }

    // 👇 ScoreManagerなどから呼ばれる「スコア送信」のメソッド
    public void SubmitHighScore(int scoreToUpload)
    {
        // ゲストログインが成功している前提で、スコアを送信する
        // ※ 最初の "" はプレイヤーID（空欄でOK）、次はスコア、最後はボードIDです
        LootLockerSDKManager.SubmitScore("", scoreToUpload, leaderboardID.ToString(), (response) =>
        {
            if (response.success)
            {
                Debug.Log("スコア送信成功！ サーバーに記録されました！");
            }
            else
            {
                Debug.Log("スコア送信失敗... 理由: " + response.errorData.message);
            }
        });
    }

    // 👇サーバーからランキングを取得して表示するメソッド
    public void FetchHighScores()
    {
        // 取得中は「Loading...」と表示しておく
        if (leaderboardText != null) leaderboardText.text = "Loading...";

        // 最大10件のスコアを取得する
        int count = 10;

        LootLockerSDKManager.GetScoreList(leaderboardID.ToString(), count, (response) =>
        {
            if (response.success)
            {
                Debug.Log("ランキング取得成功！");

                // タイトル部分を作る
                string tempPlayerNames = "Top " + count + " Scores\n\n";

                // 取得したスコアのリストを順番に処理する
                LootLockerLeaderboardMember[] scores = response.items;

                for (int i = 0; i < scores.Length; i++)
                {
                    // 順位とスコア
                    int rank = scores[i].rank;
                    int score = scores[i].score;

                    // プレイヤー名（まだ名前登録機能を作っていないので、ゲストIDを表示します）
                    string playerName = scores[i].player.name != "" ? scores[i].player.name : "Guest " + scores[i].player.id;

                    // 1行分のテキストを作成して改行（\n）を追加
                    // 例: "1. Guest 1234 : 1500"
                    tempPlayerNames += rank + ". " + playerName + " : " + score + "\n";
                }

                // 画面のテキストを書き換える
                if (leaderboardText != null)
                {
                    leaderboardText.text = tempPlayerNames;
                }
            }
            else
            {
                Debug.Log("ランキング取得失敗... 理由: " + response.errorData.message);
                if (leaderboardText != null) leaderboardText.text = "エラーが発生しました";
            }
        });
    }


}