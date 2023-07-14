using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using DG.Tweening;

[System.Serializable]
public class LevelData
{
    public int LevelID;
    public Vector3 statusID;
    public int[] arrayDir;
}
public class LevelManager : Singleton<LevelManager>
{
    public Transform pretransform;
    public Transform temporarymain;
    public GameObject PrefabsObject;

    public SkinData skindata;
    private int levelInt;
    public int LevelIDInt
    {
        get
        {
            return levelInt;
        }
        set
        {
            levelInt = value;
            if(levelInt >= 0 && levelInt <= Controller.Instance.constantsDiffical[DiffirentEnum.EASY]){
                if(levelInt > PlayerPrefs.GetInt("Easylevel")){
                    PlayerPrefs.SetInt("Easylevel", levelInt);
                    DataDiffical[DiffirentEnum.EASY] = levelInt;
                }
                
            }else if(levelInt >= 10 && levelInt <= Controller.Instance.constantsDiffical[DiffirentEnum.MEDIUM]){
                if(levelInt > PlayerPrefs.GetInt("Mediumlevel")){
                    PlayerPrefs.SetInt("Mediumlevel", levelInt);
                    DataDiffical[DiffirentEnum.MEDIUM] = levelInt;
                }

            }else if(levelInt>= 30 && levelInt <= Controller.Instance.constantsDiffical[DiffirentEnum.HARD]){
                if(levelInt > PlayerPrefs.GetInt("Hardlevel")){
                    PlayerPrefs.SetInt("Hardlevel", levelInt);
                    DataDiffical[DiffirentEnum.HARD] = levelInt;
                }
            }
            PlayerPrefs.SetInt("Playinglevel", levelInt);
        }
    }

    public Vector3 statusLevel;

    private string[] linesLevel;
    private string[] numbers;
    private int[] arraydata;
    private int flag = 0;
    private int flag2 = 0;
    public int[] arrayDir;
    private int childCountParent;

    //test
    // public int leasy = 4;
    // public int leamedium = 15;
    // public int leamehard = 30;

    private bool firstgame = true;
    public  Dictionary<DiffirentEnum, int> DataDiffical = new Dictionary<DiffirentEnum, int>()
    {
        { DiffirentEnum.EASY, 0 },
        { DiffirentEnum.MEDIUM, 0 },
        { DiffirentEnum.HARD, 0}
    };
    private void Awake()
    {
        DOTween.SetTweensCapacity(10000, 10000);
        
        DataDiffical[DiffirentEnum.EASY] = PlayerPrefs.GetInt("Easylevel");
        DataDiffical[DiffirentEnum.MEDIUM] = PlayerPrefs.GetInt("Mediumlevel");
        DataDiffical[DiffirentEnum.HARD] = PlayerPrefs.GetInt("Hardlevel");
    }
    private void Start()
    {
        LoaddataFromLocal();
        // //Edittext(linesLevel[LevelIDInt - 1]);
        // //pretransform.localPosition = new Vector3(statusLevel.x / 2, (float)statusLevel.y / 2, (float)statusLevel.z / 2);
        // //Camera.main.transform.position = new Vector3((float)statusLevel.x / 2, (float)statusLevel.y / 2, -10);
        // // Debug.Log((float)arrayxyz[0]+"=="+(float)arrayxyz[2]+"=="+ (float)arrayxyz[1]);
        // if (CheckSaveGame())
        // {
        //     Edittext(linesLevel[levelInt - 1]);
        //     pretransform.localPosition = new Vector3(statusLevel.x / 2, (float)statusLevel.y / 2, (float)statusLevel.z / 2);
        //     Camera.main.transform.position = new Vector3((float)statusLevel.x / 2, (float)statusLevel.y / 2+4, ReturnyCamera(levelInt));
        //     CreateMapToSave();
        // }
        // else
        // {
        //     LoadLevelInGame( Mathf.Clamp(PlayerPrefs.GetInt("Playinglevel"),1,1000));
        //     //CreateMap();
        // }
    }

