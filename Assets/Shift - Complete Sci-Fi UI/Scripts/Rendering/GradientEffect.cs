using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class GradientEffect : BaseMeshEffect
    {
        [Header("Gradient Style")]
        [Tooltip("Gradient Direction")]
        [SerializeField] Direction m_Direction;

        [Tooltip("Color1: Top or Left")]
        [SerializeField] Color m_Color1 = Color.white;

        [Tooltip("Color2: Bottom or Right")]
        [SerializeField] Color m_Color2 = Color.white;

        [Tooltip("Color3: For diagonal")]
        [SerializeField] Color m_Color3 = Color.white;

        [Tooltip("Color4: For diagonal")]
        [SerializeField] Color m_Color4 = Color.white;

        [Tooltip("Gradient rotation")]
        [SerializeField] [Range(-180, 180)] float m_Rotation;

        [Tooltip("Gradient offset for Horizontal, Vertical or Angle")]
        [SerializeField] [Range(-1, 1)] float m_Offset1;

        [Tooltip("Gradient offset for Diagonal")]
        [SerializeField] [Range(-1, 1)] float m_Offset2;

        [Header("Settings")]
        [Tooltip("Gradient style for Text.")]
        [SerializeField] GradientStyle m_GradientStyle;

        [Tooltip("Color space to correct color")]
        [SerializeField] ColorSpace m_ColorSpace = ColorSpace.Uninitialized;

        [Tooltip("Ignore aspect ratio")]
        [SerializeField] bool m_IgnoreAspectRatio = true;

        public Graphic TargetGraphic { get { return base.graphic; } }

        public enum Direction
        {
            Horizontal,
            Vertical,
            Angle,
            Diagonal,
        }

        public enum GradientStyle
        {
            Rect,
            Fit,
            Split,
        }

        public Direction DirectionMain
        {
            get { return m_Direction; }
            set
            {
                if (m_Direction != value)
                    m_Direction = value;
            }
        }

        public Color Color1
        {
            get { return m_Color1; }
            set
            {
                if (m_Color1 != value)
                    m_Color1 = value;
            }
        }

        public Color Color2
        {
            get { return m_Color2; }
            set
            {
                if (m_Color2 != value)
                    m_Color2 = value;
            }
        }

        public Color Color3
        {
            get { return m_Color3; }
            set
            {
                if (m_Color3 != value)
                    m_Color3 = value;
            }
        }

        public Color Color4
        {
            get { return m_Color4; }
            set
            {
                if (m_Color4 != value)
                    m_Color4 = value;
            }
        }

        public float Rotation
        {
            get
            {
                return m_Direction == Direction.Horizontal ? -90
                        : m_Direction == Direction.Vertical ? 0
                        : m_Rotation;
            }
            set
            {
                if (!Mathf.Approximately(m_Rotation, value))
                    m_Rotation = value;
            }
        }

        public float Offset
        {
            get { return m_Offset1; }
            set
            {
                if (m_Offset1 != value)
                    m_Offset1 = value;
            }
        }

        public Vector2 Offset2
        {
            get { return new Vector2(m_Offset2, m_Offset1); }
            set
            {
                if (m_Offset1 != value.y || m_Offset2 != value.x)
                {
                    m_Offset1 = value.y;
                    m_Offset2 = value.x;
                }
            }
        }

        public GradientStyle GradientStyleMain
        {
            get { return m_GradientStyle; }
            set
            {
                if (m_GradientStyle != value)
                    m_GradientStyle = value;
            }
        }

        public ColorSpace ColorSpace
        {
            get { return m_ColorSpace; }
            set
            {
                if (m_ColorSpace != value)
                    m_ColorSpace = value;
            }
        }

        public bool IgnoreAspectRatio
        {
            get { return m_IgnoreAspectRatio; }
            set
            {
                if (m_IgnoreAspectRatio != value)
                    m_IgnoreAspectRatio = value;
            }
        }

        public override void ModifyMesh(VertexHelper vh)
        {
            if (!IsActive())
                return;

            Rect rect = default(Rect);
            UIVertex vertex = default(UIVertex);

            if (m_GradientStyle == GradientStyle.Rect)
                rect = graphic.rectTransform.rect;

            else if (m_GradientStyle == GradientStyle.Split)
                rect.Set(0, 0, 1, 1);

            else if (m_GradientStyle == GradientStyle.Fit)
            {
                rect.xMin = rect.yMin = float.MaxValue;
                rect.xMax = rect.yMax = float.MinValue;

                for (int i = 0; i < vh.currentVertCount; i++)
                {
                    vh.PopulateUIVertex(ref vertex, i);
                    rect.xMin = Mathf.Min(rect.xMin, vertex.position.x);
                    rect.yMin = Mathf.Min(rect.yMin, vertex.position.y);
                    rect.xMax = Mathf.Max(rect.xMax, vertex.position.x);
                    rect.yMax = Mathf.Max(rect.yMax, vertex.position.y);
                }
            }

            float rad = Rotation * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));

            if (!m_IgnoreAspectRatio && Direction.Angle <= m_Direction)
            {
                dir.x *= rect.height / rect.width;
                dir = dir.normalized;
            }

            Color color;
            Vector2 nomalizedPos;
            Matrix2x3 localMatrix = new Matrix2x3(rect, dir.x, dir.y);

            for (int i = 0; i < vh.currentVertCount; i++)
            {
                vh.PopulateUIVertex(ref vertex, i);

                if (m_GradientStyle == GradientStyle.Split)
                    nomalizedPos = localMatrix * s_SplitedCharacterPosition[i % 4] + Offset2;

                else
                    nomalizedPos = localMatrix * vertex.position + Offset2;

                if (DirectionMain == Direction.Diagonal)
                {
                    color = Color.LerpUnclamped(
                        Color.LerpUnclamped(m_Color1, m_Color2, nomalizedPos.x),
                        Color.LerpUnclamped(m_Color3, m_Color4, nomalizedPos.x),
                        nomalizedPos.y);
                }

                else
                    color = Color.LerpUnclamped(m_Color2, m_Color1, nomalizedPos.y);

                vertex.color *= (m_ColorSpace == ColorSpace.Gamma) ? color.gamma
                    : (m_ColorSpace == ColorSpace.Linear) ? color.linear
                    : color;

                vh.SetUIVertex(vertex, i);
            }
        }

        static readonly Vector2[] s_SplitedCharacterPosition = { Vector2.up, Vector2.one, Vector2.right, Vector2.zero };

        struct Matrix2x3
        {
            public float m00, m01, m02, m10, m11, m12;

            public Matrix2x3(Rect rect, float cos, float sin)
            {
                const float center = 0.5f;
                float dx = -rect.xMin / rect.width - center;
                float dy = -rect.yMin / rect.height - center;
                m00 = cos / rect.width;
                m01 = -sin / rect.height;
                m02 = dx * cos - dy * sin + center;
                m10 = sin / rect.width;
                m11 = cos / rect.height;
                m12 = dx * sin + dy * cos + center;
            }

            public static Vector2 operator *(Matrix2x3 m, Vector2 v)
            {
                return new Vector2(
                    (m.m00 * v.x) + (m.m01 * v.y) + m.m02,
                    (m.m10 * v.x) + (m.m11 * v.y) + m.m12
                );
            }
        }
    }
}