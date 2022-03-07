using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using TMPro;

public class GuardGM : MonoBehaviour
{
    public static GuardGM instance = null; // Экземпляр объекта

    public Transform guest;
    public Transform guard;

    public int likes;
    public int dislikes;
    public int totalMoney;


    public float initTime;

    //Photo 
    public int photoCount;
    public int maxPhotoCount;

    //FeedBack
   [SerializeField] MMFeedbacks moneyPopUp;
   public MMFeedbacks pointsPopUp;
   public MMFeedbacks likesPopUp;
   public MMFeedbacks ChangePoseFeedBack;
   public MMFeedbacks FallFeedBack;
   public MMFeedbacks TurdFeedBack;
   public MMFeedbacks WaterSplashFeedBack;
   public MMFeedbacks earnMoneyFromLike;
   public MMFeedbacks lostMoneyFromDisLike;
   public MMFeedbacks lostDislikes;


    //FX
    public GameObject likePrefab;
    public GameObject dislikePrefab;

    public Vector3 fxOffset;
    public Vector3 bodyOffset;

    public float likesLifeTime;
    public float delayBTWpopUp;
    public float updateDelay;

   
    //Vehicles
    public Transform[] vehicles; //0 - BMX, 1-FIAT, 2-PEUGEOUT, 3-RENAULT, 4-PRIUS, 5-LAMBO, 6-MAYBACH 7 - Helicopter
    public int[] vehiclePrices;
    public TMP_Text[] vehiclePrice;
    public int interval;
    private void Awake()
    {
        if (instance == null)
        { // Экземпляр менеджера был найден
            instance = this; // Задаем ссылку на экземпляр объекта
        } else if (instance == this)
        { // Экземпляр объекта уже существует на сцене
            Destroy(gameObject); // Удаляем объект
        }
    }
    private void OnEnable()
    {
        GameActions.OnFanKicked += DestroyHater;
    }
    private void OnDisable()
    {
        GameActions.OnFanKicked -= DestroyHater;
    }
    private void Start()
    {
        StartCoroutine(InitManagers());
    }

    private void Update()
    {
        if(Time.frameCount % interval == 0)
        {
            if (photoCount >= maxPhotoCount)
            {
                photoCount = 0;
                GuestInteraction GI = FindObjectOfType<GuestInteraction>();
                GI.EndPhotoSession();
            }
        }
       
    }
    IEnumerator InitManagers()
    {
        TinySauce.OnGameStarted(levelNumber: GuardLM.LMinstance.currentLevelNum.ToString());

        yield return new WaitForSeconds(initTime);
        totalMoney = PlayerPrefs.GetInt("Money", 0);
        AddMoney(totalMoney);
        likes = 0;
        GuardUI.UIinstance.UpdateLikesCount(likes);

        for (int i = 0; i < vehiclePrices.Length; i++)
        {
            vehiclePrices[i] += vehiclePrices[i] * GuardLM.LMinstance.priceModifier /100;
        }

        vehiclePrice[0].text = vehiclePrices[0].ToString() + "$";
        vehiclePrice[1].text = vehiclePrices[1].ToString() + "$";
        vehiclePrice[2].text = vehiclePrices[2].ToString() + "$";
        vehiclePrice[3].text = vehiclePrices[3].ToString() + "$";
        vehiclePrice[4].text = vehiclePrices[4].ToString() + "$";
        vehiclePrice[5].text = vehiclePrices[5].ToString() + "$";
        vehiclePrice[6].text = vehiclePrices[6].ToString() + "$";
        vehiclePrice[7].text = vehiclePrices[7].ToString() + "$";
    }

    #region InGame
    public void AddPhoto()
    {
        photoCount++;
    }
    public IEnumerator AddLikes(int likeCount, GameObject likePrefab, Vector3 likePosition, Vector3 fxOffset, float fxLifeTime)
    {
        for (int i = 0; i < likeCount; i++)
        {
            likes++;

            GameObject LikeFX = Instantiate(likePrefab, likePosition + new Vector3(0f, 0.25f, Random.Range(-0.75f, -1f)), Quaternion.identity);
            Destroy(LikeFX, fxLifeTime);
            GuardUI.UIinstance.UpdateLikesCount(likes);
            yield return new WaitForSeconds(updateDelay);
        }
    }
    public IEnumerator AddDislikes(int dislikeCount, GameObject dislikePrefab, Vector3 dislikePosition, Vector3 fxOffset, float fxLifeTime)
    {
        for (int i = 0; i < dislikeCount; i++)
        {
            dislikes++;
            GameObject DisLikeFX = Instantiate(dislikePrefab, dislikePosition, Quaternion.identity);
            Destroy(DisLikeFX, fxLifeTime);
            GuardUI.UIinstance.UpdateDislikesCount(dislikes);
            yield return new WaitForSeconds(updateDelay);
        }
    }
    public void AddMoney(int money)
    {
        totalMoney += money;
        GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
    }
    public void EarnMoney(int lostMoneyCount)
    {
        totalMoney += lostMoneyCount;
        GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());

