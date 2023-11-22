using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private GameObject panelReward;
    [SerializeField] private GameObject btnLeave;
    [SerializeField] private Text infoText;
    [SerializeField] private Text coinText;
    [SerializeField] private Text gemText;

    private int currentLevel = 1;
    private int currentCount = 0;
    private List<GameObject> rewardList = new List<GameObject>();

    private int num_of_Coins;
    private int num_of_Gems;

    private List<Reward> rewardsPending = new List<Reward>();

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
        return Random.Range(0, rewardDB.rewards.Length-1);
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

            if(currentLevel == 4)
            {
                btnLeave.SetActive(true);
            } 
           
            if(currentLevel < 4 && rewardDB.rewards[rIndex].rType == "bomb" || (currentLevel-1) % 5 == 0 && rewardDB.rewards[rIndex].rType == "bomb")
            {
                while (rewardDB.rewards[rIndex].rType == "bomb")
                {
                    rIndex = getRandomIndex();
                }
            }
           

            btns[i].gameObject.transform.GetChild(0).gameObject.SetActive(true);
            btns[i].gameObject.transform.GetChild(0).GetComponent<Image>().sprite = rewardDB.rewards[rIndex].rSprite;

            indexes[i] = rIndex;
        }

        int flag = 0;

        foreach(Reward r in rewardsPending)
        {
            if(r.rQuantity != 0 && !(r.rType == "chest") && r.rType == rewardDB.rewards[indexes[val]].rType)
            {
                r.rQuantity++;
                flag = 1;

               if(r.rType == "coin")
                {
                    rewardDB.rewards[indexes[val]].rAmount = Random.Range(100, 500);
                    r.rAmount += rewardDB.rewards[indexes[val]].rAmount;
                }

                    foreach (GameObject gO in rewardList)
                {
                    if(r.rType == "coin" && gO.name == r.rNum.ToString())
                    {
                        gO.GetComponentInChildren<Text>().text = r.rAmount.ToString();
                    } else if(gO.name == r.rNum.ToString())
                    {
                        gO.GetComponentInChildren<Text>().text = "X " + r.rQuantity.ToString();
                    }
                }

                break;
            }
            
        }

       
        if(flag == 0)
        {
            rewardsPending.Add(rewardDB.rewards[indexes[val]]);
            rewardsPending[rewardsPending.Count - 1].rQuantity++;

         

            GameObject rImg = Instantiate(rewardPrefab);

            rImg.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = rewardsPending[rewardsPending.Count - 1].rSprite;
            rImg.name = rewardsPending[rewardsPending.Count - 1].rNum.ToString();

            if (rewardDB.rewards[indexes[val]].rType == "coin")
            {
                rewardDB.rewards[indexes[val]].rAmount = Random.Range(100, 500);
                rImg.gameObject.transform.GetChild(1).GetComponent<Text>().text = rewardDB.rewards[indexes[val]].rAmount.ToString();
            }

            rImg.transform.SetParent(rewardContainer.transform, false);
            rewardList.Add(rImg);
        }

       

        if (rewardDB.rewards[indexes[val]].rType == "bomb" )
        {
            panelWrong.SetActive(true);

            for(int i = 0; i < rewardList.Count-1; i++)
            {

                GameObject gO = Instantiate(rewardList[i]);
                gO.transform.SetParent(panelWrong.transform.GetChild(4).transform, false);
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

    IEnumerator ResetGame()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(0);
    }

    private void Start()
    {
        GetButtons();
        AddListeners();
        UpdateQuantity();

        num_of_Coins = PlayerPrefs.GetInt("num_of_Coins", 0);
        num_of_Gems = PlayerPrefs.GetInt("num_of_Gems", 0);

        coinText.text = num_of_Coins.ToString();
        gemText.text = num_of_Gems.ToString();
    }

    private void UpdateQuantity()
    {
        foreach(Reward r in rewardDB.rewards)
        {
            r.rQuantity = 0;
        }
    }


    public void LeaveWithRewards()
    {
       foreach(Reward reward in rewardsPending)
        {
          
        }

        foreach (Transform child in rewardContainer.transform) { Destroy(child.gameObject); }

        rewardsPending.Clear();
        UpdateQuantity();
        rewardList.Clear();
    }

    public void GiveUp()
    {
        foreach (Transform child in rewardContainer.transform) { Destroy(child.gameObject); }

        rewardsPending.Clear();
        rewardList.Clear();
        UpdateQuantity();
        panelWrong.SetActive(false);
        currentLevel = 1;
    }

    public void OnLeave()
    {
      
            panelReward.transform.GetChild(2).GetComponent<Image>().sprite = rewardsPending[0].rSprite;
            panelReward.transform.GetChild(3).GetComponent<Text>().text = rewardsPending[0].rAmount.ToString();
            panelReward.transform.GetChild(4).GetComponent<Text>().text = rewardsPending[0].rType.ToString();
        panelReward.transform.GetChild(5).GetComponent<Image>().sprite = rewardsPending[0].rSprite;

        if (  rewardsPending[0].rType == "chest")
        {
            panelReward.transform.GetChild(panelReward.transform.childCount - 1).GetComponentInChildren<Text>().text = "Open";
        }
    }

    public void OnRewardClaim()
    {

       
        if (rewardsPending[currentCount].rType == "coin")
        {
            num_of_Coins = PlayerPrefs.GetInt("num_of_Coins", 0) + (rewardsPending[currentCount].rAmount * rewardsPending[currentCount].rQuantity);
            PlayerPrefs.SetInt("num_of_Coins", num_of_Coins);
            coinText.text = num_of_Coins.ToString();
            currentCount++;

            StartCoroutine(WaitforAnimation());
            


        }
        else if (rewardsPending[currentCount].rType == "gem")
        {
            Debug.Log(rewardsPending[currentCount].rType);
            Debug.Log(PlayerPrefs.GetInt("num_of_Gems", 0));
           
            num_of_Gems = PlayerPrefs.GetInt("num_of_Gems", 0) + (rewardsPending[currentCount].rAmount * rewardsPending[currentCount].rQuantity);
            Debug.Log(num_of_Gems);
            PlayerPrefs.SetInt("num_of_Gems", num_of_Gems);
            gemText.text =  num_of_Gems.ToString();
            currentCount++;

            StartCoroutine(WaitforAnimation());

        } else if (rewardsPending[currentCount].rType == "chest")
        {

            
            Reward r;
            r = rewardDB.rewards[getRandomIndex()];

            while (!(r.rType != "bomb" && r.rType != "chest"))
            {
                r = rewardDB.rewards[getRandomIndex()];
            }

            rewardsPending[currentCount] = r;
            rewardsPending[currentCount].rQuantity = 1;

            if(r.rType == "coin")
            {
                rewardsPending[currentCount].rAmount = Random.Range(100, 500);
            }

        }


        // StartCoroutine(WaitforAnimation());


        

        if (currentCount < rewardsPending.Count)
        {
            StartCoroutine(WaitForTime());

        }
        else
        {

            StartCoroutine(ResetGame());
        }

    }

    IEnumerator WaitForTime()
    {
        yield return new WaitForSeconds(0.5f);

            panelReward.transform.GetChild(2).GetComponent<Image>().sprite = rewardsPending[currentCount].rSprite;
            panelReward.transform.GetChild(5).GetComponent<Image>().sprite = rewardsPending[currentCount].rSprite;


            panelReward.transform.GetChild(3).GetComponent<Text>().text = rewardsPending[currentCount].rAmount.ToString();
            panelReward.transform.GetChild(4).GetComponent<Text>().text = rewardsPending[currentCount].rType.ToString();

            if (rewardsPending[currentCount].rType == "chest")
            {
                panelReward.transform.GetChild(panelReward.transform.childCount - 1).GetComponentInChildren<Text>().text = "Open";
            }
            else
            {
                panelReward.transform.GetChild(panelReward.transform.childCount - 1).GetComponentInChildren<Text>().text = "Claim";

            }

        }


    IEnumerator WaitforAnimation()
    {
        panelReward.transform.GetChild(5).gameObject.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        panelReward.transform.GetChild(5).gameObject.SetActive(false);
    }


    public void PlayOn()
    {
        panelWrong.SetActive(false);

        for (int i = 0; i < btns.Count; i++)
        {
            btns[i].interactable = true;
            btns[i].gameObject.transform.GetChild(0).gameObject.SetActive(false);
            
        }

        Destroy(rewardContainer.transform.GetChild(rewardContainer.transform.childCount - 1).gameObject);
    }

}
