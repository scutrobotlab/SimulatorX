using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gameplay.Embedded
{
    public enum TextureLineType
{
    none,//不画线
    line,//自由线
    dotLine,//点画线
    eraseLine,//擦除线
}
public class TextureLineTool : MonoBehaviour {
 
    public static TextureLineTool Instance;
    #region 画线设置
    /// <summary>
    /// 画线风格
    /// </summary>
    [SerializeField]
    private TextureLineType lineType;//画线风格
    /// <summary>
    /// 渲染次数：点画线1000，曲线100  不可过大，太消耗渲染性能
    /// </summary>
    [SerializeField]
    private float lineNum = 100f;
    /// <summary>
    /// 笔刷的宽度
    /// </summary>
    [SerializeField]
    private int lineWidth = 3;
    /// <summary>
    /// 笔刷的颜色
    /// </summary>
    [SerializeField]
    private Color lineColor = new Color(1, 1, 1, 0);
    #endregion
 
    /// <summary>
    /// 被操作的贴图
    /// </summary>
    [SerializeField]
    private Texture2D targetTexture;
    /// <summary>
    /// 目标图片UI
    /// </summary>
    [SerializeField]
    private RawImage targetRawImage;
    /// <summary>
    /// 划线相机
    /// </summary>
    [SerializeField]
    private Camera line_Camera;
    /// <summary>
    /// 擦除鼠标图片跟随
    /// </summary>
    [SerializeField]
    private Image mouseImage;
 
    /// <summary>
    /// 上一个点鼠标点
    /// </summary>
    Vector2 lastPoint = Vector2.zero;
 
    /// <summary>
    /// 被记录的鼠标位置
    /// </summary>
    private List<Vector2> mousePoints;
    /// <summary>
    /// 被记录的步骤
    /// </summary>
   private List<Texture2D> textureRecords;
 
    /// <summary>
    /// 当前画板下标
    /// </summary>
    int index = 0;
 
 
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        lineType = TextureLineType.none;
        mousePoints = new List<Vector2>();
        targetRawImage.texture = new Texture2D(Screen.width, Screen.height);
        targetTexture = targetRawImage.texture as Texture2D;
        ClearTexture();
        textureRecords = new List<Texture2D>();
        RecortStep();
    }
 
 
    void Update()
    {
        if (lineType == TextureLineType.line || lineType == TextureLineType.eraseLine)
        {
            if (Input.GetMouseButton(0))
            {
                if (IsPointerOverUI(targetRawImage.transform))
                {
                    Vector2 addPoint = line_Camera.ScreenToViewportPoint(Input.mousePosition);
                    OperateTexture(addPoint, lineType);
                }
                else
                {
                    lastPoint = Vector2.zero;
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                lastPoint = Vector2.zero;
                RecortStep();
                mouseImage.transform.localPosition = new Vector2(-10000, -10000);
            }
        }
        else if (lineType == TextureLineType.dotLine)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI(targetRawImage.transform))
                {
                    Vector2 addPoint = line_Camera.ScreenToViewportPoint(Input.mousePosition);
                    OperateTexture(addPoint, lineType);
                }
                else
                {
                    lastPoint = Vector2.zero;
                }
            }
 
        }

        if (!Input.GetMouseButtonDown(1)) return;
        lastPoint = Vector2.zero;
        lineType = TextureLineType.none;
    }
 
    /// <summary>
    /// 画线
    /// </summary>
    /// <param name="point"></param>
    /// <param name="mColor"></param>
    private  void OperateTexture(Vector2 point, TextureLineType lineType)
    {
        if (lastPoint == Vector2.zero)
        {
            lastPoint = point;
            return;
        }
        
        for (int j = 0; j < lineNum; j++)
        {
            float scaleX = Mathf.Lerp(lastPoint.x, point.x, j / lineNum);
            float scaleY = Mathf.Lerp(lastPoint.y, point.y, j / lineNum);
            int textureX = (int)(scaleX * Screen.width);
            int textureY = (int)(scaleY * Screen.height);
            // 线条加粗
            for (int a = textureX - lineWidth; a < textureX + lineWidth; a++)
            {
                for (int b = textureY - lineWidth; b < textureY + lineWidth; b++)
                {
                    targetTexture.SetPixel(a, b, lineColor);
                }
            }
        }
        targetTexture.Apply();
        lastPoint = point;
        if (lineType == TextureLineType.dotLine)
            RecortStep();
        else if (lineType == TextureLineType.eraseLine)
        {
            Vector2 pos;
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(this.transform as RectTransform,Input.mousePosition,line_Camera,out pos))
            {
                mouseImage.transform.localPosition = pos;
            }
        }
    }
 
    /// <summary>
    /// 重置贴图(全部擦除)
    /// </summary>
    public void ClearTexture()
    {
        int x, y;
        for (x = 0; x < targetTexture.width; x++)
        {
            for (y = 0; y < targetTexture.height; y++)
            {
                targetTexture.SetPixel(x, y, new Color(1, 1, 1, 0));
            }
        }
        targetTexture.Apply();
    }
 
    /// <summary>
    /// 设置画线模式
    /// </summary>
    public void SetLineType(TextureLineType type)
    {
        lastPoint = Vector2.zero;
        lineType = type;
        switch (lineType)
        {
            case TextureLineType.none:
                break;
            case TextureLineType.line:
                 lineNum = 100f;
                 lineWidth = 3;//设置笔宽
                lineColor = Color.red;
                break;
            case TextureLineType.dotLine:
                lineNum = 1000f;
                lineWidth = 3;//设置笔宽
                lineColor = Color.red;
                break;
            case TextureLineType.eraseLine:
                lineNum = 200f;
                lineWidth = 30;//设置笔宽
                lineColor = new Color(1, 1, 1, 0);
                break;
            default:
                break;
        }
    }
 
    /// <summary>
    /// 记录步骤
    /// </summary>
    private void RecortStep()
    {
        if (textureRecords.Count > 0)
        {
            if (index != textureRecords.Count - 1)
            {
                textureRecords.RemoveRange((index + 1), textureRecords.Count - (index + 1));
            }
        }
        Texture2D newTexture = Instantiate(targetTexture);
        textureRecords.Add(newTexture);
        index = textureRecords.Count - 1;
    }
    /// <summary>
    /// 上一步
    /// </summary>
    public void StepUp()
    {
        if (index > 0)
        {
            index--;
            targetTexture = Instantiate(textureRecords[index]);
            targetRawImage.texture = targetTexture;
        }
    }
    /// <summary>
    /// 下一步
    /// </summary>
    public void StepDown()
    {
        if (index < textureRecords.Count - 1)
        {
            index++;
            targetTexture = Instantiate(textureRecords[index]);
            targetRawImage.texture = targetTexture;
        }
    }
 
    /// <summary>
    /// 获取当前UI
    /// </summary>
    private bool IsPointerOverUI(Transform tran)
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;
            List<RaycastResult> result = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, result);
            if (result.Count > 0)
                foreach (var item in result)
                {
                    if (item.gameObject.transform.IsChildOf(tran))
                    {
                        return true;
                    }
                }
        }
        return false;
    }
}
}