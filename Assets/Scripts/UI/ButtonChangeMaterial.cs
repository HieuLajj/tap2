using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ButtonChangeMaterial : MonoBehaviour
{
    public int index;
    public TextMeshProUGUI textMeshPro;
    private SkinItemData skinData;
    private void Start()
    {
        skinData = LevelManager.Instance.skindata.GetSkinData(index);
        textMeshPro.text = skinData.Name;
    }
    public void ClickChangeMaterial()
    {
        Controller.Instance.ChangSkin(index);
    }
}
