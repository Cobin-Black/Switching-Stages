using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    #region Level Select Menu
    public void GoToLevelSelectMenu()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    public void Level1()
    {
        SceneManager.LoadScene("Level1");
    }

    public void Level2()
    {
        SceneManager.LoadScene("Level2");
    }

    public void Level3()
    {
        SceneManager.LoadScene("Level3");
    }

    public void Level4()
    {
        SceneManager.LoadScene("Level4");
    }

    public void Level5()
    {
        SceneManager.LoadScene("Level5");
    }

    public void Level6()
    {
        SceneManager.LoadScene("Level6");
    }

    public void Level7()
    {
        SceneManager.LoadScene("Level7");
    }

    public void Level8()
    {
        SceneManager.LoadScene("Level8");
    }

    public void Level9()
    {
        SceneManager.LoadScene("Level9");
    }

    public void Level10()
    {
        SceneManager.LoadScene("Level10");
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }
}
