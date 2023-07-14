using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFuctionUltiliti : MonoBehaviour
{
    private void OnEnable() {
        Controller.Instance.gameState = StateGame.AWAIT;
        
        UIManager.Instance.UIBoom?.GetComponent<BoomBtn>().AnUIBoom();
    }
    private void OnDisable() {
        Controller.Instance.gameState = StateGame.PLAY;
        UIManager.Instance.UIBoom?.GetComponent<BoomBtn>().HienUIBoom();
    }
}
