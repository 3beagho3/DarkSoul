using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    public string keyInput;

    public Image tutorialImage;
    public int tutorialIndex = 0;
    public Sprite[] tutorialTexts;
    public Sprite sucessText;

    void Start()
    {
        StartCoroutine(TutorialSet());
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) keyInput += "w";
        if (Input.GetKeyDown(KeyCode.A)) keyInput += "a";
        if (Input.GetKeyDown(KeyCode.S)) keyInput += "s";
        if (Input.GetKeyDown(KeyCode.D)) keyInput += "d";
        
        if (Input.GetKeyDown(KeyCode.Space)) keyInput += "space";

        if (Input.GetMouseButtonDown(0)) keyInput += "left";
        if (Input.GetMouseButtonDown(1)) keyInput += "right";

        if ((Input.GetAxis("Mouse X") > 0 || Input.GetAxis("Mouse X") < 0) && tutorialIndex.Equals(0)) keyInput += "rotate";
        if ((Input.GetKeyDown(KeyCode.LeftShift)) && tutorialIndex.Equals(2)) keyInput += "shift";
    }
    
    IEnumerator TutorialSet()
    {
        while (true)
        {
            if (tutorialIndex.Equals(0))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (keyInput.Contains("rotate"))
                {
                    yield return new WaitForSeconds(3f);
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            if (tutorialIndex.Equals(1))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (keyInput.Contains("w") && keyInput.Contains("a")
                && keyInput.Contains("s") && keyInput.Contains("d"))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(2))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (keyInput.Contains("w") && keyInput.Contains("a")
                && keyInput.Contains("s") && keyInput.Contains("d") && keyInput.Contains("shift"))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(3))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.A))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(4))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.D))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(5))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (Input.GetMouseButtonDown(0))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(6))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (Input.GetMouseButtonDown(1))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(7))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];

                if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0))
                {
                    tutorialImage.sprite = sucessText;
                    yield return new WaitForSeconds(3f);
                    keyInput = string.Empty;
                    tutorialIndex++;
                }
            }
            else if (tutorialIndex.Equals(8))
            {
                tutorialImage.sprite = tutorialTexts[tutorialIndex];
                yield return new WaitForSeconds(7f);
                SceneManager.LoadScene("GameScene");
            }

            yield return null;
        }
    }
}