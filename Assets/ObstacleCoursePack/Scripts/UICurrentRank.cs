using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
public class UICurrentRank : MonoBehaviour
{

    public static UICurrentRank instance;

    public TextMeshProUGUI[] txtRank;
    public List<CharacterControls> characterList;
    public GameObject endPos;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        characterList = GameObject.FindObjectsOfType<CharacterControls>().ToList();
        for (int i = 0; i < txtRank.Length; i++)
        {
            if (i < 3)
                txtRank[i].color = Color.green;
            if (i >= 3)
                txtRank[i].color = Color.red;
        }

    }

    // Update is called once per frame
    void Update()
    {
        characterList = characterList.OrderBy(i => i.GetDistance(endPos.transform.position)).OrderBy(i => i.finishedRank).ToList();
        int index = characterList.Count;
        if (index > txtRank.Length)
            index = txtRank.Length;
        for (int i = 0; i < index; i++)
        {
            if (characterList[i].finishedRank != 100)
                txtRank[i].color = Color.yellow;
            txtRank[i].text = (i + 1) + "-" + characterList[i].strCharName;
        }
    }

    public static sbyte GetMyRank(CharacterControls c)
    {
        for (sbyte i = 0; i < instance.characterList.Count; i++)
        {
            if (instance.characterList[i] == c)
                return i;
        }
        return 100;
    }


}
