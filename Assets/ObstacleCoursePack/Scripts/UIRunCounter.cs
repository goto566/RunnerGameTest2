using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIRunCounter : MonoBehaviour
{
    CanvasGroup canvasGroup;
    public TextMeshProUGUI txtCounter;

    private void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameStartTimer > 0)
        {
            if (canvasGroup.alpha == 0)
                canvasGroup.alpha = 1;
            txtCounter.text = (int)GameManager.instance.gameStartTimer + "";
        }
        else if (canvasGroup.alpha > 0)
            canvasGroup.alpha = 0;


    }
}
