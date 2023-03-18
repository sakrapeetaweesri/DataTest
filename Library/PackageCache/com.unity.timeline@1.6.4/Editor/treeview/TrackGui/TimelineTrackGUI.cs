using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using Object = UnityEngine.Object;

namespace UnityEditor.Timeline
{
    class TimelineTrackGUI : TimelineGroupGUI, IClipCurveEditorOwner, IRowGUI
    {
        struct TrackDrawData
        {
            public bool m_AllowsRecording;
            public bool m_ShowTrackBindings;
            public bool m_HasBinding;
            public bool m_IsSubTrack;
            public PlayableBinding m_Binding;
            public Object m_TrackBinding;
            public Texture m_TrackIcon;
            public bool m_HasMarkers;
        }

        static class Styles
        {
            public static readonly GUIContent trackCurvesBtnOnTooltip = DirectorStyles.TrTextContent(string.Empty, "Hide curves view");
            public static readonly GUIContent trackCurvesBtnOffTooltip = DirectorStyles.TrTextContent(string.Empty, "Show curves view");
            public static readonly GUIContent trackMarkerBtnOnTooltip = DirectorStyles.TrTextContent(string.Empty, "Collapse Track Markers");
            public static readonly GUIContent trackMarkerBtnOffTooltip = DirectorStyles.TrTextContent(string.Empty, "Expand Track Markers");

