﻿using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Xml.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

using static UnityEngine.UISystemProfilerApi;
using Random = UnityEngine.Random;

public enum DirectionBlock
{
    Down,
    Up,
    Back,
    Forward,
    Left,
    Right,
}

public enum StatusBlock
{
    Die,
    Normal,
    Gift
}
public class Block : MonoBehaviour, IListenerBlock
{
    public float checkmax;
    public float duration = 2f;

    private RaycastHit hit;
    private DirectionBlock Direction;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private Vector3 startScale;  
    private float randomScale = 0;
    private Vector3 dir;
    private Vector3 dir2;

    private bool animationback = true;
    public GameObject ModelBlock;
    GameObject trail;
    public GameObject gift;

    public Renderer rendererSkin;
    private int flagSkin = 0;
   
    private StatusBlock statusBlock = StatusBlock.Normal;
    public StatusBlock StatusBlock
    {
        set { 
            statusBlock = value;
            if(statusBlock == StatusBlock.Gift)
            {
                gift = GiftPooling.Instance.GetGift();
                gift.transform.localScale = new Vector3(0, 0, 0);
                gift.transform.DOScale(0.5f, 1).OnComplete(() =>
                {
                    gift.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                });        
                gift.transform.parent = transform;
                gift.transform.rotation = transform.rotation;
                gift.transform.position = transform.position;              
                ModelBlock.SetActive(false);
            }
            else
            {
                if (!ModelBlock.activeInHierarchy)
                {
                    ModelBlock.SetActive(true);
                }
                if (gift != null)
                {
                    gift.SetActive(false);
                    gift.transform.parent = GiftPooling.Instance.transform;
                    gift = null;
                    if (!ModelBlock.activeInHierarchy)
                    {
                        ModelBlock.SetActive(true);
                    }
                }
            }
        }
        get {
            return statusBlock;
        }
    }

    private void OnEnable()
    {
        endPosition = transform.position;

        if (trail != null)
        {
            trail.SetActive(false);
            trail.transform.parent = TrailPooling.Instance.transform;
            trail = null;
        }

        if(gift != null && statusBlock == StatusBlock.Normal)
        {
            gift.SetActive(false);
            gift.transform.parent = GiftPooling.Instance.transform;
            gift = null;
            if (!ModelBlock.activeInHierarchy)
            {
                ModelBlock.SetActive(true);
            }
        }

        if(flagSkin != Controller.Instance.IndexSkin)
        {
            ISkin(LevelManager.Instance.skindata.GetItem(Controller.Instance.IndexSkin),Controller.Instance.IndexSkin);
        }
    }
   
    //kiem tra no la qua hay hop binh thuong
    public void checkRayInput()
    {
        if(statusBlock == StatusBlock.Normal)
        {
            checkRay();
        }
        else if (statusBlock == StatusBlock.Gift)
        {
            MoveGift();          
        }
    }

