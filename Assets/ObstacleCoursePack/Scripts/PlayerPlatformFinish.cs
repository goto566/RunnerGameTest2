using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformFinish : MonoBehaviour
{
    public TexturePainter PaintObject;
    public Camera mainCamera;
    public GameObject UIRank;
    public GameObject paintPercent;
    public GameObject sprayObject;


    public GameObject charPos;

    private void OnTriggerEnter(Collider other)
    {
        CharacterControls charControl = other.gameObject.GetComponent<CharacterControls>();
        if (charControl == null)
            return;
        charControl.rb.velocity = Vector3.zero;
        if (!charControl.isMainPlayeer)
            charControl.gameObject.SetActive(false);
        else
        {
            charControl.gameObject.transform.position = charPos.transform.position;
            charControl.gameObject.transform.rotation = Quaternion.identity;
            charControl.anim.SetBool("isRunning", false);
            charControl.enabled = false;
            sprayObject.SetActive(true);
        }

        charControl.finishedRank = UICurrentRank.GetMyRank(charControl);
        if (charControl.isMainPlayeer)
        {
            mainCamera.gameObject.SetActive(false);
            UIRank.SetActive(false);
            paintPercent.SetActive(true);
            PaintObject.gameObject.SetActive(true);
        }


    }

}
