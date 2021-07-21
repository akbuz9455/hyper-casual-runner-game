using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{

    public static LevelController Current;
    public bool gameActive = false;
    public GameObject startMenu, gameMenu, gameOverMenu, finishMenu;
    public Slider levelProgressBar;
    public float maxDistance;//prograssbar ile yol arasini eşitlemek istiyoruz
    public GameObject finishLine;//bitiş çizgisini tutarız
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText;
    int currentLevel;
    int score;
    void Start()
    {
        Current = this;
         currentLevel = PlayerPrefs.GetInt("currentLevel");

        if (SceneManager.GetActiveScene().name!="Level "+currentLevel)
        {
            SceneManager.LoadScene("Level " + currentLevel);
        }
        else
        {
            currentLevelText.text = (currentLevel + 1).ToString();
            nextLevelText.text = (currentLevel + 2).ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameActive)
        {
            playerController player = playerController.Current;
            float distance = finishLine.transform.position.z - playerController.Current.transform.position.z;
            levelProgressBar.value = 1 - (distance/maxDistance); //bitiş noktasına ne kadar yakınsa bölüm 0a yaklaşır
        }
    }

    public void StartLevel()
    {
        //harika bi kod
        maxDistance = finishLine.transform.position.z - playerController.Current.transform.position.z;
        //finişle başlangıç arası uzakliği cebe atarız

        //

        playerController.Current.ChangeSpeed(playerController.Current.runningSpeed);
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        playerController.Current.animator.SetBool("running",true);
        gameActive = true;
    }


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }
    public void LoadNextLevel()
    {
        if (currentLevel==2)
        {
            SceneManager.LoadScene("Level 0");
        }
        else
        {
            SceneManager.LoadScene("Level " + (currentLevel + 1));
        }
      


    }


    public void GameOver()
    {

        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;
    }

    public void FinishGame()
    {
       
        PlayerPrefs.SetInt("currentLevel",currentLevel+1);
        finishScoreText.text = score.ToString();
        gameMenu.SetActive(false);
        finishMenu.SetActive(true);
        gameActive = false;

    }

    public void ChangeScore(int increment)
    {

        score += increment;
        scoreText.text = score.ToString();
    }
}
