using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public bool m_gameStarted = false;
    public float gameStartTimer = 5;


    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Start is called before the first frame update
    void Start()
    {
        m_gameStarted = false;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        if (!m_gameStarted)
        {
            if (gameStartTimer > 0)
                gameStartTimer -= Time.deltaTime;
            else
            {
                gameStartTimer = 0;
                m_gameStarted = true;
            }
        }
    }
}
