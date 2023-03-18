using System;
using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityEngine.UI
{
    /// <summary>
    /// Utility class for creating default implementations of builtin UI controls.
    /// </summary>
    /// <remarks>
    /// The recommended workflow for using UI controls with the UI system is to create a prefab for each type of control and instantiate those when needed. This way changes can be made to the prefabs which immediately have effect on all used instances.
    ///
    /// However, in certain cases there can be reasons to create UI controls entirely from code. The DefaultControls class provide methods to create each of the builtin UI controls. The resulting objects are the same as are obtained from using the corresponding UI menu entries in the GameObject menu in the Editor.
    ///
    /// An example use of this is creating menu items for custom new UI controls that mimics the ones that are builtin in Unity. Some such UI controls may contain other UI controls. For example, a scroll view contains scrollbars.By using the DefaultControls methods to create those parts, it is ensured that they are identical in look and setup to the ones provided in the menu items builtin with Unity.
    ///
    /// Note that the details of the setup of the UI controls created by the methods in this class may change with later revisions of the UI system.As such, they are not guaranteed to be 100% backwards compatible. It is recommended not to rely on the specific hierarchies of the GameObjects created by these methods, and limit your code to only interface with the root GameObject created by each method.
    /// </remarks>
    public static class DefaultControls
    {
        static IFactoryControls m_CurrentFactory = DefaultRuntimeFactory.Default;
        public static IFactoryControls factory
        {
            get { return m_CurrentFactory; }
#if UNITY_EDITOR
            set { m_CurrentFactory = value; }
#endif
        }

        /// <summary>
        /// Factory interface to create a GameObject in this class.
        /// It is necessary to use this interface in the whole class so MenuOption and Editor can work using ObjectFactory and default Presets.
        /// </summary>
        /// <remarks>
        /// The only available method is CreateGameObject.
        /// It needs to be called with every Components the created Object will need because of a bug with Undo and RectTransform.
        /// Adding a UI component on the created GameObject may crash if done after Undo.SetTransformParent,
        /// So it's better to prevent such behavior in this class by asking for full creation with all the components.
        /// </remarks>
        public interface IFactoryControls
        {
            GameObject CreateGameObject(string name, params Type[] components);
        }

        private class DefaultRuntimeFactory : IFactoryControls
        {
            public static IFactoryControls Default = new DefaultRuntimeFactory();

            public GameObject CreateGameObject(string name, params Type[] components)
            {
                return new GameObject(name, components);
            }
        }

        /// <summary>
        /// Object used to pass resources to use for the default controls.
        /// </summary>
        public struct Resources
        {
            /// <summary>
            /// The primary sprite to be used for graphical UI elements, used by the button, toggle, and dropdown controls, among others.
            /// </summary>
            public Sprite standard;

            /// <summary>
            /// Sprite used for background elements.
            /// </summary>
            public Sprite background;

            /// <summary>
            /// Sprite used as background for input fields.
            /// </summary>
            public Sprite inputField;

            /// <summary>
            /// Sprite used for knobs that can be dragged, such as on a slider.
            /// </summary>
            public Sprite knob;

            /// <summary>
            /// Sprite used for representation of an "on" state when present, such as a checkmark.
            /// </summary>
            public Sprite checkmark;

            /// <summary>
            /// Sprite used to indicate that a button will open a dropdown when clicked.
            /// </summary>
            public Sprite dropdown;

            /// <summary>
            /// Sprite used for masking purposes, for example to be used for the viewport of a scroll view.
            /// </summary>
            public Sprite mask;
        }

        private const float  kWidth       = 160f;
        private const float  kThickHeight = 30f;
        private const float  kThinHeight  = 20f;
        private static Vector2 s_ThickElementSize       = new Vector2(kWidth, kThickHeight);
        private static Vector2 s_ThinElementSize        = new Vector2(kWidth, kThinHeight);
        private static Vector2 s_ImageElementSize       = new Vector2(100f, 100f);
        private static Color   s_DefaultSelectableColor = new Color(1f, 1f, 1f, 1f);
        private static Color   s_PanelColor             = new Color(1f, 1f, 1f, 0.392f);
        private static Color   s_TextColor              = new Color(50f / 255f, 50f / 255f, 50f / 255f, 1f);

        // Helper methods at top

        private static GameObject CreateUIElementRoot(string name, Vector2 size, params Type[] components)
        {
            GameObject child = factory.CreateGameObject(name, components);
            RectTransform rectTransform = child.GetComponent<RectTransform>();
            rectTransform.sizeDelta = size;
            return child;
        }

        private static GameObject CreateUIObject(string name, GameObject parent, params Type[] components)
        {
            GameObject go = factory.CreateGameObject(name, components);
            SetParentAndAlign(go, parent);
            return go;
        }

        private static void SetDefaultTextValues(Text lbl)
        {
            // Set text values we want across UI elements in default controls.
            // Don't set values which are the same as the default values for the Text component,
            // since there's no point in that, and it's good to keep them as consistent as possible.
            lbl.color = s_TextColor;

            // Reset() is not called when playing. We still want the default font to be assigned
            // We may have a font assigned from default preset. We shouldn't override it
            if (lbl.font == null)
                lbl.AssignDefaultFont();
        }

        private static void SetDefaultColorTransitionValues(Selectable slider)
        {
            ColorBlock colors = slider.colors;
            colors.highlightedColor = new Color(0.882f, 0.882f, 0.882f);
            colors.pressedColor     = new Color(0.698f, 0.698f, 0.698f);
            colors.disabledColor    = new Color(0.521f, 0.521f, 0.521f);
        }

        private static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

#if UNITY_EDITOR
            Undo.SetTransformParent(child.transform, parent.transform, "");
#else
            child.transform.SetParent(parent.transform, false);
#endif
            SetLayerRecursively(child, parent.layer);
        }

        private static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        /// <summary>
        /// Create the basic UI Panel.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Image
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreatePanel(Resources resources)
        {
            GameObject panelRoot = CreateUIElementRoot("Panel", s_ThickElementSize, typeof(Image));

            // Set RectTransform to stretch
            RectTransform rectTransform = panelRoot.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;

            Image image = panelRoot.GetComponent<Image>();
            image.sprite = resources.background;
            image.type = Image.Type.Sliced;
            image.color = s_PanelColor;

            return panelRoot;
        }

        /// <summary>
        /// Create the basic UI button.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Button
        ///         -Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateButton(Resources resources)
        {
            GameObject buttonRoot = CreateUIElementRoot("Button (Legacy)", s_ThickElementSize, typeof(Image), typeof(Button));

            GameObject childText = CreateUIObject("Text (Legacy)", buttonRoot, typeof(Text));

            Image image = buttonRoot.GetComponent<Image>();
            image.sprite = resources.standard;
            image.type = Image.Type.Sliced;
            image.color = s_DefaultSelectableColor;

            Button bt = buttonRoot.GetComponent<Button>();
            SetDefaultColorTransitionValues(bt);

            Text text = childText.GetComponent<Text>();
            text.text = "Button";
            text.alignment = TextAnchor.MiddleCenter;
            SetDefaultTextValues(text);

            RectTransform textRectTransform = childText.GetComponent<RectTransform>();
            textRectTransform.anchorMin = Vector2.zero;
            textRectTransform.anchorMax = Vector2.one;
            textRectTransform.sizeDelta = Vector2.zero;

            return buttonRoot;
        }

        /// <summary>
        /// Create the basic UI Text.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Text
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateText(Resources resources)
        {
            GameObject go = CreateUIElementRoot("Text (Legacy)", s_ThickElementSize, typeof(Text));

            Text lbl = go.GetComponent<Text>();
            lbl.text = "New Text";
            SetDefaultTextValues(lbl);

            return go;
        }

        /// <summary>
        /// Create the basic UI Image.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Image
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("Image", s_ImageElementSize, typeof(Image));
            return go;
        }

        /// <summary>
        /// Create the basic UI RawImage.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     RawImage
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateRawImage(Resources resources)
        {
            GameObject go = CreateUIElementRoot("RawImage", s_ImageElementSize, typeof(RawImage));
            return go;
        }

        /// <summary>
        /// Create the basic UI Slider.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Slider
        ///         - Background
        ///         - Fill Area
        ///             - Fill
        ///         - Handle Slide Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateSlider(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject root = CreateUIElementRoot("Slider", s_ThinElementSize, typeof(Slider));

            GameObject background = CreateUIObject("Background", root, typeof(Image));
            GameObject fillArea = CreateUIObject("Fill Area", root, typeof(RectTransform));
            GameObject fill = CreateUIObject("Fill", fillArea, typeof(Image));
            GameObject handleArea = CreateUIObject("Handle Slide Area", root, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", handleArea, typeof(Image));

            // Background
            Image backgroundImage = background.GetComponent<Image>();
            backgroundImage.sprite = resources.background;
            backgroundImage.type = Image.Type.Sliced;
            backgroundImage.color = s_DefaultSelectableColor;
            RectTransform backgroundRect = background.GetComponent<RectTransform>();
            backgroundRect.anchorMin = new Vector2(0, 0.25f);
            backgroundRect.anchorMax = new Vector2(1, 0.75f);
            backgroundRect.sizeDelta = new Vector2(0, 0);

            // Fill Area
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = new Vector2(0, 0.25f);
            fillAreaRect.anchorMax = new Vector2(1, 0.75f);
            fillAreaRect.anchoredPosition = new Vector2(-5, 0);
            fillAreaRect.sizeDelta = new Vector2(-20, 0);

            // Fill
            Image fillImage = fill.GetComponent<Image>();
            fillImage.sprite = resources.standard;
            fillImage.type = Image.Type.Sliced;
            fillImage.color = s_DefaultSelectableColor;

            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.sizeDelta = new Vector2(10, 0);

            // Handle Area
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.sizeDelta = new Vector2(-20, 0);
            handleAreaRect.anchorMin = new Vector2(0, 0);
            handleAreaRect.anchorMax = new Vector2(1, 1);

            // Handle
            Image handleImage = handle.GetComponent<Image>();
            handleImage.sprite = resources.knob;
            handleImage.color = s_DefaultSelectableColor;

            RectTransform handleRect = handle.GetComponent<RectTransform>();
            handleRect.sizeDelta = new Vector2(20, 0);

            // Setup slider component
            Slider slider = root.GetComponent<Slider>();
            slider.fillRect = fill.GetComponent<RectTransform>();
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.targetGraphic = handleImage;
            slider.direction = Slider.Direction.LeftToRight;
            SetDefaultColorTransitionValues(slider);

            return root;
        }

        /// <summary>
        /// Create the basic UI Scrollbar.
        /// </summary>
        /// <remarks>
        /// Hierarchy:
        /// (root)
        ///     Scrollbar
        ///         - Sliding Area
        ///             - Handle
        /// </remarks>
        /// <param name="resources">The resources to use for creation.</param>
        /// <returns>The root GameObject of the created element.</returns>
        public static GameObject CreateScrollbar(Resources resources)
        {
            // Create GOs Hierarchy
            GameObject scrollbarRoot = CreateUIElementRoot("Scrollbar", s_ThinElementSize, typeof(Image), typeof(Scrollbar));

            GameObject sliderArea = CreateUIObject("Sliding Area", scrollbarRoot, typeof(RectTransform));
            GameObject handle = CreateUIObject("Handle", sliderArea, typeof(Image));

            ImagW��W��W�$�a�3�j�3�j�3�j�3�j�3�j�3�j��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������W��W��W��W�$�a�3�j�3�j�3�j�3�j�3�j�3�j�3�j��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������W��W��W�$�a�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������W��W�$�a�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������W�$�a�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������$�a�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���g���g���g���g���g���Z���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���g���g���g���g���Z���M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���g���g���g���Z���M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���g���g���Z���M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���g���Z���M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���g���Z���M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���g���Z���M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���g���Z���M�}�M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���g���Z���M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���g���Z���M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������g���Z���M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������Z���M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�M�}�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�#�`�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�#�`��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�#�`��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�3�j�3�j�#�`��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�3�j�#�`��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�3�j�#�`��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�3�j�#�`��W��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�3�j�#�`��W��W��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�3�j�#�`��W��W��W��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�3�j�#�`��W��W��W��W��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������3�j�#�`��W��W��W��W��W��W��W��W��W��W�������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������#�`��W��W��W��W��W��W��W��W��W��W��W�����������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�|M�|M�|M�|M�|M�|M�|M�b<�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�
