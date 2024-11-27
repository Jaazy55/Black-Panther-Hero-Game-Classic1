using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Ads_Loading : MonoBehaviour
{
    IEnumerator Start()
    {
        yield return  new WaitForSeconds(5.0f);
        SceneManager.LoadScene(1);
    }
}
