using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GuardUI : MonoBehaviour
{
    public static GuardUI UIinstance = null; // Ёкземпл€р объекта

    public Transform guest;
    public TMP_Text moneyCount;

    //Likes system
    public TMP_Text likesCountTxt;
    public TMP_Text dislikesCountTxt;
    public TMP_Text levelNumber;


    public GameObject[] buttons; //0 - play, 1-next, 2 -restart

    private void Awake()
    {
        if (UIinstance == null) { // Ёкземпл€р менеджера был найден
            UIinstance = this; // «адаем ссылку на экземпл€р объекта
        } else if (UIinstance == this)
        { // Ёкземпл€р объекта уже существует на сцене
            Destroy(gameObject); // ”дал€ем объект
        }

        // “еперь нам нужно указать, чтобы объект не уничтожалс€
        // при переходе на другую сцену игры
        // DontDestroyOnLoad(gameObject);

    }

    private void OnEnable()
    {
        GameActions.OnGameFailed += ShowRestart;
    }
    private void OnDisable()
    {
        GameActions.OnGameFailed -= ShowRestart;
    }
    private void Start()
    {
        ShowButton(0);
    }

    void ShowRestart()
    {

        foreach (GameObject go in buttons)
        {
            go.SetActive(false);
        }
        buttons[2].SetActive(true);
    }
    public void ShowButton(int buttonNum)
    {
        foreach (GameObject go in buttons)
        {
            go.SetActive(false);
        }
        buttons[buttonNum].SetActive(true);
    }
    public void HideAllButtons()
    {
        foreach (GameObject go in buttons)
        {
            go.SetActive(false);
        }
    }
    public void  UpdateLikesCount(int likes)
    {
         likesCountTxt.text = likes.ToString();
    }
    public void  UpdateDislikesCount(int disLikes)
    {
        dislikesCountTxt.text = disLikes.ToString();
    }

    public void UpdateMoney(string textToUpdate)
    {
        moneyCount.text = textToUpdate;
    }

   
}
