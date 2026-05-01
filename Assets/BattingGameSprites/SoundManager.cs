using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("スピーカー（AudioSource）")]
    public AudioSource seSource;

    [Header("オーディオクリップ（WAVファイル）")]
    public AudioClip swingSound;

    // 👇 変更：複数の音を登録できるように「配列（[]）」にする！
    public AudioClip[] hitEarlyLateSounds;

    public AudioClip hitGreatSound;
    public AudioClip hitPerfectSound;

    public void PlaySwing()
    {
        if (seSource != null && swingSound != null)
        {
            seSource.PlayOneShot(swingSound);
        }
    }

    public void PlayHitSound(BallController.HitType hitType)
    {
        if (seSource == null) return;

        switch (hitType)
        {
            case BallController.HitType.Perfect:
                if (hitPerfectSound != null) seSource.PlayOneShot(hitPerfectSound);
                break;

            case BallController.HitType.Great:
                if (hitGreatSound != null) seSource.PlayOneShot(hitGreatSound);
                break;

            case BallController.HitType.Early:
            case BallController.HitType.Late:
                // 👇 変更：配列の中に音が登録されているか確認して、ランダムに鳴らす！
                if (hitEarlyLateSounds != null && hitEarlyLateSounds.Length > 0)
                {
                    // Random.Range(0, 要素の数) でランダムな番号（インデックス）を取得
                    int randomIndex = Random.Range(0, hitEarlyLateSounds.Length);
                    seSource.PlayOneShot(hitEarlyLateSounds[randomIndex]);
                }
                break;
        }
    }
}