    public void StartGame(){
        if(Controller.Instance.gameState == StateGame.WIN){
            gameObject.SetActive(false);
            return;
        }
        if(CheckObjectActive()){
            Controller.Instance.gameState = StateGame.PLAY;
        }else{
            //LoaddataFromLocal();
            //Edittext(linesLevel[LevelIDInt - 1]);
            //pretransform.localPosition = new Vector3(statusLevel.x / 2, (float)statusLevel.y / 2, (float)statusLevel.z / 2);
            //Camera.main.transform.position = new Vector3((float)statusLevel.x / 2, (float)statusLevel.y / 2, -10);
            // Debug.Log((float)arrayxyz[0]+"=="+(float)arrayxyz[2]+"=="+ (float)arrayxyz[1]);
            if (CheckSaveGame() && firstgame)
            {
                Edittext(linesLevel[levelInt - 1]);
                pretransform.localPosition = new Vector3(statusLevel.x / 2, (float)statusLevel.y / 2, (float)statusLevel.z / 2);
                Camera.main.transform.position = new Vector3((float)statusLevel.x / 2, (float)statusLevel.y / 2+4, ReturnyCamera(levelInt));
                CreateMapToSave();
            }
            else
            {
                int index = PlayerPrefs.GetInt("Playinglevel");
                if(firstgame){
                    LoadLevelInGame( Mathf.Clamp(index,1,300));
                }else{
                    int index2 = index+1;
                    LoadLevelInGame( Mathf.Clamp(index2,1,300));
                }
                //CreateMap();
            }
            
        }
        firstgame = false;
    }
    public void LoadLevelInGame(int level)
    {
        //an man hinh chon level
        if (UIManager.Instance.SelectLevelUI.activeInHierarchy)
        {
            UIManager.Instance.SelectLevelUI.SetActive(false);
        }
        //an tat ca cac object con tren man hinh
        SetAllFalse();
        Edittext(linesLevel[level - 1]);
        LevelIDInt = level;
        pretransform.localPosition = new Vector3(statusLevel.x / 2, (float)statusLevel.y / 2, (float)statusLevel.z / 2);
        Camera.main.transform.position = new Vector3((float)statusLevel.x / 2, (float)statusLevel.y / 2+4, ReturnyCamera(levelInt));
        CreateMap();      
    }
    public void LoaddataFromLocal()
    {
        TextAsset mapText = Resources.Load("level-normal") as TextAsset;
        if (mapText != null)
        {     
            ProcessGameDataFromString(mapText.text);        
        }
    }
    public void ProcessGameDataFromString(string mapText)
    {

        linesLevel = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);

    }

    private void Edittext(string inputString)
    {
        numbers = inputString.Split('|');
      
        arraydata = new int[numbers.Length - 3];

        //for (int i = 0; i < 3; i++)
        //{
        //    arrayxyz[i] = int.Parse(numbers[i]);
        //}
        statusLevel = new Vector3(int.Parse(numbers[1]), int.Parse(numbers[2]), int.Parse(numbers[0]));

        for (int i = 3; i < numbers.Length; i++)
        {
            arraydata[i - 3] = int.Parse(numbers[i]);
        }
    }

    public void CreateMap()
    {
        Controller.Instance.ListenerBlock.Clear();
        Controller.Instance.amountNumberBlock = 0;
        Controller.Instance.gameState = StateGame.AWAITLOAD;
        Controller.Instance.checkloadBlock = 0;
        pretransform.rotation = Quaternion.identity;
        flag = 0;
        childCountParent = pretransform.childCount;
        int m = 0;

        for (int i = 0; i < statusLevel.y; i++)
        {
            for (int j = 0; j < statusLevel.x; j++)
            {
                for (int g = 0; g < statusLevel.z; g++)
                {
                    if (arraydata[flag] != -1)
                    {
                        GameObject block;
                        if (m <= childCountParent - 1)
                        {
                           
                            block = pretransform.GetChild(m).gameObject;
                            block.transform.position = new Vector3(j + 0.5f, i + 0.5f, g + 0.5f);
                            block.transform.rotation = Quaternion.identity;
                            m++;
                            block.SetActive(true);
                            
                        }
                        else
                        {
                            block = Instantiate(PrefabsObject, new Vector3(j + 0.5f, i + 0.5f, g + 0.5f), Quaternion.identity, pretransform);                    
                        }
                        Block blockscript = block.GetComponent<Block>();
                        blockscript.GetDirectionBlock(arraydata[flag]);
                        blockscript.Crack();
                        blockscript.MoveBlock();
                        blockscript.StatusBlock = StatusBlock.Normal;
                        AddListenerBlock(blockscript);

                        Controller.Instance.amountNumberBlock++;
                    }
                    
                    flag++;
                }
            }

        }
    }

    public void CreateMapToSave()
    {
        //Debug.Log(statusLevel.x+"load truong hoop 2" + statusLevel.y);
        flag = 0;
        flag2 = 0;
        Controller.Instance.ListenerBlock.Clear();
        Controller.Instance.amountNumberBlock = 0;
        Controller.Instance.checkloadBlock = 0;
        Controller.Instance.gameState = StateGame.AWAITLOAD;

        for (int i = 0; i < statusLevel.y; i++)
        {
            for (int j = 0; j < statusLevel.x; j++)
            {
                for (int g = 0; g < statusLevel.z; g++)
                {
                    if (arraydata[flag] != -1)
                    {
                        GameObject block = Instantiate(PrefabsObject, new Vector3(j + 0.5f, i + 0.5f, g + 0.5f), Quaternion.identity, pretransform);
                        Block blockscript = block.GetComponent<Block>();
                        int arrayflag = arrayDir[flag2];
                        if (arrayflag != -1)
                        {
                            if (arrayflag==9)
                            {
                                blockscript.StatusBlock = StatusBlock.Gift;
                            }
                            else
                            {
                                blockscript.GetDirectionBlock(arrayflag);
                            }
                            blockscript.Crack();
                            blockscript.MoveBlock();
                            Controller.Instance.amountNumberBlock ++;
                        }
                        else
                        {
                            
                            blockscript.GetDirectionBlock(arraydata[flag]);
                            blockscript.Crack();
                            blockscript.MoveBlock();
                            block.SetActive(false);
                        }
                        AddListenerBlock(blockscript);
                        flag2++;
                    }
                    flag++;
                }
            }

        }
    }
    private void AddListenerBlock(Block block)
    {
        if (!Controller.Instance.ListenerBlock.Contains(block))
        {
            Controller.Instance.ListenerBlock.Add(block);
        }
    }

    public void SaveGame(int[] arraydir)
    {
        LevelData levelData = new LevelData();
        levelData.LevelID = levelInt;
        levelData.statusID = statusLevel;
        levelData.arrayDir = arraydir;

        string directoryPath = Path.Combine(Application.persistentDataPath, "Resources");

        string filePath = Path.Combine(directoryPath, "level.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }



        //string file = Application.dataPath + "/Data/level.json";
        if (levelData.arrayDir.Length > 0)
        {
            string json = JsonUtility.ToJson(levelData);
           
            File.WriteAllText(filePath, json);
           // Debug.Log("luu phai"+filePath);
            //File.WriteAllText(file, json);
            //Debug.Log(file);
        }
        else
        {
           // Debug.Log("khong luu dc");
            ClearDataSaveGame();
        }
        //Debug.Log("??"+ arraydir.Length);
    }
    //void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
    //    {
    //        int i = 0;
    //        int[] arraytest = new int[Controller.Instance.ListenerBlock.Count];

    //        // kiem tra xem mang co toan -1 hay khong
    //        bool checkminus1 = false;
    //        foreach (var item in Controller.Instance.ListenerBlock)
    //        {
    //            arraytest[i] = item.IType();
    //            if (arraytest[i] != -1 && !checkminus1)
    //            {
    //                checkminus1 = true;
    //            }
    //            i++;
    //        }
    //        if (checkminus1)
    //        {
    //            LevelManager.Instance.SaveGame(arraytest);
    //           // Debug.Log("luu thanhc ong cnayh");
    //        }
    //        else
    //        {
    //          //  Debug.Log("xoa du hieu xong");
    //            LevelManager.Instance.ClearDataSaveGame();
    //        }
    //    }
    //}

    public void ClearDataSaveGame()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Resources");

        string filePath = Path.Combine(directoryPath, "level.json");

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        File.WriteAllText(filePath, "");
    }

    public bool CheckSaveGame()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, "Resources");

        string filePath = Path.Combine(directoryPath, "level.json");
        if (!File.Exists(filePath))
        {
            return false;
        }
        
       
        string json = File.ReadAllText(filePath);
        if (json == "" || json == null)
        {
           // Debug.Log(filePath+"ko co giu lieu "+ json);
            return false;
        }
        else
        {
           // Debug.Log("co du lieu");
            LevelData leveldata = JsonUtility.FromJson<LevelData>(json);
            LevelIDInt = leveldata.LevelID;
            statusLevel = leveldata.statusID;
            arrayDir = leveldata.arrayDir;
            return true;
        }
    }

    public void SetAllFalse()
    {
        //for(int i = 0; i < temporarymain.childCount; i++) {
        //    temporarymain.GetChild(i).parent = pretransform;
        //    temporarymain.GetChild(i).gameObject.SetActive(false);
        //    Debug.Log(i);
        //}
        //foreach (Transform child in temporarymain)
        //{
        //    child.gameObject.SetActive(false);
        //    child.parent = pretransform;
        //}
        List<Transform> childObjects = new List<Transform>();

        foreach (Transform child in temporarymain)
        {
            childObjects.Add(child);
        }
        foreach (Transform child in childObjects)
        {
            child.SetParent(pretransform);
        }
        foreach (Transform child in pretransform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void CheckWin()
    {
        for (int i = 0; i < pretransform.childCount; i++)
        {
            GameObject g = pretransform.GetChild(i).gameObject;
            if (g.activeInHierarchy && g.GetComponent<Block>().StatusBlock != StatusBlock.Die)
            {
                return;
            }
        }
        Controller.Instance.gameState = StateGame.WIN;
    }

    public bool CheckObjectActive(){
        for (int i = 0; i < pretransform.childCount; i++)
        {
            GameObject g = pretransform.GetChild(i).gameObject;
            if (g.activeInHierarchy && g.GetComponent<Block>().StatusBlock != StatusBlock.Die)
            {
                return true;
            }
        }
        return false;
    }

    public void NextLevel()
    {
        LevelIDInt++;
        //PlayerPrefs.SetInt("Playinglevel", levelInt);
        LoadLevelInGame(levelInt);
    }

    public GameObject GetRandomActiveChild()
    {
        List<GameObject> activeChildren = new List<GameObject>();

        foreach (Transform child in pretransform.transform)
        {
            if (child.gameObject.activeInHierarchy && child.gameObject.GetComponent<Block>().StatusBlock == StatusBlock.Normal)
            {
                activeChildren.Add(child.gameObject);
            }
        }

        if (activeChildren.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeChildren.Count);
            return activeChildren[randomIndex];
        }

        return null;
    }

    public GameObject GetRandomGift()
    {
        List<GameObject> activeChildren = new List<GameObject>();

        foreach (Transform child in pretransform.transform)
        {
            if (child.gameObject.activeInHierarchy && child.gameObject.GetComponent<Block>().gift == null)
            {
                activeChildren.Add(child.gameObject);
            }
        }

        if (activeChildren.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, activeChildren.Count);
            return activeChildren[randomIndex];
        }

        return null;
    }

    public float ReturnyCamera(int index)
    {
        if(index <= 5)
        {
            return -13.5f;
        }
        else if (index < 20)
        {
            return -15;
        }else if(index < 120)
        {
            return -16.5f;
        }
        else
        {
            return -23;
        }
    }
}
