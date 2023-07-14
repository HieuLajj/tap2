using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    public Touchpad touchpad;
    public GameObject parent;
    
    public Transform CameraMain;
    private float currentVelocity;
    private Vector2 lookAt;
    public InGameUI GameUIIngame;
    public Block BlockGiftPresent;
    public RewardManager RewardManager;
    public GameObject UIBoom;
    public GameObject CompleteLevelUI;
    public GiftUIManager giftUIManager;
    public GameObject SelectLevelUI;
    public GameObject FunctionsButtons;
    public GameObject SelectShopUI;
    public GameObject SelectHomeUI;
    public RectTransform canvasRectTransform;
    public void SwipeScreen()
    {
        lookAt.x = Mathf.Lerp(lookAt.x, -touchpad.LookInput().x, 2 * Time.deltaTime);
        lookAt.y = Mathf.Lerp(lookAt.y, touchpad.LookInput().y, 2 * Time.deltaTime);
        parent.transform.Rotate(Vector3.up, lookAt.x, Space.World);
        parent.transform.Rotate(Vector3.right, lookAt.y, Space.World);
        //parent.transform.Rotate(Vector3.up, -touchpad.LookInput().x, Space.World);
        //parent.transform.Rotate(Vector3.right, touchpad.LookInput().y, Space.World);
        //Quaternion targetRotation = Quaternion.Euler(0, -touchpad.LookInput().x, 0);
        //float smoothTime = 0.3f; 
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothTime);
        //float targetangle = -touchpad.lookinput().x;
        //float smoothangle = mathf.smoothdampangle(parent.transform.eulerangles.y, targetangle, ref currentvelocity, 0.3f);
        //parent.transform.rotation = quaternion.euler(0, smoothangle, 0);
    }

    public void DisplayGamePlay()
    {
        UIBoom.SetActive(false);
        FunctionsButtons.SetActive(false);
    }

    public void ActiveGamePlay()
    {
        UIBoom.SetActive(true);
        FunctionsButtons.SetActive(true);
    }
}