            public static readonly GUIContent kActiveRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "End recording");
            public static readonly GUIContent kInactiveRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "Start recording");
            public static readonly GUIContent kIgnorePreviewRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty, "Recording is disabled: scene preview is ignored for this TimelineAsset");
            public static readonly GUIContent kDisabledRecordButtonTooltip = DirectorStyles.TrTextContent(string.Empty,
                "Recording is not permitted when Track Offsets are set to Auto. Track Offset settings can be changed in the track menu of the base track.");
            public static Texture2D kProblemIcon = DirectorStyles.GetBackgroundImage(DirectorStyles.Instance.warning);
        }

        static GUIContent s_ArmForRecordContentOn;
        static GUIContent s_ArmForRecordContentOff;
        static GUIContent s_ArmForRecordDisabled;

        readonly InfiniteTrackDrawer m_InfiniteTrackDrawer;
        readonly TrackEditor m_TrackEditor;
        readonly GUIContent m_DefaultTrackIcon;
        readonly TrackResizeHandle m_ResizeHandle;

        TrackItemsDrawer m_ItemsDrawer;
        TrackDrawData m_TrackDrawData;
        TrackDrawOptions m_TrackDrawOptions;

        bool m_InlineCurvesSkipped;
        int m_TrackHash = -1;
        int m_BlendHash = -1;
        int m_LastDirtyIndex = -1;

        bool? m_TrackHasAnimatableParameters;
        int m_HeightExtension;

        public override bool expandable
        {
            get { return hasChildren; }
        }

        internal InlineCurveEditor inlineCurveEditor { get; set; }

        public ClipCurveEditor clipCurveEditor { get; private set; }

        public bool inlineCurvesSelected => SelectionManager.IsCurveEditorFocused(this);

        bool IClipCurveEditorOwner.showLoops
        {
            get { return false; }
        }

        TrackAsset IClipCurveEditorOwner.owner
        {
            get { return track; }
        }

        static bool DoesTrackAllowsRecording(TrackAsset track)
        {
            // if the root animation track is in auto mode, recording is not allowed
            var animTrack = TimelineUtility.GetSceneReferenceTrack(track) as AnimationTrack;
            if (animTrack != null)
                return animTrack.trackOffset != TrackOffset.Auto;

            return false;
        }

        bool trackHasAnimatableParameters
        {
            get
            {
                // cache this value to avoid the recomputation
                if (!m_TrackHasAnimatableParameters.HasValue)
                    m_TrackHasAnimatableParameters = track.HasAnyAnimatableParameters() ||
                        track.clips.Any(c => c.HasAnyAnimatableParameters());

                return m_TrackHasAnimatableParameters.Value;
            }
        }

        public bool locked
        {
            get { return track.lockedInHierarchy; }
        }

        public bool showMarkers
        {
            get { return track.GetShowMarkers(); }
        }

        public bool muted
        {
            get { return track.muted; }
        }

        public List<TimelineClipGUI> clips
        {
            get
            {
                return m_ItemsDrawer.clips == null ? new List<TimelineClipGUI>(0) : m_ItemsDrawer.clips;
            }
        }

        TrackAsset IRowGUI.asset { get { return track; } }

        bool showTrackRecordingDisabled
        {
            get
            {
                // if the root animation track is in auto mode, recording is not allowed
                var animTrack = TimelineUtility.GetSceneReferenceTrack(track) as AnimationTrack;
                return animTrack != null && animTrack.trackOffset == TrackOffset.Auto;
            }
        }

        public int heightExtension
        {
            get => m_HeightExtension;
            set => m_HeightExtension = Math.Max(0, value);
        }

        float minimumHeight => m_TrackDrawOptions.minimumHeight <= 0.0f ? TrackEditor.DefaultTrackHeight : m_TrackDrawOptions.minimumHeight;

        public TimelineTrackGUI(TreeViewController tv, TimelineTreeViewGUI w, int id, int depth, TreeViewItem parent, string displayName, TrackAsset sequenceActor)
            : base(tv, w, id, depth, parent, displayName, sequenceActor, false)
        {
            var animationTrack = sequenceActor as AnimationTrack;
            if (animationTrack != null)
                m_InfiniteTrackDrawer = new InfiniteTrackDrawer(new AnimationTrackKeyDataSource(animationTrack));
            else if (sequenceActor.HasAnyAnimatableParameters() && !sequenceActor.clips.Any())
                m_InfiniteTrackDrawer = new InfiniteTrackDrawer(new TrackPropertyCurvesDataSource(sequenceActor));

            UpdateInfiniteClipEditor(w.TimelineWindow);

            var bindings = track.outputs.ToArray();
            m_TrackDrawData.m_HasBinding = bindings.Length > 0;
            if (m_TrackDrawData.m_HasBinding)
                m_TrackDrawData.m_Binding = bindings[0];
            m_TrackDrawData.m_IsSubTrack = IsSubTrack();
            m_TrackDrawData.m_AllowsRecording = DoesTrackAllowsRecording(sequenceActor);
            m_TrackDrawData.m_HasMarkers = track.GetMarkerCount() > 0;
            m_DefaultTrackIcon = TrackResourceCache.GetTrackIcon(track);

            m_TrackEditor = CustomTimelineEditorCache.GetTrackEditor(sequenceActor);
            m_TrackDrawOptions = m_TrackEditor.GetTrackOptions_Safe(track, null);

            m_TrackDrawOptions.errorText = null; // explicitly setting to null for an uninitialized state
            m_ResizeHandle = new TrackResizeHandle(this);
            heightExtension = TimelineWindowViewPrefs.GetTrackHeightExtension(track);

            RebuildGUICacheIfNecessary();
        }

        public override float GetVerticalSpacingBetweenTracks()
        {
            if (track != null && track.isSubTrack)
                return 1.0f; // subtracks have less of a gap than tracks
            return base.GetVerticalSpacingBetweenTracks();
        }

        void UpdateInfiniteClipEditor(TimelineWindow window)
        {
            if (clipCurveEditor != null || track == null || !ShouldShowInfiniteClipEditor())
                return;

            var dataSource = CurveDataSource.Create(this);
            clipCurveEditor = new ClipCurveEditor(dataSource, window, track);
        }

        void DetectTrackChanged()
        {
            if (Event.current.type == EventType.Layout)
            {
                // incremented when a track or it's clips changed
                if (m_LastDirtyIndex != track.DirtyIndex)
                {
                    m_TrackEditor.OnTrackChanged_Safe(track);
                    m_LastDirtyIndex = track.DirtyIndex;
                }
                OnTrackChanged();
            }
        }

        // Called when the source track data, including it's clips have changed has changed.
        void OnTrackChanged()
        {
            // recompute blends if necessary
            int newBlendHash = BlendHash();
            if (m_BlendHash != newBlendHash)
            {
                UpdateClipOverlaps();
                m_BlendHash = newBlendHash;
            }

            RebuildGUICacheIfNecessary();
        }

        void UpdateDrawData(WindowState state)
        {
            if (Event.current.type == EventType.Layout)
            {
                m_TrackDrawData.m_ShowTrackBindings = false;
                m_TrackDrawData.m_TrackBinding = null;

                if (state.editSequence.director != null && showSceneReference)
                {
                    m_TrackDrawData.m_ShowTrackBindings = state.GetWindow().currentMode.ShouldShowTrackBindings(state);
                    m_TrackDrawData.m_TrackBinding = state.editSequence.director.GetGenericBinding(track);
                }

                var lastHeight = m_TrackDrawOptions.minimumHeight;
                m_TrackDrawOptions = m_TrackEditor.GetTrackOptions_Safe(track, m_TrackDrawData.m_TrackBinding);

                m_TrackDrawData.m_HasMarkers = track.GetMarkerCount() > 0;
                m_TrackDrawData.m_AllowsRecording = DoesTrackAllowsRecording(track);
                m_TrackDrawData.m_TrackIcon = m_TrackDrawOptions.icon;
                if (m_TrackDrawData.m_TrackIcon == null)
                    m_TrackDrawData.m_TrackIcon = m_DefaultTrackIcon.image;

                // track height has changed. need to update gui
                if (!Mathf.Approximately(lastHeight, m_TrackDrawOptions.minimumHeight))
                    state.Refresh();
            }
        }

        public override void Draw(Rect headerRect, Rect contentRect, WindowState state)
        {
            DetectTrackChanged();
            UpdateDrawData(state);

            UpdateInfiniteClipEditor(state.GetWindow());

            var trackHeaderRect = headerRect;
            var trackContentRect = contentRect;

            float inlineCurveHeight = contentRect.height - GetTrackContentHeight(state);
            bool hasInlineCurve = inlineCurveHeight > 0.0f;

            if (hasInlineCurve)
            {
                trackHeaderRect.height -= inlineCurveHeight;
                trackContentRect.height -= inlineCurveHeight;
            }

            if (Event.current.type == EventType.Repaint)
            {
                m_TreeViewRect = trackContentRect;
            }

            track.SetCollapsed(!isExpanded);

            RebuildGUICacheIfNecessary();

            // Prevents from drawing outside of bounds, but does not effect layout or markers
            bool isOwnerDrawSucceed = false;

            Vector2 visibleTime = state.timeAreaShownRange;

            if (drawer != null)
                isOwnerDrawSucceed = drawer.DrawTrack(trackContentRect, track, visibleTime, state);

            if (!isOwnerDrawSucceed)
            {
                using (new GUIViewportScope(trackContentRect))
                    DrawBackground(trackContentRect, track, visibleTime, state);

                if (m_InfiniteTrackDrawer != null)
                    m_InfiniteTrackDrawer.DrawTrack(trackContentRect, track, visibleTime, state);

                // draw after user customization so overlay text shows up
                using (new GUIViewportScope(trackContentRect))
                    m_ItemsDrawer.Draw(trackContentRect, state);
            }

            DrawTrackHeader(trackHeaderRect, state);

            if (hasInlineCurve)
            {
                var curvesHeaderRect = headerRect;
                curvesHeaderRect.yMin = trackHeaderRect.yMax;

                var curvesContentRect = contentRect;
                curvesContentRect.yMin = trackContentRect.yMax;

                DrawInlineCurves(curvesHeaderRect, curvesContentRect, state);
            }

            DrawTrackColorKind(headerRect);
            DrawTrackState(contentRect, contentRect, track);
        }

        void DrawInlineCurves(Rect curvesHeaderRect, Rect curvesContentRect, WindowState state)
        {
            if (!track.GetShowInlineCurves())
                return;

            // Inline curves are not within the editor window -- case 952571
            if (!IsInlineCurvesEditorInBounds(ToWindowSpace(curvesHeaderRect), curvesContentRect.height, state))
            {
                m_InlineCurvesSkipped = true;
                return;
            }

            // If inline curves were skipped during the last event; we want to avoid rendering them until
            // the next Layout event. Otherwise, we still get the RTE prevented above when the user resizes
            // the timeline window very fast. -- case 952571
            if (m_InlineCurvesSkipped && Event.current.type != EventType.Layout)
                return;

            m_InlineCurvesSkipped = false;

            if (inlineCurveEditor == null)
                inlineCurveEditor = new InlineCurveEditor(this);


            curvesHeaderRect.x += DirectorStyles.kBaseIndent;
            curvesHeaderRect.width -= DirectorStyles.kBaseIndent;

            inlineCurveEditor.Draw(curvesHeaderRect, curvesContentRect, state);
        }

        static bool IsInlineCurvesEditorInBounds(Rect windowSpaceTrackRect, float inlineCurveHeight, WindowState state)
        {
            var legalHeight = state.windowHeight;
            var trackTop = windowSpaceTrackRect.y;
            var inlineCurveOffset = windowSpaceTrackRect.height - inlineCurveHeight;
            return legalHeight - trackTop - inlineCurveOffset > 0;
        }

        void DrawErrorIcon(Rect position, WindowState state)
        {
            Rect bindingLabel = position;
            bindingLabel.x = position.xMax + 3;
            bindingLabel.width = state.bindingAreaWidth;
            EditorGUI.LabelField(position, m_ProblemIcon);
        }

        void DrawBackground(Rect trackRect, TrackAsset trackAsset, Vector2 visibleTime, WindowState state)
        {
            bool canDrawRecordBackground = IsRecording(state);
            if (canDrawRecordBackground)
            {
                DrawRecordingTrackBackground(trackRect, trackAsset, visibleTime, state);
            }
            else
            {
                Color trackBackgroundColor;

                if (SelectionManager.Contains(track))
                {
                    trackBackgroundColor = state.IsEditingASubTimeline() ?
                        DirectorStyles.Instance.customSkin.colorTrackSubSequenceBackgroundSelected :
                        DirectorStyles.Instance.customSkin.colorTrackBackgroundSelected;
                }
                else
                {
                    trackBackgroundColor = state.IsEditingASubTimeline() ?
                        DirectorStyles.Instance.customSkin.colorTrackSubSequenceBackground :
                        DirectorStyles.Instance.customSkin.colorTrackBackground;
                }

                EditorGUI.DrawRect(trackRect, trackBackgroundColor);
            }
        }

        float InlineCurveHeight()
        {
            return track.GetShowInlineCurves() && CanDrawInlineCurve()
                ? TimelineWindowViewPrefs.GetInlineCurveHeight(track)
                : 0.0f;
        }

        public override float GetHeight(WindowState state)
        {
            var height = GetTrackContentHeight(state);

            if (CanDrawInlineCurve())
                height += InlineCurveHeight();

            return height;
        }

        float GetTrackContentHeight(WindowState state)
        {
            var defaultHeight = Mathf.Min(minimumHeight, TrackEditor.MaximumTrackHeight);
            return (defaultHeight + heightExtension) * state.trackScale;
        }

        static bool CanDrawIcon(GUIContent icon)
        {
            return icon != null && icon != GUIContent.none && icon.image != null;
        }

        bool showSceneReference
        {
            get
            {
         �   ��)))�)))�___�___�___�___�___�___�fff�fff�fff�fff�___�___�www�"""�"""�   �        333�"""�"""��MMM�MMM�    UUU�UUU�����                        ������������                                                                                                ���)III-ddd0���$                                                                                                           �   ��)))�)))�___�___�___�___�___�___�fff�fff�fff�fff�___�___�www�"""�"""�   �        333�"""�"""��MMM�MMM�    UUU�UUU�����                        ������������                                                                                                ���)III-ddd0���$                                                                                                    ����)))�)))����UUU�fff�fff�___�___�___�___�___�___�___�___�___�fff�999�999�   �BBB�BBB�___�UUU�UUU�333�999�999�����MMM�MMM�������������������������UUU���www�                                                                                                ���)III-ddd0���$                                                                                                        fff�fff����999���������___�UUU�UUU�fff�___�___�___�___�___�fff�fff�fff�   �999�999�999�UUU�UUU�MMM�BBB�BBB�)))���)))�___�___�"""�   �   �   �   �   �999�                                                                                                ���)III-ddd0���$                                                                                                        fff�fff����999���������___�UUU�UUU�fff�___�___�___�___�___�fff�fff�fff�   �999�999�999�UUU�UUU�MMM�BBB�BBB�)))���)))�___�___�"""�   �   �   �   �   �999�                                                                                                ���)III-ddd0���$                                                                                                        �����������"""�fff�fff�    ___�___�MMM�fff�fff�___�___�___�___�fff�fff�   �fff�fff�BBB���MMM�UUU�UUU�UUU���)))�   �   ��BBB�BBB�   �   �   �BBB�                                                                                                ���)III-ddd0���$                                                                                                                ����"""�"""�"""�)))�)))�___�        ����UUU�UUU�___�___�___�___�   �   �___�fff�fff�MMM���333�999�999�BBB�333�333�UUU�   �   ��  �___�___�   �   �   �BBB�                                                                                                ���)III-ddd0���$                                                                                                                ����"""�"""�"""�)))�)))�___�        ����UUU�UUU�___�___�___�___�   �   �___�fff�fff�MMM���333�999�999�BBB�333�333�UUU�   �   ��  �___�___�   �   �   �BBB�                                                                                                ���)III-ddd0���$                                                                                                                    ��������www�BBB�BBB�BBB�BBB�BBB�    ������������___�___�   �___�___�___�www�www�UUU���BBB�999�999�333�UUU�UUU�"""�   �   �"""�           �   �   �BBB�                                                                                                ���)III-ddd0���$                                                                                                                            ����������������BBB�BBB�999���BBB�   �   �fff�___�___�fff�UUU�UUU�fff�MMM�MMM�UUU�UUU�UUU�MMM�)))�)))�   ���UUU���   �   �   �"""���������                                                                                        ���)III-ddd0���$                                                                                                                            ����������������BBB�BBB�999���BBB�   �   �fff�___�___�fff�UUU�UUU�fff�MMM�MMM�UUU�UUU�UUU�MMM�)))�)))�   ���UUU���   �   �   �"""���������                                                                                        ���)III-ddd0���$                                                                                                                                ��������333�"""�"""��   �   ��___�___�fff�fff�fff�UUU���    www�www�___�)))�)))�)))����BBB�BBB��   �   �   �   �   ����������                                                                                        ���)III-ddd0���$                                                                                                        ��������������������ff��  3�  3�UUU�www�www�   �   �   ��___�___�fff�fff�fff�___����www�www�___������������������������   �   �   �   �   �   �   ���������                                                                                        ���)III-ddd0���$                                                                                                        ��������������������ff��  3�  3�UUU�www�www�   �   �   ��___�___�fff�fff�fff�___����www�www�___������������������������   �   �   �   �   �   �   ���������                                                                                        ���)III-ddd0���$                                                                                                    ���������  ��  ��  ��������������   �   �   ��UUU�UUU�fff�___�___�www�MMM�MMM�UUU�fff�fff�UUU���������    ���������   �   �   �"""�"""�MMM�UUU�UUU�www���������                                                                                        ���)III-ddd0���$                                                                                                    ������   �   �   �ff����������������������MMM�   �   ��MMM�MMM�fff�___�___�___�fff�fff�fff�fff�fff�UUU���������    ��������www���������������������                                                                                                                ���)III-ddd0���$                                                                                                    ������   �   �   �ff����������������������MMM�   �   ��MMM�MMM�fff�___�___�___�fff�fff�fff�fff�fff�UUU���������    ��������www���������������������                                                                                                                ���)III-ddd0���$                                                                                                    ����999�999�   �UUU�UUU�����                    ����   �   ��)))�)))�www�___�___�___�fff�fff�fff�fff�fff�MMM���������    ������������                                                                                                                                    ���)III-ddd0���$                                                                                                        ��������fff�                                ����BBB�BBB�   ���    ��������MMM�___�___�fff�MMM�MMM�����                                                                                                                                                            ���)III-ddd0���$                                                                                                        ��������fff�                                ����BBB�BBB�   ���    ��������MMM�___�___�fff�MMM�MMM�����                                                                                                                                                            ���)III-ddd0���$                                                                                                                                                        �����������"""�        ����___�___�BBB�UUU�UUU�����                                                                                                                                                            ���)III-ddd0���$                                                                                                                                                                ����BBB�BBB����___���������������������                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                ����BBB�BBB����___���������������������                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                    ��������������������������������                                                                                                                                                                            ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                                                                                                                                                                                                                                                                                                                ���)III-ddd0���$                                                                                   