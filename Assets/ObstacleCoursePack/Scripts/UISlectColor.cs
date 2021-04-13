using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UISlectColor : MonoBehaviour
{
    public List<Button> m_btnColors;
    public TexturePainter m_painter;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var s in m_btnColors)
        {
            s.onClick.AddListener(()=> {
                Image img = s.GetComponent<Image>();
                m_painter.SetBrushColor(img.color);
            });

           
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
