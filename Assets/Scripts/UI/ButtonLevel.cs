using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class ButtonLevel : MonoBehaviour
{
  
    public TextMeshProUGUI textLevel;
    public Sprite[] spiteBg;
    public Sprite[] spiteIcon;
    public Sprite[] spiteButton;
    private int level;
    public Image imageBG;
    public Image imageCup;
    public Image imageButton;
    public TextMeshProUGUI decripText;
    private int flag2 = 0;
    public int Level
    {
        set { 
            level = value; 
            textLevel.text = level.ToString();
        }
        get {
            return level;
        }
    }

    private void OnEnable()
    {
        
        if (Controller.Instance.DiffirentGame == DiffirentEnum.EASY)
        {
            flag2 = 0;
        }
        else if (Controller.Instance.DiffirentGame == DiffirentEnum.MEDIUM)
        {
            flag2 = Controller.Instance.constantsDiffical[DiffirentEnum.EASY];
        }
        else if (Controller.Instance.DiffirentGame == DiffirentEnum.HARD)
        {
            flag2 = Controller.Instance.constantsDiffical[DiffirentEnum.EASY] + Controller.Instance.constantsDiffical[DiffirentEnum.MEDIUM];
        }
        Level = transform.GetSiblingIndex() + 1 + flag2;
        EditFromData();
    }

    public void LoadLevel()
    {
        if(UIManager.Instance.SelectHomeUI.activeInHierarchy){
            UIManager.Instance.SelectHomeUI.SetActive(false);
        }
        LevelManager.Instance.LoadLevelInGame(level);
    }
    // Update is called once per frame

    public void EditFromData()
    {
       
        int flag = LevelManager.Instance.DataDiffical[Controller.Instance.DiffirentGame];
        if (Level < flag)
        {
            Actived();
        }
        else if (Level == flag || Level == flag2 + 1){
            if(Level == Controller.Instance.constantsDiffical[Controller.Instance.DiffirentGame])
            {
                Actived();
            }
            else
            {
                Activing();
            }
        }
        else
        {
            AwaitActive();
        }
    }
    public void Actived()
    {
        imageBG.sprite = spiteBg[0];
        imageCup.sprite = spiteIcon[0];
        if (!imageButton.gameObject.activeInHierarchy)
        {
            imageButton.gameObject.SetActive(true);
        }
        imageButton.sprite = spiteButton[0];
        decripText.text = "Play Again";
    }
    public void Activing()
    {
        imageBG.sprite = spiteBg[1];
        imageCup.sprite = spiteIcon[1];
        imageButton.sprite = spiteButton[1];
        if (!imageButton.gameObject.activeInHierarchy)
        {
            imageButton.gameObject.SetActive(true);
        }
        decripText.text = "Play";
    }

    public void AwaitActive()
    {
        imageBG.sprite = spiteBg[2];
        imageCup.sprite = spiteIcon[2];
        if (imageButton.gameObject.activeInHierarchy)
        {
            imageButton.gameObject.SetActive(false);
        }
        decripText.text = "";
    }

    //private void OnDisable()
    //{
    //    AwaitActive();
    //}

}
