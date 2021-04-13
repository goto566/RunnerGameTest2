/// <summary>
/// CodeArtist.mx 2015
/// This is the main class of the project, its in charge of raycasting to a model and place brush prefabs infront of the canvas camera.
/// If you are interested in saving the painted texture you can use the method at the end and should save it to a file.
/// </summary>


using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;
using TMPro;
using System;

public enum Painter_BrushMode { PAINT, DECAL };
public class TexturePainter : MonoBehaviour
{
    public GameObject brushCursor, brushContainer; //The cursor that overlaps the model and our container for the brushes painted
    public Camera sceneCamera, canvasCam;  //The camera that looks at the model, and the camera that looks at the canvas.
    public Sprite cursorPaint, cursorDecal; // Cursor for the differen functions 
    public RenderTexture canvasTexture; // Render Texture that looks at our Base Texture and the painted brushes
    public Material baseMaterial; // The material of our base texture (Were we will save the painted texture)
    public Texture defaultTexture;
    public LayerMask paintLayer;
    public SpriteRenderer selectedBrush;
    public TextMeshProUGUI txtPaintPercent;

    public GameObject paintObject;

    Painter_BrushMode mode; //Our painter mode (Paint brushes or decals)
    public float brushSize = 1.0f; //The size of our brush
    Color brushColor; //The selected color
    int brushCounter = 0, MAX_BRUSH_COUNT = 1000; //To avoid having millions of brushes
    bool saving = false; //Flag to check if we are saving the texture

    public Animator playerAnimator;

    public void SetBrushColor(Color color)
    {
        brushColor = color;
    }
    public void SetBrush(SpriteRenderer sr)
    {
        selectedBrush = sr;
    }

    private void Start()
    {
        baseMaterial.SetTexture("_MainTex", defaultTexture);
        paintObject.SetActive(false);
    }
    float useTimer = .5f;
    private void OnEnable()
    {
        useTimer = .5f;
    }
    void Update()
    {
        useTimer -= Time.deltaTime;
        if (useTimer > 0)
            return;
        if (Input.GetMouseButton(0))
        {
            DoAction();
           
        }
        else
            playerAnimator.SetBool("isPainting", false);

        UpdateBrushCursor();
    }

