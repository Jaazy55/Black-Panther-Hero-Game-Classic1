using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class panelDelay : MonoBehaviour
{
    bool check;
    public Image Bar;
    public float TimeDelay;

    private void OnEnable()
    {
        check = false;
        Bar.fillAmount = 0f;
        //   Invoke(nameof(ShowInterstitialWithDelay), 0.5f);
        StartCoroutine(ShowInterstitialWithDelay());
    }



    public IEnumerator ShowInterstitialWithDelay()
    {
        yield return new WaitForSecondsRealtime(1f);
      //  CapricornAdsManager.instance.ShowInterstitial();
    }


    void Update()
    {
        if(!check && Bar.fillAmount < 1)
        Bar.fillAmount += 1 / TimeDelay * Time.unscaledDeltaTime;
        if (Bar.fillAmount >= 1)
        {
            this.gameObject.SetActive(false);
            check = true;
        }
    }
}