>%�
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 ���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�|M�|M�|M�|M�|M�|M�kB�b<�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�G+�
=%�
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|M�|M�|M�|M�|M�|M�|M�|M�|M�kB�kB�b<�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�G+�G+�
>%�
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�|M�|M�|M�|M�kB�kB�kB�b<�Y6�Y6�Y6�Y6�Y6�Y6�Y6�Y6�G+�G+�G+�
>%�
4 �
4 �
4 �
4 �
4 �
4 �
4 �
4 �#�#�#���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�|M�|M�|M�kB�kB�kB�kB�b<�Y6�Y6�Y6�Y6�Y6�Y6�Y6�G+�G+�G+�G+�
=%�
4 �
4 �
4 �
4 �
4 �
4 �
4 �#�#�#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|M�|M�|M�|M�|M�|M�kB�kB�kB�kB�kB�b<�Y6�Y6�Y6�Y6�Y6�Y6�G+�G+�G+�G+�G+�
>%�
4 �
4 �
4 �
4 �
4 �
4 �#�#�#�#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�|M�kB�kB�kB�kB�kB�kB�b<�Y6�Y6�Y6�Y6�Y6�G+�G+�G+�G+�G+�G+�
>%�
4 �
4 �
4 �
4 �
4 �#�#�#�#�#�#���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�|M�|M�kB�kB�kB�kB�kB�kB�kB�b<�Y6�Y6�Y6�Y6�G+�G+�G+�G+�G+�G+�G+�
=%�
4 �
4 �
4 �
4 �#�#�#�#�#�#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|M�|M�|M�kB�kB�kB�kB�kB�kB�kB�kB�b<�Y6�Y6�Y6�G+�G+�G+�G+�G+�G+�G+�G+�
>%�
4 �
4 �
4 �#�#�#�#�#�#�#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�|M�kB�kB�kB�kB�kB�kB�kB�kB�kB�b<�Y6�Y6�G+�G+�G+�G+�G+�G+�G+�G+�G+�
>%�
4 �
4 �#�#�#�#�#�#�#�#�#���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������|N�kB�kB�kB�kB�kB�kB�kB�kB�kB�kB�b<�Y6�G+�G+�G+�G+�G+�G+�G+�G+�G+�G+�
=%�
4 �#�#�#�#�#�#�#�#�#�
&���������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������kB�kB�kB�kB�kB�kB�kB�kB�kB�kB�kB�b<�G+�G+�G+�G+�G+�G+�G+�G+�G+�G+�G+�
>%�#�#�#�#�#�#�#�#�#�#�
&��������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������������