    //The main action, instantiates a brush or decal entity at the clicked position on the UV map
    int layerOder = 1;
    void DoAction()
    {
        if (saving)
            return;
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition))
        {

            SpriteRenderer brushObj = Instantiate(selectedBrush);
            brushObj.color = brushColor;
            brushObj.sortingOrder = layerOder++;

            brushColor.a = brushSize * 2.0f; // Brushes have alpha to have a merging effect when painted over.
            brushObj.transform.parent = brushContainer.transform; //Add the brush to our container to be wiped later
            brushObj.transform.localPosition = uvWorldPosition; //The position of the brush (in the UVMap)
            brushObj.transform.localScale = Vector3.one * brushSize;//The size of the brush
            SetPaintPercent();
            playerAnimator.SetBool("isPainting", true);
            playerAnimator.gameObject.transform.LookAt(lastHitPoint);
        }
        else
            playerAnimator.SetBool("isPainting", false);
        brushCounter++; //Add to the max brushes
        if (brushCounter >= MAX_BRUSH_COUNT)
        { //If we reach the max brushes available, flatten the texture and clear the brushes
            brushCursor.SetActive(false);
            saving = true;
            Invoke("SaveTexture", 0.1f);

        }
    }
    //To update at realtime the painting cursor on the mesh
    void UpdateBrushCursor()
    {
        Vector3 uvWorldPosition = Vector3.zero;
        if (HitTestUVPosition(ref uvWorldPosition) && !saving)
        {
            brushCursor.SetActive(true);
            brushCursor.transform.position = uvWorldPosition + brushContainer.transform.position;
        }
        else
        {
            brushCursor.SetActive(false);
        }
    }
    Vector3 lastHitPoint;
    //Returns the position on the texuremap according to a hit in the mesh collider
    bool HitTestUVPosition(ref Vector3 uvWorldPosition)
    {
        RaycastHit hit;
        Vector3 cursorPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f);
        Ray cursorRay = sceneCamera.ScreenPointToRay(cursorPos);
        if (Physics.Raycast(cursorRay, out hit, 200, paintLayer))
        {
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider == null || meshCollider.sharedMesh == null)
                return false;
            lastHitPoint = hit.point;
            lastHitPoint.y = 0;

            Vector2 pixelUV = new Vector2(hit.textureCoord.x, hit.textureCoord.y);
            uvWorldPosition.x = pixelUV.x - canvasCam.orthographicSize;//To center the UV on X
            uvWorldPosition.y = pixelUV.y - canvasCam.orthographicSize;//To center the UV on Y
            uvWorldPosition.z = 0.0f;
            return true;
        }
        else
        {
            return false;
        }

    }
    //Sets the base material with a our canvas texture, then removes all our brushes
    void SaveTexture()
    {

        brushCounter = 0;
        System.DateTime date = System.DateTime.Now;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        baseMaterial.mainTexture = tex; //Put the painted texture as the base
        foreach (Transform child in brushContainer.transform)
        {//Clear brushes
            Destroy(child.gameObject);
        }
        saving = false;
        //StartCoroutine ("SaveTextureToFile"); //Do you want to save the texture? This is your method!
        ///   Invoke("ShowCursor", 0.1f);
    }
    //Show again the user cursor (To avoid saving it to the texture)


    ////////////////// PUBLIC METHODS //////////////////

    public void SetBrushMode(Painter_BrushMode brushMode)
    { //Sets if we are painting or placing decals
        mode = brushMode;
        brushCursor.GetComponent<SpriteRenderer>().sprite = brushMode == Painter_BrushMode.PAINT ? cursorPaint : cursorDecal;
    }
    public void SetBrushSize(float newBrushSize)
    { //Sets the size of the cursor brush or decal
        brushSize = newBrushSize;
        brushCursor.transform.localScale = Vector3.one * brushSize;
    }
    void SetPaintPercent()
    {
        if (txtPaintPercent == null)
            return;
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        var colorGroups = tex.GetPixels(0, 0, canvasTexture.width, canvasTexture.height).GroupBy(i => i).OrderByDescending(i => i.Count());
        int sum = colorGroups.Sum(i => i.Count());
        int whiteCount = colorGroups.Where(i => i.Key.r >= 0.9f && i.Key.g >= 0.9f && i.Key.b >= 0.9f).Sum(i => i.Count());
        int otherColorsSum = sum - whiteCount;

        txtPaintPercent.text = "Painted Wall : %" + Mathf.Ceil((float)otherColorsSum / sum * 100);


        //foreach (var s in test)
        //{
        //    Debug.Log(s.Key + "->" + s.Count() + " Toplam = " + toplam);
        //    //txtPaintPercent.text = "DicSize  = " + test.Count();
        //}

    }
    int GetColorCount(Color color)
    {
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        var test = tex.GetPixels(0, 0, canvasTexture.width, canvasTexture.height).GroupBy(i => i).OrderByDescending(i => i.Count());
        int toplam = test.Sum(i => i.Count());
        int sumColor = test.Where(i => i.Key == color).Sum(i => i.Count());
        return sumColor;
    }
    void FillColor(Color color)
    {
        RenderTexture.active = canvasTexture;
        Texture2D tex = new Texture2D(canvasTexture.width, canvasTexture.height, TextureFormat.RGB24, false);
        tex.ReadPixels(new Rect(0, 0, canvasTexture.width, canvasTexture.height), 0, 0);
        Color[] arr = new Color[canvasTexture.width * canvasTexture.height];
        for (int i = 0; i < canvasTexture.width * canvasTexture.height; i++)
            arr[i] = color;



        tex.SetPixels(0, 0, canvasTexture.width, canvasTexture.height, arr);
        tex.Apply();
    }


    ////////////////// OPTIONAL METHODS //////////////////



#if !UNITY_WEBPLAYER
    IEnumerator SaveTextureToFile(Texture2D savedTexture)
    {
        brushCounter = 0;
        string fullPath = System.IO.Directory.GetCurrentDirectory() + "\\UserCanvas\\";
        System.DateTime date = System.DateTime.Now;
        string fileName = "CanvasTexture.png";
        if (!System.IO.Directory.Exists(fullPath))
            System.IO.Directory.CreateDirectory(fullPath);
        var bytes = savedTexture.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath + fileName, bytes);
        Debug.Log("<color=orange>Saved Successfully!</color>" + fullPath + fileName);
        yield return null;
    }
#endif
}
