using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Coin : MonoBehaviour
{
    public TextMeshProUGUI textCoin;
    
    private void Start() {
        textCoin.text = Utiliti.SetCoinsText(PlayerPrefs.GetInt("Coin"));
    }
}
