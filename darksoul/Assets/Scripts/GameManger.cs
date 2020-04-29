using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{
    bool isCursorLock = false;
    void CursorSet()
    {
        Cursor.visible = isCursorLock;
        isCursorLock = !isCursorLock;

        if(isCursorLock)
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    void Start()
    {
        CursorSet();
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            CursorSet();
        }
        if(Input.GetKeyDown(KeyCode.R) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("TutorialScene");
        }
        if (Input.GetKeyDown(KeyCode.T) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            SceneManager.LoadScene("GameScene");
        }

        if (Input.GetKeyDown(KeyCode.F11) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            FindObjectOfType<Player>().currentHp = 100000;
        }

        if (Input.GetKeyDown(KeyCode.F12) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            FindObjectOfType<Enemy>().currentHp = 20;
        }
        if (Input.GetKeyDown(KeyCode.UpArrow) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            Enemy.maxHp += 10;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            Enemy.maxHp -= 10;
        }
    }
}
