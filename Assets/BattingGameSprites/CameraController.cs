using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Rendering;
using System.Collections;

public class HitEffectManager : MonoBehaviour
{
    [Header("カメラ設定")]
    public CinemachineCamera mainCam;
    public CinemachineCamera zoomCam;
    private CinemachineImpulseSource impulseSource;

    [Header("エフェクト・UI設定")]
    public Volume hitVolume;
    public GameObject flashUI;
    public ParticleSystem hitSparks;

    [Header("ヒットストップ背景エフェクト")]
    public GameObject hitStopBackground;

    [Header("Perfect時専用エフェクト")]
    public GameObject lightFrameUI;

    void Start()
    {
        // アタッチしたコンポーネントを取得
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void PlaySparks(Vector3 hitPosition, int count)
    {
        if (hitSparks != null)
        {
            hitSparks.transform.position = hitPosition;
            hitSparks.Emit(count);
        }
    }

    public void HitStopEffect(bool isPerfect = false)
    {
        // カメラのズームイン
        zoomCam.Priority = 20;
        mainCam.Priority = 10;

        // 【スロー演出】時間を完全に止めず、10%（0.1f）の速度にする
        Time.timeScale = 0.01f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        // 【画面揺れ演出】
        if (impulseSource != null)
        {
            impulseSource.GenerateImpulse();
        }

        // 【ポストプロセッシング】
        if (hitVolume != null)
        {
            hitVolume.weight = 1f;
        }

        // 【フラッシュ演出】
        if (flashUI != null)
        {
            StartCoroutine(FlashRoutine());
        }

        // 【背景エフェクト表示】
        if (hitStopBackground != null)
        {
            hitStopBackground.SetActive(true);
        }
        if (isPerfect && lightFrameUI != null)
        {
            lightFrameUI.SetActive(true);
        }
    }

    private IEnumerator FlashRoutine()
    {
        flashUI.SetActive(true);
        yield return new WaitForSecondsRealtime(0.05f);
        flashUI.SetActive(false);
    }

    public void ReturnNormal()
    {

        // カメラを元に戻す
        zoomCam.Priority = 5;
        mainCam.Priority = 10;

        // 時間と物理演算のペースを通常（1.0）に戻す
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;


        // ポストプロセッシングを戻す
        if (hitVolume != null)
        {
            hitVolume.weight = 0f;
        }

        // 追加：【背景エフェクト非表示】
        if (hitStopBackground != null)
        {
            hitStopBackground.SetActive(false);
        }
        if (lightFrameUI != null)
        {
            lightFrameUI.SetActive(false);
        }

    }

    public void TriggerLightHitStop()
    {
        StartCoroutine(LightHitStopRoutine());
    }

    // 👇 追加：実際に時間を遅らせて待つ処理
    private IEnumerator LightHitStopRoutine()
    {
        // 時間を半分のスピード（0.5倍速）にする
        Time.timeScale = 0.25f;
        Time.fixedDeltaTime = 0.05f * Time.timeScale;

        // 【オプション】もしごく微弱な揺れも入れたければ以下のコメントアウトを外す
         if (impulseSource != null) impulseSource.GenerateImpulseWithForce(0.1f);

        // 現実時間で 0.05秒 だけ待つ
        yield return new WaitForSecondsRealtime(0.05f);

        // 既存の ReturnNormal を呼んで通常スピードに戻す！
        ReturnNormal();
    }
}