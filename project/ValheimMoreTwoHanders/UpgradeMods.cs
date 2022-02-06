using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMods : MonoBehaviour
{
    bool persistPastLevel2 = true;
    bool persistPastLevel3 = true;

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
        //if (transform.gameObject.name.Contains("Clone"))
        //{
        //    TryApplyMods();
        //}
        //if (transform.parent.gameObject.name.Contains("Clone"))
        //{
        //    TryApplyMods();
        //}
    }

    private void TryApplyMods(ItemDrop itemInstance)
    {
        var id = itemInstance.m_itemData;
        if ((id.m_quality == 2 && !persistPastLevel2) || (id.m_quality >= 2 && persistPastLevel2)) transform.Find("levelmods/lvl2").gameObject.SetActive(true);
        if ((id.m_quality == 3 && !persistPastLevel3) || (id.m_quality >= 3 && persistPastLevel3)) transform.Find("levelmods/lvl3").gameObject.SetActive(true);
        if (id.m_quality >= 4) transform.Find("levelmods/lvl4").gameObject.SetActive(true);
    }
}
