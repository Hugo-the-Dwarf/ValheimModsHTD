using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMods : MonoBehaviour
{
    public int qualityLevel = 1;

    // Start is called before the first frame update
    void Start()
    {
        transform.Find("levelmods/lvl1").gameObject.SetActive(false);
        transform.Find("levelmods/lvl2").gameObject.SetActive(false);
        transform.Find("levelmods/lvl3").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.gameObject.name.Contains("Clone"))
        {
            TryApplyMods();
        }
        if (transform.parent.gameObject.name.Contains("Clone"))
        {
            TryApplyMods();
        }
    }

    private void TryApplyMods()
    {
        if (qualityLevel > 1) transform.Find("levelmods/lvl1").gameObject.SetActive(true);
        if (qualityLevel > 2) transform.Find("levelmods/lvl2").gameObject.SetActive(true);
        if (qualityLevel > 3) transform.Find("levelmods/lvl3").gameObject.SetActive(true);
    }
}
