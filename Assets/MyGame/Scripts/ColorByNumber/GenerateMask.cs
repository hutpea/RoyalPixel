using BizzyBeeGames.ColorByNumbers;
using DG.Tweening;
using EventDispatcher;
using MoreMountains.NiceVibrations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class GenerateMask : MonoBehaviour
{
    private int m_width = -1;
    public List<Color32> m_colorDist;
    public List<Color32> m_colors;
    private int m_height = -1;

    [SerializeField]
    private Transform m_numbersContent;
    private Color32[] m_pixels;
    private Color32[] m_pixels_draw;
    public Action OnComplete;

    public Action<Color> OnColorComplete;

    public Action<Color> OnColorUncomplete;

    public Action<int, int, int> OnCheckColor;

    public Action<bool> OnColorPixel;

    public int AllPixelsCount;

    public int ColoredPixCount;

    public Vector2 clearpoint;

    private Dictionary<Color, bool> m_completedColors = new Dictionary<Color, bool>();
    public Dictionary<Vector2, Vector2> posPixel;
    [SerializeField]
    private Camera m_mainCamera;
    public bool loadSuccess;

    public MeshRenderer m_grayRenderer;

    [SerializeField]
    private MeshRenderer m_gridRenderer;

    [SerializeField]
    private MeshRenderer m_gridLoupeRenderer;

    [SerializeField]
    private MeshRenderer m_highlightedGridRenderer1;

    public MeshRenderer m_highlightedTextureRenderer;

    public MeshRenderer m_newHighlightedTextureRenderer;

    public Material m_resMaterial;

    [SerializeField]
    private GameObject m_content;
    public History history;
    public Texture2D[] m_highlightedTextures;

    public int[,] checkPainted;
    public int[,] colorsNumber;
    public Dictionary<int, int> numTileStart;
    public Dictionary<int, int> numTilePainted;
    public Dictionary<int, int> numTileUnPainted;
    public Dictionary<Color, int> numTileColors;
    public Dictionary<int, List<Tile>> pixelPerNumber;
    public Dictionary<Vector2, int> dicPixelNumber;
    public int totalPixel, totalUnpaint, startTotalUnpaid, totalPainted;
    public static GenerateMask instance;
    public bool edit;
    public int totalPixelDraw;
    //public SpriteRenderer surprised;
    //public GameObject itemLightUp;
    int preX, preY;
    private List<Texture2D> gameTextures;
    [Tooltip("The amount of alpha that is applied to the grayscale texture for each picture.")]
    [SerializeField] private float grayscaleColorAlpha = 1;

    [Tooltip("The amount of alpha that is applied to the black texture that goes over the picture to indicate cells that correspond to the color number the player has selected.")]
    [SerializeField] private float selectionColorAlpha = 0.15f;

    [Tooltip("The amount of alpha that is applied to an incorrect colored cell.")]
    [SerializeField] private float incorrectColorAlpha = 0.3f;

    [Tooltip("List of all the categories that can be played in the game.")]
    [SerializeField] private List<CategoryInfo> categoryInfos = null;
    public List<CategoryInfo> CategoryInfos { get { return categoryInfos; } }
    bool modify;
    private void OnEnable()
    {
        instance = this;
        history = GameData.HistoryForPictureId(GameData.picChoice.Id);
        loadSuccess = false;
        if (GameData.picChoice.SinglePic || GameData.picChoice.CreatePic || GameData.picChoice.DrawPic)
        {
            GameData.picSinglePainted.Add(GameData.picChoice.Id);

        }
    }

    private void Start()
    {
        StartInit();

    }
    void SetTotalUnPaintPixels()
    {
        for (int i = 0; i < pixelPerNumber.Count; i++)
        {
            List<Tile> pixels = pixelPerNumber[i + 1];
            for (int j = 0; j < pixels.Count; j++)
            {
                if (checkPainted[(int)pixels[j].x, (int)pixels[j].y] == 0)
                {
                    totalUnpaint++;
                    if (numTileUnPainted.ContainsKey(i + 1))
                    {
                        numTileUnPainted[i + 1]++;
                    }
                    else
                    {
                        numTileUnPainted.Add(i + 1, 1);
                    }
                }
            }
        }
        totalPainted = totalPixel - totalUnpaint;
        UIGameController.instance.SetProgressComplete((float)totalPainted / (float)totalPixel, totalPainted, totalPixel);
        startTotalUnpaid = totalUnpaint;
    }

    public void StartInit()
    {
        numTileStart = new Dictionary<int, int>();
        numTilePainted = new Dictionary<int, int>();
        numTileColors = new Dictionary<Color, int>();
        numTileUnPainted = new Dictionary<int, int>();
        m_colors = new List<Color32>();
        m_colorDist = new List<Color32>();
        pixelPerNumber = new Dictionary<int, List<Tile>>();
        dicPixelNumber = new Dictionary<Vector2, int>();
        Init(GameData.picChoice);
    }
    public List<Color32> GroupColor(Color32[] colors)
    {
        IEnumerable<Color32> query = colors
       .Where(s => s != Color.white && s.a != 0)
       .GroupBy(s => s) // groups identical strings into an IGrouping
                        //.OrderByDescending(group => group.Count()) // IGrouping is a collection, so you can count it
       .Select(group => group.Key); // IGrouping has a Key, which is the thing you used to group with. In this case group.Key==group.First()==group.skip(1).First()
        return query.ToList();
    }
    public void InitMaskDrawPixel()
    {

    }
    // Start is called before the first frame update
    public void Init(PictureInformation pictureInfo)
    {
        Debug.Log("bb" + ((Mathf.Max(m_width, m_height))));
        m_grayRenderer.sharedMaterial.mainTexture = GameData.curGrayTexture;
        if (GameData.isDrawPixel)
            m_gridLoupeRenderer.gameObject.SetActive(false);
        m_gridLoupeRenderer.sharedMaterial.mainTexture = GameData.curGrayTexture;
        m_gridLoupeRenderer.sharedMaterial.SetFloat("_Alpha1", 1);
        m_width = pictureInfo.XCells;
        m_height = pictureInfo.YCells;
        m_pixels = GameData.CurColorTexture.GetPixels32();
        if (GameData.isDrawPixel)
        {
            if (GameData.whiteColor != null)
            {
                m_pixels_draw = GameData.whiteColor.GetPixels32();
                GameData.whiteColor = null;
            }
            else
                m_pixels_draw = GameData.CurColorTexture.GetPixels32();
        }
        m_highlightedTextures = new Texture2D[pictureInfo.Colors.Count];
        GamePlayControl.Instance.mainCamera.minOrthographicSize = Mathf.Min(0.7f, 0.7f * (20f / (Mathf.Max(m_width, m_height))));
        colorsNumber = new int[m_width, m_height];
        checkPainted = new int[m_width, m_height];
        Texture2D texture2D = new Texture2D(m_width, m_height, TextureFormat.RGBA32, mipChain: false);
        texture2D.filterMode = FilterMode.Point;
        Color[] array = new Color[GameData.isDrawPixel ? m_pixels_draw.Length : m_pixels.Length];
        Color color = new Color(1f, 1f, 1f, 0f);
        Color white = Color.white;
        if (!GameData.isDrawPixel)
            for (int i = 0; i < array.Length; i++)
            {
                if (m_pixels[i] == white || m_pixels[i].a == 0)
                {
                    array[i] = white;
                    continue;
                }
                array[i] = color;
            }
        else
        {
            for (int i = 0; i < array.Length; i++)
            {
                //if (m_pixels_draw[i] == white || m_pixels_draw[i].a == 0)
                //{
                //    array[i] = white;
                //    continue;
                //}
                array[i] = color;
            }
        }
        texture2D.SetPixels(array);
        GetHistoryApplyMaterial(texture2D);
        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.Apply();
        m_resMaterial.mainTexture = texture2D;
        if (GameData.isDrawPixel)
        {
            m_colorDist = GroupColor(GameData.peletteColor.GetPixels32());
        }
        else

            m_colorDist = GroupColor(m_pixels);
        m_grayRenderer.sharedMaterial.SetFloat("_Alpha", 1);
        if (m_width > m_height)
        {
            base.transform.localScale = new Vector3(1f, (float)m_height / (float)m_width, 1f);
        }
        else if (m_width < m_height)
        {
            base.transform.localScale = new Vector3((float)m_width / (float)m_height, 1f, 1f);
        }
        StartCoroutine(GenerateNumbersCoroutine(delegate
        {
            if (totalPixel > 300)
                GameData.isPicBig = true;
            else
                GameData.isPicBig = false;
            SetCheckPaintedPixel(texture2D);
            if (!GameData.isDrawPixel)
            {
                SetTotalUnPaintPixels();
            }
            GamePlayControl.Instance.GenerateNumberColor();
            if (!GameData.isDrawPixel)
            {
                //Show button when init game
                GenerateHighlightedTextures();
                m_highlightedTextureRenderer.gameObject.SetActive(true);
                m_highlightedTextureRenderer.material.mainTexture = m_highlightedTextures[GetSelectedNumber() - 1];
                m_newHighlightedTextureRenderer = UnityEngine.Object.Instantiate(m_highlightedTextureRenderer, m_content.transform);
                m_highlightedTextureRenderer.gameObject.SetActive(false);
                m_newHighlightedTextureRenderer.sharedMaterial.SetFloat("_Alpha", 0);
                m_newHighlightedTextureRenderer.material.mainTexture = m_highlightedTextures[GetSelectedNumber() - 1];
                m_newHighlightedTextureRenderer.material.renderQueue = 3004;
                //if (GameData.newTutorial && GameData.newPlayer)
                //    m_newHighlightedTextureRenderer.gameObject.SetActive(false);
                m_newHighlightedTextureRenderer.material.renderQueue = 2500;
            }
            loadSuccess = true;
            //CanvasControll.instance.ClosePanelLoading();
        }, pictureInfo));
    }
    void SetCheckPaintedPixel(Texture2D texture)
    {
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                //Debug.Log(checkPainted[i, j]);
                Color color = texture.GetPixel(i, j);
                if (color != Color.white && color.a > 0f)
                {
                    checkPainted[i, j] = 1;
                    totalPixelDraw++;
                    UIGameController.instance.btnSaveDraw.SetActive(true);
                }
            }
        }
    }
    void GetHistoryApplyMaterial(Texture2D texture)
    {
        History history = GameData.HistoryForPictureId(GameData.picChoice.Id);
        if (GameData.isDrawPixel)
        {
            List<HistoryDrawStep> historySteps;
            historySteps = history.DrawSteps;

            for (int i = 0; i < historySteps.Count; i++)
            {
                //Debug.Log("get history");
                int x = historySteps[i].x;
                int y = historySteps[i].y;
                Color color = new Color32(historySteps[i].colorR, historySteps[i].colorG, historySteps[i].colorB, 255);
                texture.SetPixel(x, y, color);
            }
        }
        else
        {
            List<HistoryStep> historySteps;
            historySteps = history.Steps;
            for (int i = 0; i < historySteps.Count; i++)
            {
                //Debug.Log("get history");
                int x = historySteps[i].x;
                int y = historySteps[i].y;
                texture.SetPixel(x, y, GameData.CurColorTexture.GetPixel(x, y));
            }
        }

    }
    public void SetHighLight(int number)
    {
        if (!loadSuccess)
            return;
        GamePlayControl.Instance.selectedNumber = number;
        m_highlightedTextureRenderer.material.mainTexture = m_highlightedTextures[number - 1];
        m_newHighlightedTextureRenderer.material.mainTexture = m_highlightedTextures[number - 1];
        GamePlayControl.Instance.SetSelectBtn();

    }
    private IEnumerator GenerateNumbersCoroutine(Action endAction, PictureInformation picture)
    {
        Numbers2DSettings numbers2DSettings = Numbers2DSettings.GetSettingsByQuality(MySystemInfo.AppTextureQuality);
        int newWidth = m_width * numbers2DSettings.CellSize;
        int newHeight = m_height * numbers2DSettings.CellSize;
        int maxSize = 3600;
        Color[] array2 = new Color[numbers2DSettings.CellSize * numbers2DSettings.CellSize];
        Color[] emptyColors = /*AppData.BulbMode ?*/ Resources.Load<Texture2D>("Grid/" + numbers2DSettings.GridCellTextureName).GetPixels()/* : Resources.Load<Texture2D>(numbers2DSettings.GridCellTextureName + "_17p").GetPixels()*/;
        Dictionary<Color, int> colorsDict = new Dictionary<Color, int>();
        Dictionary<Color, Color[]> colorTextures = new Dictionary<Color, Color[]>();
        Color32 whiteColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);
        for (int i = 0; i < m_colorDist.Count; i++)
        {
            //Debug.Log("Hex String: " + Convert.ToString(m_colorDist[i].r, 16) + Convert.ToString(m_colorDist[i].g, 16) + Convert.ToString(m_colorDist[i].b, 16));
            if (!colorsDict.ContainsKey(m_colorDist[i])) colorsDict.Add(m_colorDist[i], i + 1);
            else colorsDict[m_colorDist[i]] = i + 1;
            int num = i + 1;
            Color[] array = (Color[])emptyColors.Clone();
            if (num < 10)
            {
                int num2 = (numbers2DSettings.CellSize - numbers2DSettings.NumberSize.X) / 2;
                int num3 = (numbers2DSettings.CellSize - numbers2DSettings.NumberSize.Y) / 2;
                for (int j = 0; j < numbers2DSettings.NumberSize.X; j++)
                {
                    for (int k = 0; k < numbers2DSettings.NumberSize.Y; k++)
                    {
                        array[num2 + j + (num3 + k) * numbers2DSettings.CellSize] = numbers2DSettings.NumberPixels[num][j + k * numbers2DSettings.NumberSize.X];
                    }
                }
            }
            else if (num < 100)
            {
                int num4 = num / 10;
                int num5 = num - num4 * 10;
                int num6 = (numbers2DSettings.CellSize - numbers2DSettings.NumberSize.X * 2 - 2) / 2 + 1;
                int num7 = (numbers2DSettings.CellSize - numbers2DSettings.NumberSize.Y) / 2;
                for (int l = 0; l < numbers2DSettings.NumberSize.X; l++)
                {
                    for (int m = 0; m < numbers2DSettings.NumberSize.Y; m++)
                    {
                        array[num6 + l + (num7 + m) * numbers2DSettings.CellSize] = numbers2DSettings.NumberPixels[num4][l + m * numbers2DSettings.NumberSize.X];
                    }
                }
                num6 += numbers2DSettings.NumberSize.X;
                for (int n = 0; n < numbers2DSettings.NumberSize.X; n++)
                {
                    for (int num8 = 0; num8 < numbers2DSettings.NumberSize.Y; num8++)
                    {
                        array[num6 + n + (num7 + num8) * numbers2DSettings.CellSize] = numbers2DSettings.NumberPixels[num5][n + num8 * numbers2DSettings.NumberSize.X];
                    }
                }
            }
            if (!colorTextures.ContainsKey(m_colorDist[i])) colorTextures.Add(m_colorDist[i], array);
            else colorTextures[m_colorDist[i]] = array;
        }
        if (!colorTextures.ContainsKey(whiteColor)) colorTextures.Add(whiteColor, emptyColors);
        else colorTextures[whiteColor] = emptyColors;
        yield return null;
        Texture2D bigTex = new Texture2D(m_width, m_height, TextureFormat.Alpha8, mipChain: false);
        for (int mainI = 0; mainI * maxSize < newWidth; mainI++)
        {
            int curWidth = Mathf.Min(maxSize, newWidth - mainI * maxSize);
            for (int mainJ = 0; mainJ * maxSize < newHeight; mainJ++)
            {
                int curHeight = Mathf.Min(maxSize, newHeight - mainJ * maxSize);
                bigTex = new Texture2D(curWidth, curHeight, TextureFormat.Alpha8, mipChain: false);
                yield return null;
                for (int num9 = 0; num9 < curWidth / numbers2DSettings.CellSize; num9++)
                {
                    int num10 = num9 + mainI * maxSize / numbers2DSettings.CellSize;
                    for (int num11 = 0; num11 < curHeight / numbers2DSettings.CellSize; num11++)
                    {
                        int num12 = num11 + mainJ * maxSize / numbers2DSettings.CellSize;
                        //Debug.Log(colorPix);
                        Color colorPix;
                        if (GameData.isDrawPixel)
                        {
                            colorPix = m_pixels_draw[num10 + num12 * m_width];
                        }
                        else
                        {
                            colorPix = m_pixels[num10 + num12 * m_width];
                            if (colorPix != Color.white && colorPix.a != 0)
                            {
                                //Debug.Log("total " + totalPixel);
                                if (!colorsDict.ContainsKey(colorPix)) continue;
                                colorsNumber[num10, num12] = colorsDict[colorPix];
                                totalPixel++;
                                if (pixelPerNumber.ContainsKey(colorsDict[colorPix]))
                                {
                                    Tile tile = new Tile();
                                    tile.x = num10;
                                    tile.y = num12;
                                    pixelPerNumber[colorsDict[colorPix]].Add(tile);
                                }
                                else
                                {
                                    List<Tile> lst = new List<Tile>();
                                    Tile tile = new Tile();
                                    tile.x = num10;
                                    tile.y = num12;
                                    lst.Add(tile);
                                    pixelPerNumber.Add(colorsDict[colorPix], lst);
                                }
                                if (numTileStart.ContainsKey(colorsDict[colorPix]))
                                    numTileStart[colorsDict[colorPix]]++;
                                else
                                {
                                    numTileStart.Add(colorsDict[colorPix], 1);
                                }
                                Vector2 keys = new Vector2(num10, num12);
                                if (!dicPixelNumber.ContainsKey(keys)) dicPixelNumber.Add(keys, colorsDict[colorPix]);
                                else dicPixelNumber[keys] = colorsDict[colorPix];
                            }
                        }
                        bigTex.SetPixels(num9 * numbers2DSettings.CellSize, num11 * numbers2DSettings.CellSize, numbers2DSettings.CellSize, numbers2DSettings.CellSize, colorTextures[colorPix]);
                    }
                }
                bigTex.Apply();
                Transform curNumbersContent = UnityEngine.Object.Instantiate(m_numbersContent, m_content.transform);
                //curNumbersContent.SetParent(m_numbersContent.parent);
                MeshFilter mf = curNumbersContent.gameObject.AddComponent<MeshFilter>();
                mf.sharedMesh = new Mesh();
                mf.sharedMesh.vertices = new Vector3[4]
                {
                    new Vector2(-1f + 2f / (float)newWidth * (float)mainI * (float)maxSize, -1f + 2f / (float)newHeight * (float)mainJ * (float)maxSize),
                    new Vector2(-1f + 2f / (float)newWidth * (float)(mainI * maxSize + curWidth), -1f + 2f / (float)newHeight * (float)mainJ * (float)maxSize),
                    new Vector2(-1f + 2f / (float)newWidth * (float)mainI * (float)maxSize, -1f + 2f / (float)newHeight * (float)(mainJ * maxSize + curHeight)),
                    new Vector2(-1f + 2f / (float)newWidth * (float)(mainI * maxSize + curWidth), -1f + 2f / (float)newHeight * (float)(mainJ * maxSize + curHeight))
                };
                mf.sharedMesh.uv = new Vector2[4]
                {
                    new Vector2(0f, 0f),
                    new Vector2(1f, 0f),
                    new Vector2(0f, 1f),
                    new Vector2(1f, 1f)
                };
                mf.sharedMesh.triangles = new int[6]
                {
                    0,
                    3,
                    1,
                    2,
                    3,
                    0
                };
                MeshRenderer mr = curNumbersContent.gameObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = new Material(Shader.Find("My/FromAlphaShaderForNumbers2D"));
                mr.sharedMaterial.renderQueue = 3002;
                mr.sharedMaterial.mainTexture = bigTex;
                mr.sharedMaterial.SetFloat("_Alpha", 0.9f);
            }
        }
        bigTex.filterMode = FilterMode.Point;
        bigTex.wrapMode = TextureWrapMode.Clamp;
        bigTex.Apply();
        endAction.SafeInvoke();
    }
    private void GenerateHighlightedTextures()
    {
        Color32 color = new Color32(150, 150, 150, byte.MaxValue);
        Color32 color2 = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
        Color32[] array = new Color32[m_pixels.Length];
        for (int i = 0; i < m_colorDist.Count; i++)
        {
            m_highlightedTextures[i] = new Texture2D(m_width, m_height, TextureFormat.RGBA32, mipChain: false);
            m_highlightedTextures[i].filterMode = FilterMode.Point;
            m_highlightedTextures[i].wrapMode = TextureWrapMode.Clamp;
            for (int j = 0; j < m_pixels.Length; j++)
            {
                array[j] = ((!m_pixels[j].EqualsColor32(m_colorDist[i])) ? color2 : color);
            }
            m_highlightedTextures[i].SetPixels32(array);
            m_highlightedTextures[i].Apply();
        }
    }
    [SerializeField]
    private LayerMask m_raycastLayer;

    public void TryClickPixel(Vector2 mousePosition)
    {
        Ray ray = m_mainCamera.ScreenPointToRay(mousePosition);
        //Vector2 mouse = m_mainCamera.ScreenToWorldPoint(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 50f, m_raycastLayer))
        {
            Vector3 point = hitInfo.point;
            Vector3 mouse = hitInfo.point;
            float x = point.x;
            Vector3 lossyScale = hitInfo.collider.transform.lossyScale;
            point.x = x / lossyScale.x;
            float y = point.y;
            Vector3 lossyScale2 = hitInfo.collider.transform.lossyScale;
            point.y = y / lossyScale2.y;
            point.x = (point.x + 1f) / 2f * (float)m_width;
            point.y = (point.y + 1f /*- 0.08f*/) / 2f * (float)m_height;
            int pixX = (int)point.x;
            int pixY = (int)point.y;
            if (pixX < 0 || pixY < 0)
            {
                return;
            }
            Color pixel;
            if (GameData.isDrawPixel)
            {
                pixel = m_colorDist[Math.Max(0, GamePlayControl.Instance.selectedNumber - 1)];
            }
            else
            {
                pixel = GameData.CurColorTexture.GetPixel(pixX, pixY);
                if ((checkPainted.GetLength(0) <= pixX || checkPainted.GetLength(1) <= pixY) || (pixel == Color.white || checkPainted[pixX, pixY] == 1))
                    return;

                if (GamePlayControl.Instance.UseItem())
                {
                    if (GamePlayControl.Instance.useItemStar)
                    {
                        int colorNumber = colorsNumber[pixX, pixY];
                        StartCoroutine(GamePlayControl.Instance.PaintStar(pixX, pixY, colorNumber));
                        return;
                    }
                    if (GamePlayControl.Instance.useItemBomb)
                    {
                        GamePlayControl.Instance.PaintBomb(pixX, pixY, 6);
                        return;
                    }
                }
            }
            SetPixelColorByClick(pixX, pixY, pixel, mouse);
        }
    }
    int countNumber = 0;
    public void SetPixelColorByClick(int pixX, int pixY, Color color, Vector2 posMouse)
    {
        if (preX == pixX && preY == pixY && GamePlayControl.Instance.mainCamera.brush)
            return;
        preX = pixX;
        preY = pixY;
        Color pixel = GameData.curGrayTexture.GetPixel(pixX, pixY);
        Color curPixel = ((Texture2D)m_resMaterial.mainTexture).GetPixel(pixX, pixY);
        int numberColor = 0;
        if (GameData.isDrawPixel)
        {
            if (GamePlayControl.Instance.useEraser)
            {
                if (pixel == Color.white || pixel.a == 0)
                    return;
                color.a = 0;
                GameData.curGrayTexture.SetPixel(pixX, pixY, Color.white);
                totalPixelDraw--;
                GameData.curGrayTexture.Apply();
                m_grayRenderer.sharedMaterial.mainTexture = GameData.curGrayTexture;
                history.RemoveValueContains(pixX, pixY);
                GameData.isEdit = true;
            }
            else
            {
                if (color == pixel)
                    return;
                if (pixel == Color.white || pixel.a == 0)
                    totalPixelDraw++;
                GameData.curGrayTexture.SetPixel(pixX, pixY, color);
                history.AddStepDraw(pixX, pixY, color);
                UIGameController.instance.SetFillCombo(posMouse, color);
                GameData.isEdit = true;
            }
            if (totalPixelDraw == 0)
            {
                UIGameController.instance.btnSaveDraw.SetActive(false);
            }
            else
            {
                UIGameController.instance.btnSaveDraw.SetActive(true);
            }
        }
        else
        {
            //if (pixel != Color.white)
            //{
            numberColor = colorsNumber[pixX, pixY];
            //}

            if (pixel == Color.white || pixel == curPixel || color == curPixel)
            {
                return;
            }
            if (!GamePlayControl.Instance.useItemPen && curPixel.a < 0.9f && curPixel.a > 0.05f && numberColor != GamePlayControl.Instance.selectedNumber && !GamePlayControl.Instance.UseItem())
            {
                color.a = 0;
            }
            else if (numberColor != GamePlayControl.Instance.selectedNumber && !GamePlayControl.Instance.UseItem() && !GamePlayControl.Instance.useItemPen)
            {
                Color newColor = m_colorDist[GamePlayControl.Instance.selectedNumber - 1];
                color = new Color(newColor.r, newColor.g, newColor.b, 0.1f);
            }

            else
            {
                checkPainted[pixX, pixY] = 1;
                edit = true;
                GamePlayControl.Instance.totalPixelPainting++;
                GamePlayControl.Instance.tempSelectNumber = numberColor;
                numTileUnPainted[numberColor]--;
                totalUnpaint--;
                if (!modify)
                {
                    modify = true;
                }
                GameData.curGrayTexture.SetPixel(pixX, pixY, color);
                history.AddStep(pixX, pixY);
                if(!GamePlayControl.Instance.UseItem())
                {
                    float process = (float)(totalPainted + GamePlayControl.Instance.totalPixelPainting) / (float)totalPixel;
                    UIGameController.instance.SetProgressComplete(process, GamePlayControl.Instance.totalPixelPainting, totalPixel);
                }
                UIGameController.instance.SetFillCombo(posMouse, color);
                this.PostEvent(EventID.DOPAINT, numTileUnPainted[numberColor]);
                if (numTileUnPainted[numberColor] == 0)
                {
                    CompletePainting(numberColor - 1);
                }
                try
                {
                    AchievementController.instance.UpdateProgressQuest(TypeQuest.Pixels);
                }
                catch
                {

                }
            }
        }
        ((Texture2D)m_resMaterial.mainTexture).SetPixel(pixX, pixY, color);
        ((Texture2D)m_resMaterial.mainTexture).Apply();
    }
    public void SetPixelItem(int x, int y, Color color, Vector2 posMouse)
    {
        Color curPixel = ((Texture2D)m_resMaterial.mainTexture).GetPixel(x, y);
        if (color == Color.white || color == curPixel)
        {
            return;
        }
        int numberColor = colorsNumber[x, y];
        checkPainted[x, y] = 1;
        edit = true;
        GamePlayControl.Instance.totalPixelPainting++;
        GamePlayControl.Instance.tempSelectNumber = numberColor;
        numTileUnPainted[numberColor]--;
        totalUnpaint--;
        if (!modify)
        {
            modify = true;
        }
        GameData.curGrayTexture.SetPixel(x, y, color);
        history.AddStep(x, y);
        if (numTileUnPainted[numberColor] == 0)
        {
            CompletePainting(numberColor - 1);
        }
        try
        {
        }
        catch
        {

        }
    ((Texture2D)m_resMaterial.mainTexture).SetPixel(x, y, color);
    }
    public int GetSelectedNumber()
    {
        for (int i = 1; i <= m_colorDist.Count; i++)
        {
            if (numTileUnPainted.ContainsKey(i) && numTileUnPainted[i] != 0)
            {
                return i;
            }
        }
        return 1;
    }
    private void OnDisable()
    {
        SavePainting();
    }
    bool isSaveProgress;
    bool isSaveAchiviement;
    public void SavePainting()
    {
        bool newUser = GameController.Instance.useProfile.NewUser;
        if (!GamePlayControl.Instance.isGameComplete && !GameData.picChoice.Completed && modify)
        {
            GameData.PicPainting = GameData.picChoice.Id;
        }
        if (!GameData.GetUnlockArea(GameData.picChoice.IdArea) && !GameData.picChoice.SinglePic && !GameData.picChoice.CreatePic && !GameData.picChoice.DrawPic)
        {
            GameData.PaintingAreas = GameData.picChoice.IdArea;
        }
        if (modify && !GameData.picChoice.CreatePic && !GameData.picChoice.SinglePicNoel && !GameData.picChoice.DrawPic && !isSaveProgress)
        {
            isSaveProgress = true;
            if (GameData.picChoice.SinglePic)
            {
                this.PostEvent(EventID.PAINTED_PIC, GameData.picChoice.Id);
                Debug.Log(GameData.picChoice.Id);
                GameData.InprogressPic(GameData.picChoice.Id);
            }
            else
                GameData.InprogressPic(GameData.picChoice.IdArea.ToString());
        }
        if (modify && totalPainted == 0 && !GameData.isDrawPixel && !isSaveAchiviement)
        {
            isSaveAchiviement = true;
            AchievementController.instance.UpdateProgressQuest(TypeQuest.Artist);
            if (GameData.picChoice.IdArea == CategoryConst.POPULAR)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Popular);
            if (GameData.picChoice.IdArea == CategoryConst.CUTE)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Cute);
            if (GameData.picChoice.IdArea == CategoryConst.FANTASY)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Fantasy);
            if (GameData.picChoice.IdArea == CategoryConst.FASHION)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Fashion);
            if (GameData.picChoice.IdArea == CategoryConst.FESTIVAL)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Festival);
            if (GameData.picChoice.IdArea == CategoryConst.FOOD)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Food);
            if (GameData.picChoice.IdArea == CategoryConst.ANIMAL)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Animal);
            if (GameData.picChoice.IdArea == CategoryConst.UNICORN)
                AchievementController.instance.UpdateProgressQuest(TypeQuest.Unicorn);
        }
        GameData.IdAreaChoice = GameData.PaintingAreas;
        history.StopRecord();
        GameData.picChoice.TotalPixelPainted = totalPainted + GamePlayControl.Instance.totalPixelPainting;
        GameController.Instance.useProfile.NewUser = false;
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SavePainting();
        }
    }
    int SelectNumberColorSuggest()
    {
        for (int i = GamePlayControl.Instance.selectedNumber; i <= m_colorDist.Count; i++)
        {
            if (numTileUnPainted.ContainsKey(i) && numTileUnPainted[i] != 0)
            {
                return i;
            }
        }
        for (int i = GamePlayControl.Instance.selectedNumber; i > 0; i--)
        {
            if (numTileUnPainted.ContainsKey(i) && numTileUnPainted[i] != 0)
            {
                return i;
            }
        }
        return 1;
    }
    public void CompletePainting(int index)
    {
        //Debug.Log("total " + totalUnpaint);
        var selectedNumber = SelectNumberColorSuggest();
        if (selectedNumber != -1)
        {
            SetHighLight(selectedNumber);
        }
        if (totalUnpaint <= 0 && !GameData.picChoice.Completed)
        {
            GameData.picChoice.Completed = true;
            GameData.CountGemPig += 40;
            GameData.isReciveGemPig = true;
            int old = GameData.GetCurrentPicInAreas(GameData.picChoice.IdArea);
            old++;
            if (!GameData.picChoice.SinglePic && !GameData.picChoice.CreatePic && !GameData.picChoice.DrawPic || GameData.picChoice.SinglePicNoel)
                GameData.SetCurrentPicInAreas(GameData.picChoice.IdArea, old);
            GamePlayControl.Instance.isGameComplete = true;
            if (old == GameData.totalPicInAreas && UseProfile.CanShowRate)
            {
                DialogueRate.Setup().Show();
            }
            //UIGameController.instance.btnBack.SetActive(false);
            GamePlayControl.Instance.mainCamera.ZoomCamFinish();
            UIGameController.instance.SetProgressComplete(1);
        }
        MMVibrationManager.Haptic(HapticTypes.Success, false, true, this);
    }

    public void ZoomUpdate(float zoom)
    {
        bool enableText = zoom > 0.15f /*&& generate.boardCam.orthographicSize < markedOrthographicSize*/;
        if (!GameData.isDrawPixel)
            m_newHighlightedTextureRenderer.sharedMaterial.SetFloat("_Alpha", zoom);
        if (enableText)
        {
            m_grayRenderer.material.renderQueue = 3000;
            //if (!GameData.isDrawPixel)
            {
                m_grayRenderer.sharedMaterial.SetFloat("_Alpha", (1 - 2 * zoom));
            }
            m_content.SetActive(true);
            if (zoom >= 0.4f)
            {
                m_gridLoupeRenderer.sharedMaterial.SetFloat("_Alpha", 0.5f * (1 - zoom));
                if (!GameData.isDrawPixel)
                    m_newHighlightedTextureRenderer.sharedMaterial.SetFloat("_Alpha", 0.8f * zoom);
            }
            else
            {
                m_gridLoupeRenderer.sharedMaterial.SetFloat("_Alpha", (1 - 0.7f * zoom));
            }
            if (!GameData.isDrawPixel)
                m_newHighlightedTextureRenderer.material.renderQueue = 3001;
        }
        else
        {
            m_grayRenderer.material.renderQueue = 3003;
            if (!GameData.isDrawPixel)
            {
                m_newHighlightedTextureRenderer.material.renderQueue = 3004;
            }
            m_grayRenderer.sharedMaterial.SetFloat("_Alpha", 1);
            m_gridLoupeRenderer.sharedMaterial.SetFloat("_Alpha1", 1);
        }
        //float opacityBtnback = 1f - zoom;
        //if (opacityBtnback < 0.3f)
        //    opacityBtnback = 0;
        //if (opacityBtnback < 0.5f)
        //{
        //    UIGameController.instance.btnBack.enabled = false;
        //}
        //else
        //{
        //    UIGameController.instance.btnBack.color = new Color(1, 1, 1, opacityBtnback);
        //    UIGameController.instance.btnBack.enabled = true;
        //}
    }
    public void TestPainted()
    {

    }
    public void ZoomAndMoveCamLookAtTileSelected()
    {
        List<Tile> lstPixel = pixelPerNumber[GamePlayControl.Instance.selectedNumber];
        for (int i = 0; i < lstPixel.Count; i++)
        {
            int x = (int)lstPixel[i].x;
            int y = (int)lstPixel[i].y;
            if (checkPainted[x, y] == 0)
            {
                Debug.Log("x " + x + "y " + y);
                float posX = (m_grayRenderer.transform.position.x - m_grayRenderer.bounds.size.x / 2) + (float)x * (m_grayRenderer.bounds.size.x / (float)m_width);
                float posY = (m_grayRenderer.transform.position.y - m_grayRenderer.bounds.size.y / 2) + y * (m_grayRenderer.bounds.size.y / (float)m_height);
                Debug.Log("pos " + posX + "|" + posY);
                Vector3 pos = new Vector3(posX, posY, -10);
                m_mainCamera.transform.DOMove(pos, 0.3f);
                //float endScale = m_mainCamera.orthographicSize;
                //if (m_mainCamera.orthographicSize == GamePlayControl.Instance.mainCamera.maxOrthographicSize)
                float endScale = GamePlayControl.Instance.mainCamera.minOrthographicSize;
                StartCoroutine(GamePlayControl.Instance.mainCamera.ZoomCamFocus(endScale, pos));
                return;
            }
        }
    }
    public void ResetObject()
    {
        if (m_content.transform.childCount != 0)
            foreach (Transform child in m_content.transform)
                Destroy(child.gameObject);
    }
}
public class Tile
{
    public int x;
    public int y;
}
