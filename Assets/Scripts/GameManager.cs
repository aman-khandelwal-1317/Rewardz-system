using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class GameManager : MonoBehaviour
{
   
    public List<Button> btns = new List<Button>();
    public List<int> selectedIndex = new List<int>();

    [SerializeField] private Sprite btnBlueSprite;
    [SerializeField] private Sprite btnGreenSprite;
    [SerializeField] private Sprite[] btnObjectSprites;
    [SerializeField] private Rewards rewardDB;
    [SerializeField] private GameObject panelWrong;
    [SerializeField] private GameObject panelNum;
    [SerializeField] private GameObject imgBtbn;
    [SerializeField] private GameObject rewardPrefab;
    [SerializeField] private GameObject rewardContainer;
    [SerializeField] private Text infoText;

    private int currentLevel = 1;
    private List<GameObject> rewardList = new List<GameObject>();
  

    public void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Btn");
        
        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
        }
    }

   
    public void AddListeners()
    {
        foreach(Button btn in btns) {
            btn.onClick.AddListener(() => OnBtnClick(btn.name));
        }
    }

    public int getRandomIndex()
    {
        return UnityEngine.Random.Range(0, rewardDB.rewards.Length-1);
    }


    public void OnBtnClick(string index)
    {
        int val;
        int.TryParse(index, out val);
        currentLevel++;

        int[] indexes = new int[4];

       
        for(int i = 0; i < btns.Count;i++)
        {

                btns[i].interactable = false;

            int rIndex = getRandomIndex();

            btns[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            btns[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = rewardDB.rewards[rIndex].rSprite;
            indexes[i] = rIndex;
        }


        GameObject rImg = Instantiate(rewardPrefab);
        rImg.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = rewardDB.rewards[indexes[val]].rSprite;
        rImg.name = currentLevel.ToString();

        rImg.transform.SetParent(rewardContainer.transform, false);

        rewardList.Add(rImg);



        if (rewardDB.rewards[indexes[val]].rType == "bomb" )
        {
            panelWrong.SetActive(true);

            for(int i = 0; i < rewardList.Count-1; i++)
            {
                rewardList[i].transform.SetParent(panelWrong.transform.GetChild(4).transform, false);
            }

        } else
        {
            StartCoroutine(Reload());
        }

      
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(2f);

        Destroy(panelNum.gameObject.transform.GetChild(0).gameObject);

        GameObject img = Instantiate(imgBtbn);
        img.name = (currentLevel + 7).ToString();
        img.GetComponentInChildren<Text>().text = img.name;
       img.transform.SetParent(panelNum.transform, false);

        infoText.text = currentLevel.ToString();


        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].interactable = true;
            btns[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }

    }

    public void ResetGame()
    {
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        GetButtons();
        AddListeners();
    }

}