        likes--;
        GuardUI.UIinstance.UpdateLikesCount(likes);

        earnMoneyFromLike.PlayFeedbacks();
    }
    public void LostMoney(int lostMoneyCount)
    {
        totalMoney -= lostMoneyCount;
        GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());

        dislikes--;
        GuardUI.UIinstance.UpdateDislikesCount(dislikes);

        lostMoneyFromDisLike.PlayFeedbacks();
    }
    public void ExtraMoney(int extraMoney)
    {
        AddMoney(extraMoney);
        moneyPopUp.PlayFeedbacks();
    }
    #endregion InGame


    public void MakePhoto()
    {
        GameActions.OnPhotographMakePhoto();
        AddPhoto();
    }
    public IEnumerator FinishGame()
    {
        //If you have less likes then dislikes = you FAIL
        if (likes < dislikes)
        {
            GameOver(false);
        }//If you have more likes then dislikes = Count how much Money you will have
        else
        {
            //Count how much money you will have?
            int temoLikes = likes;
            for (int i = 0; i < temoLikes; i++)
            {
                EarnMoney(300);
                yield return new WaitForSeconds(0.025f);
            }
            //Count how much money you lose
            yield return new WaitForSeconds(0.025f * likes);
            int temoDisLikes = dislikes;
            for (int i = 0; i < temoDisLikes; i++)
            {
                LostMoney(300);
                yield return new WaitForSeconds(0.025f);
            }
            yield return new WaitForSeconds(0.025f * dislikes);
            PickUpAVehicle();

           
        }
       
    }
    void PickUpAVehicle()
    {
        AgentFolower af = guest.GetComponent<AgentFolower>();
        Animator anim = guest.GetComponent<Animator>();
        af.runToACar = true;
        anim.SetBool("Run", true);
        //SetTarget
        if (totalMoney > vehiclePrices[7])
        {
            //Debug.Log("MB");
          af = guest.GetComponent<AgentFolower>();
           
            af.vehicleTarget = vehicles[7].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[7];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if(totalMoney > vehiclePrices[6])
        {
            //Debug.Log("MB");
             af = guest.GetComponent<AgentFolower>();
           
            af.vehicleTarget = vehicles[6].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[6];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if((totalMoney > vehiclePrices[5]))
        {
            af = guest.GetComponent<AgentFolower>();
          
            af.vehicleTarget = vehicles[5].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[5];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if ((totalMoney > vehiclePrices[4]))
        {
          
            af.vehicleTarget = vehicles[4].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[4];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if ((totalMoney > vehiclePrices[3]))
        {
           
            af.vehicleTarget = vehicles[3].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[3];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if ((totalMoney > vehiclePrices[2]))
        {
         
            af.vehicleTarget = vehicles[2].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[2];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if ((totalMoney > vehiclePrices[1]))
        {
           
            af.vehicleTarget = vehicles[1].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[1];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }
        else if ((totalMoney > vehiclePrices[0]))
        {
            
            af.vehicleTarget = vehicles[0].transform;
            af.followGuard = false;
            totalMoney -= vehiclePrices[0];
            GuardUI.UIinstance.UpdateMoney(totalMoney.ToString());
            return;
        }

       
    }
    public void GameOver(bool catched)
    {
        //Fire an event
        GameActions.OnGameFailed();
       
        TinySauce.OnGameFinished(false, totalMoney, levelNumber: GuardLM.LMinstance.currentLevelNum.ToString());
    }

    public void DestroyHater()
    {
            StartCoroutine(DecreaseDislikes(5));
    }
    public IEnumerator DecreaseDislikes(int ammount)
    {
        for (int i = 0; i < ammount; i++)
        {
            if (dislikes > 0)
            {
                dislikes--;
                GuardUI.UIinstance.UpdateDislikesCount(dislikes);
                lostDislikes.PlayFeedbacks();
            }
            yield return new WaitForSeconds(0.075f);
        }
    }
    
}