    private void MoveGift()
    {
        Controller.Instance.gameState = StateGame.AWAIT;
        //UIManager.Instance.GameUIIngame.UIGift.SetActive(true);
        UIManager.Instance.BlockGiftPresent = this;
        UIManager.Instance.UIBoom.SetActive(false);

        StartCoroutine(IMoveGift());
    }
    public IEnumerator IMoveGift()
    {
        transform.parent = LevelManager.Instance.temporarymain;
        QuadraticCurve.Instance.A.position = transform.position;
        QuadraticCurve.Instance.ClearPositionB();
        float sampleTime = 0f;
        transform.position = QuadraticCurve.Instance.evaluate(sampleTime);
        transform.DORotate(new Vector3(-40, 0, 0), 1f, RotateMode.FastBeyond360);
        while (sampleTime <= 1f)
        {
            sampleTime += Time.deltaTime;
            transform.position = QuadraticCurve.Instance.evaluate(sampleTime);
            //transform.forward = QuadraticCurve.Instance.evaluate(sampleTime + 0.001f) - transform.position;
            yield return null;
        }
        transform.position = QuadraticCurve.Instance.evaluate(1);
        UIManager.Instance.GameUIIngame.UIGift.SetActive(true);

        LevelManager.Instance.pretransform.localScale = new Vector3(0, 0, 0);
    }
    public void checkRay()
    {
        if (animationback == true)
        {
            checkDirection();
            if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, 1 << 6))
            {
                Block blockcheck = hit.collider.GetComponent<Block>();
                if (blockcheck.statusBlock == StatusBlock.Die)
                {
                    RunBlock();
                    return;
                }
                if (hit.distance < 1)
                {
                    RunAwaitBack(dir, dir2);
                }
                else
                {
                    RunAwait2(blockcheck);
                }
            }
            else
            {
                RunBlock();
            }
        }
    }
   

    public void RunAwaitBack(Vector3 dircheck, Vector3 animationdir)
    {

            if (animationback == true)
            {
                animationback = false;

                if (statusBlock == StatusBlock.Normal)
                {
                    Vector3 originalPosition = ModelBlock.transform.localPosition;
                    Vector3 targetPosition = originalPosition + animationdir * 0.2f;
                    ModelBlock.transform.DOLocalMove(targetPosition, 0.15f).OnComplete(() =>
                    {

                        if (Physics.Raycast(transform.position, dircheck, out hit, Mathf.Infinity, 1 << 6))
                        {
                            Block blockdir = hit.collider.GetComponent<Block>();
                            if (hit.distance < 1)
                            {
                                blockdir.RunAwaitBack(dircheck, animationdir);
                            }
                        }
                        ModelBlock.transform.DOLocalMove(originalPosition, 0.15f).OnComplete(() =>
                        {
                            animationback = true;
                        });
                    });
                }
                else
                {
                    Vector3 originalPosition = gift.transform.localPosition;
                    Vector3 targetPosition = originalPosition + animationdir * 0.2f;
                    gift.transform.DOLocalMove(targetPosition, 0.15f).OnComplete(() =>
                    {

                        if (Physics.Raycast(transform.position, dircheck, out hit, Mathf.Infinity, 1 << 6))
                        {
                            Block blockdir = hit.collider.GetComponent<Block>();
                            if (hit.distance < 1)
                            {
                                blockdir.RunAwaitBack(dircheck, animationdir);
                            }
                        }
                        gift.transform.DOLocalMove(originalPosition, 0.15f).OnComplete(() =>
                        {
                            animationback = true;
                        });
                    });
                }
            }         
    }

    public void RunAwait2(Block b)
    {   
        if (animationback == true)
        {
            animationback = false;
            Vector3 originalPosition = ModelBlock.transform.localPosition;

            Vector3 targetPosition = transform.InverseTransformPoint(b.transform.position) - dir2;
            ModelBlock.gameObject.transform.DOLocalMove(targetPosition, 0.3f).OnComplete(() =>
            {

                if (Physics.Raycast(transform.position, dir, out hit, Mathf.Infinity, 1 << 6))
                {
                    Block blockdir = hit.collider.GetComponent<Block>();
                    blockdir.RunAwaitBack(dir, dir2);                  
                }
                ModelBlock.gameObject.transform.DOLocalMove(originalPosition, 0.3f).OnComplete(() =>
                {
                    animationback = true;
                });
            });
        }
    }


    public void RunBlock()
    {
        statusBlock = StatusBlock.Die;
        checkDirection();
        transform.parent = LevelManager.Instance.temporarymain;

        LevelManager.Instance.CheckWin();
        StartCoroutine(Run(transform.TransformDirection(dir2)));
    }
    IEnumerator Run(Vector3 direction)
    {
        trail =  TrailPooling.Instance.GetTrail();
        if (trail != null)
        {
            trail.transform.position = transform.position;
            trail.transform.parent = transform;
            trail.SetActive(true);
        }


        for (float alpha = 20f; alpha >= 0; alpha -= 0.1f)
        {
            transform.Translate(direction * 10 * Time.deltaTime, Space.World);
            yield return null;
        }
        if(trail != null)
        {
            trail.SetActive(false);
            trail.transform.parent = TrailPooling.Instance.transform;
        }
        gameObject.SetActive(false);      
    }
    public void MoveBlock()
    {
        MoveToDestination();
    }
    public void MoveToDestination()
    {
        startRotation = transform.rotation;
        startScale = new Vector3(1,1,1);
        randomScale = Random.Range(0.5f, 2f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
        transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
        transform.DOMove(endPosition, 2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            Controller.Instance.Checkawaitload();
        });
        transform.DORotate(new Vector3(0, 360, 0), 2f, RotateMode.FastBeyond360).OnComplete(() =>
        {
            transform.rotation = startRotation;
        });
        transform.DOScale(startScale, 2f).OnComplete(() =>
        {
            transform.localScale = startScale;
        });
    }

    public void checkDirection()
    {
        switch (Direction)
        {
             case DirectionBlock.Left:
                dir = -transform.right;
                break;
             case DirectionBlock.Right:
                dir = transform.right;
          
                break;
             case DirectionBlock.Up:
                dir = transform.up;
               
                break;
             case DirectionBlock.Down:
                dir = -transform.up;
               
                break;
             case DirectionBlock.Forward:
                dir = transform.forward;
                
                break;
             case DirectionBlock.Back:
                dir = -transform.forward;
               
                break;
            default:
                dir = -transform.forward;
                break;
        }
    }
    public void GetDirectionBlock(int input)
    {
        switch (input)
        {
            case 1:
                Direction =  DirectionBlock.Down;
                dir2 = Vector3.down;
                ModelBlock.transform.eulerAngles = new Vector3(90, 0, 0);
                break;
            case 2:
                Direction = DirectionBlock.Up;
                dir2 = Vector3.up;
                ModelBlock.transform.eulerAngles = new Vector3(-90, 0, 0);
                break;
            case 3:
                Direction = DirectionBlock.Back;       
                dir2 = Vector3.back;
                ModelBlock.transform.eulerAngles = new Vector3(180, 0, 0);
                break;
            case 4:
                Direction = DirectionBlock.Forward;
                ModelBlock.transform.eulerAngles = new Vector3(0, 0, 0);
                dir2 = Vector3.forward;
                break;
            case 5:
                Direction = DirectionBlock.Left;              
                dir2 = Vector3.left;
                ModelBlock.transform.eulerAngles = new Vector3(0, -90, 0);
                break;
            case 6:
                Direction = DirectionBlock.Right;
                dir2 = Vector3.right;
                ModelBlock.transform.eulerAngles = new Vector3(0, 90, 0);
                break;
            default:
                //Direction = DirectionBlock.Right;          
                //dir2 = Vector3.right;
                break;
        }
    }
    public void Crack()
    {
        Vector3 direction = (transform.position - transform.parent.position).normalized;
        Vector3 targetPosition = transform.position + direction * 5;
        transform.position = targetPosition;
        startPosition = transform.position;
    }

    public void FlyBoomed()
    {
        //Vector3 direction = (transform.position - transform.parent.position).normalized;
        Vector3 direction = Random.insideUnitSphere.normalized;
        Vector3 targetPosition = transform.position + direction * 5;
        transform.parent = LevelManager.Instance.temporarymain;
        //Vector3 direction = (transform.position - transform.parent.position).normalized;
        //direction.y += Random.Range(3f, 5f); // Thêm thành phần theo chiều dọc
        //direction = direction.normalized;
        //Vector3 targetPosition = transform.position + direction * Random.Range(3f, 5f);
        Quaternion targetRotation = Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), Random.Range(0f, 360f));

        transform.DOMove(targetPosition, 2f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            gameObject.SetActive(false);
            LevelManager.Instance.CheckWin();
        });
        transform.DORotate(targetRotation.eulerAngles, 2f);
    }

    public int IType()
    {
        if(statusBlock== StatusBlock.Gift && gameObject.activeInHierarchy)
        {
            return 9;
        }
        if (!gameObject.activeInHierarchy || StatusBlock == StatusBlock.Die)
        {
            return -1;
        }
        else
        {
            return (int)Direction+1;
        }
    }

    public void ISkin(Material material, int index)
    {
        rendererSkin.material = material;
        flagSkin = index;
    }
}
