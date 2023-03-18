using System;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [AddComponentMenu("UI/Slider", 34)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    /// <summary>
    /// A standard slider that can be moved between a minimum and maximum value.
    /// </summary>
    /// <remarks>
    /// The slider component is a Selectable that controls a fill, a handle, or both. The fill, when used, spans from the minimum value to the current value while the handle, when used, follow the current value.
    /// The anchors of the fill and handle RectTransforms are driven by the Slider. The fill and handle can be direct children of the GameObject with the Slider, or intermediary RectTransforms can be placed in between for additional control.
    /// When a change to the slider value occurs, a callback is sent to any registered listeners of UI.Slider.onValueChanged.
    /// </remarks>
    public class Slider : Selectable, IDragHandler, IInitializePotentialDragHandler, ICanvasElement
    {
        /// <summary>
        /// Setting that indicates one of four directions.
        /// </summary>
        public enum Direction
        {
            /// <summary>
            /// From the left to the right
            /// </summary>
            LeftToRight,

            /// <summary>
            /// From the right to the left
            /// </summary>
            RightToLeft,

            /// <summary>
            /// From the bottom to the top.
            /// </summary>
            BottomToTop,

            /// <summary>
            /// From the top to the bottom.
            /// </summary>
            TopToBottom,
        }

        [Serializable]
        /// <summary>
        /// Event type used by the UI.Slider.
        /// </summary>
        public class SliderEvent : UnityEvent<float> {}

        [SerializeField]
        private RectTransform m_FillRect;

        /// <summary>
        /// Optional RectTransform to use as fill for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI;  // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform"(Child of FillArea).
        ///     public RectTransform newFillRect;
        ///
        ///     //Deactivates the old FillRect and assigns a new one.
        ///     void Start()
        ///     {
        ///         mainSlider.fillRect.gameObject.SetActive(false);
        ///         mainSlider.fillRect = newFillRect;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public RectTransform fillRect { get { return m_FillRect; } set { if (SetPropertyUtility.SetClass(ref m_FillRect, value)) {UpdateCachedReferences(); UpdateVisuals(); } } }

        [SerializeField]
        private RectTransform m_HandleRect;

        /// <summary>
        /// Optional RectTransform to use as a handle for the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///     //Reference to new "RectTransform" (Child of "Handle Slide Area").
        ///     public RectTransform handleHighlighted;
        ///
        ///     //Deactivates the old Handle, then assigns and enables the new one.
        ///     void Start()
        ///     {
        ///         mainSlider.handleRect.gameObject.SetActive(false);
        ///         mainSlider.handleRect = handleHighlighted;
        ///         mainSlider.handleRect.gameObject.SetActive(true);
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public RectTransform handleRect { get { return m_HandleRect; } set { if (SetPropertyUtility.SetClass(ref m_HandleRect, value)) { UpdateCachedReferences(); UpdateVisuals(); } } }

        [Space]

        [SerializeField]
        private Direction m_Direction = Direction.LeftToRight;

        /// <summary>
        /// The direction of the slider, from minimum to maximum value.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //Changes the direction of the slider.
        ///         if (mainSlider.direction == Slider.Direction.BottomToTop)
        ///         {
        ///             mainSlider.direction = Slider.Direction.TopToBottom;
        ///         }
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public Direction direction { get { return m_Direction; } set { if (SetPropertyUtility.SetStruct(ref m_Direction, value)) UpdateVisuals(); } }

        [SerializeField]
        private float m_MinValue = 0;

        /// <summary>
        /// The minimum allowed value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     void Start()
        ///     {
        ///         // Changes the minimum value of the slider to 10;
        ///         mainSlider.minValue = 10;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public float minValue { get { return m_MinValue; } set { if (SetPropertyUtility.SetStruct(ref m_MinValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private float m_MaxValue = 1;

        /// <summary>
        /// The maximum allowed value of the slider.
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     void Start()
        ///     {
        ///         // Changes the max value of the slider to 20;
        ///         mainSlider.maxValue = 20;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public float maxValue { get { return m_MaxValue; } set { if (SetPropertyUtility.SetStruct(ref m_MaxValue, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        private bool m_WholeNumbers = false;

        /// <summary>
        /// Should the value only be allowed to be whole numbers?
        /// </summary>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public Slider mainSlider;
        ///
        ///     public void Start()
        ///     {
        ///         //sets the slider's value to accept whole numbers only.
        ///         mainSlider.wholeNumbers = true;
        ///     }
        /// }
        /// ]]>
        ///</code>
        /// </example>
        public bool wholeNumbers { get { return m_WholeNumbers; } set { if (SetPropertyUtility.SetStruct(ref m_WholeNumbers, value)) { Set(m_Value); UpdateVisuals(); } } }

        [SerializeField]
        protected               ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������������������   �                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ��������������������������������������������������������������������������������   �   �   �   �   �   �   �   �   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    ��������������������������������������������������������������������������������������������������������������������                                                                                                                                        ���)III-ddd0���$                                                                                                                    ��������������������������������������������������������������������������������������������                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ��������������������������������������������������������������������������������������������                                                                                                                                                                ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������   �   �   �   �   �   �   �   �   �                                                                                                                                            ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                            ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� �������                                                                                                                                            ���)III-ddd0���$                                                                                                                    �������������������������������������������������������������������������������� ��� ��� ��� ��� ��� ��� ��� ���   �                                                                                                                                        ���)III-ddd0���$                                                                                                                    ������������������������������������������������������������������������������������ ��� ��� ��� ��� ��� ��� ��� �����������                                                                                                                                ���)III-ddd0���$                                                                                                                    ������������������������������������������������������������������������������������ ��� ��� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                            ���)III-ddd0���$                                                                                                                    ����������������������������������������������������������������������������        ���� ��� ��� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                        ���)III-ddd0���$                                                                                                                    ��������������������������������������������������������������������                    �������� ��� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                    ���)III-ddd0���$                                                                                                                                                                                                                    ���� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                    ���)III-ddd0���$                                                                                                                                                                                                                        ���� ��� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                            ���� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                        ��������������������������������                    ���� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                        ���� ��� ��� ��� ��� ��� �������                    ���� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                        ���� ��� ��� ��� ��� ��� �������                    ���� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                        ���� ��� ��� ��� ��� ��� ��� �������                ���� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                            ���� ��� ��� ��� ��� ��� ��� ������������������� ��� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                ���)III-ddd0���$                                                                                                                                                                            ���� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                    ���)III-ddd0���$                                                                                                                                                                                ���� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ���www�                                                                                                                    ���)III-ddd0���$                                                                                                                                                                                    ���� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                        ���)III-ddd0���$                                                                                                                                                                                        ���� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� ��� �������                                                                                                                            ���)III-ddd0���$                                                                                                                                                                                            �������� ��� ��� ��� ��� ��� ��� ��� ��� �����������                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                    ������������������������������������                                                                                                                                        ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$