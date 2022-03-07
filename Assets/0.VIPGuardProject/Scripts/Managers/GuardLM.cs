using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Cinemachine;
public class GuardLM : MonoBehaviour
{

    public static GuardLM LMinstance = null; // Экземпляр объекта
    public Transform levelStart;
    public Transform levelsParent;
    public Transform usedLevels;
    public GameObject currentLevel;

    public Transform guardSpawnPosition;
    public Transform guestSpawnPosition;
   

    public NavMeshAgent guestAgent;
    public GuardMovement guardMovementScript;
    public CinemachineVirtualCamera levelStartCM;

    public float delayBeforeStart;

    //  public GameObject[] levels;

    public int LoadedlevelID;

    public int currentLevelNum;

    public int levelIndex;

    public int priceModifier;

    public float agentSpeed;
    private void Awake()
    {
        if (LMinstance == null)
        { // Экземпляр менеджера был найден
            LMinstance = this; // Задаем ссылку на экземпляр объекта
        }
        else if (LMinstance == this)
        { // Экземпляр объекта уже существует на сцене
            Destroy(gameObject); // Удаляем объект
        }
    }


    private void OnEnable()
    {
        GameActions.OnGameCompleted += StageCompleted;
    }
    private void OnDisable()
    {
        GameActions.OnGameCompleted -= StageCompleted;
    }
    private void Start()
    {
        InitializePlayer();
        currentLevelNum = PlayerPrefs.GetInt("LevelNum",1);
        GuardUI.UIinstance.levelNumber.text = "LEVEL - " + currentLevelNum.ToString();
        priceModifier = (currentLevelNum + 10);
        levelIndex = PlayerPrefs.GetInt("LevelIndex", 0);
    }


    void StageCompleted()
    {
        
        currentLevelNum++;

       
        levelIndex++;
        //Loop to the first prefab in hierarchy
        if(levelIndex>= levelsParent.childCount)
        {
            levelIndex = 0;
        }
      //  Debug.Log("Completed" + "," + currentLevelNum.ToString() + "," + levelIndex.ToString());
        SaveProgress();
        TinySauce.OnGameFinished(true, GuardGM.instance.totalMoney, levelNumber: GuardLM.LMinstance.currentLevelNum.ToString());
    }


    void SaveProgress()
    {
        PlayerPrefs.SetInt("Money", GuardGM.instance.totalMoney);
        PlayerPrefs.SetInt("LevelNum", currentLevelNum);
        PlayerPrefs.SetInt("LevelIndex", levelIndex);
    }

    public void LoadLevel()
    {
        GuardUI.UIinstance.HideAllButtons();

        //Activate level prefab
        levelsParent.GetChild(levelIndex).gameObject.SetActive(true); 

        StartCoroutine(StartGame());
    }

    void InitializePlayer()
    {
        levelStartCM.Priority = 50;
        //Prepare guard
        guardMovementScript.gameObject.transform.position = guardSpawnPosition.position;
        guardMovementScript.gameObject.transform.rotation = guardSpawnPosition.rotation;
        guardMovementScript.stop = true;
        guardMovementScript.anim.SetBool("Waiting", true);

        //Prepare Guest
        guestAgent.speed = 0;
        guestAgent.gameObject.transform.position = guestSpawnPosition.position;
        guestAgent.gameObject.transform.rotation = guestSpawnPosition.rotation;
    }

    public IEnumerator StartGame()
    {
        levelStartCM.Priority = 0;
        yield return new WaitForSeconds(delayBeforeStart);
        guardMovementScript.stop = false;
        guardMovementScript.anim.SetBool("Waiting", false);
        guestAgent.speed = agentSpeed;
    }

    public void RestartLevelWithProgress()
    {
        //Fire an event
        GameActions.OnGameCompleted();
        SceneManager.LoadScene(0);

    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }







}
