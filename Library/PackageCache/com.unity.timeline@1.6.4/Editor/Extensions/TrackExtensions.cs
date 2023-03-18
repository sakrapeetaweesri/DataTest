using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

namespace UnityEditor.Timeline
{
    /// <summary>
    /// Extension Methods for Tracks that require the Unity Editor, and may require the Timeline containing the Track to be currently loaded in the Timeline Editor Window.
    /// </summary>
    public static class TrackExtensions
    {
        static readonly double kMinOverlapTime = TimeUtility.kTimeEpsilon * 1000;

        /// <summary>
        /// Queries whether the children of the Track are currently visible in the Timeline Editor.
        /// </summary>
        /// <param name="track">The track asset to query.</param>
        /// <returns>True if the track is collapsed and false otherwise.</returns>
        public static bool IsCollapsed(this TrackAsset track)
        {
            return TimelineWindowViewPrefs.IsTrackCollapsed(track);
        }

        /// <summary>
        /// Sets whether the children of the Track are currently visible in the Timeline Editor.
        /// </summary>
        /// <param name="track">The track asset to collapsed state to modify.</param>
        /// <param name="collapsed">`true` to collapse children, false otherwise.</param>
        /// <remarks> The track collapsed state is not serialized inside the asset and is lost from one checkout of the project to another. </remarks>>
        public static void SetCollapsed(this TrackAsset track, bool collapsed)
        {
            TimelineWindowViewPrefs.SetTrackCollapsed(track, collapsed);
        }

        /// <summary>
        /// Queries whether any parent of the track is collapsed, rendering the track not visible to the user.
        /// </summary>
        /// <param name="track">The track asset to query.</param>
        /// <returns>True if all parents are not collapsed, false otherwise.</returns>
        public static bool IsVisibleInHierarchy(this TrackAsset track)
        {
            var t = track;
            while ((t = t.parent as TrackAsset) != null)
            {
                if (t.IsCollapsed())
                    return false;
            }

            return true;
        }

        internal static AnimationClip GetOrCreateClip(this AnimationTrack track)
        {
            if (track.infiniteClip == null && !track.inClipMode)
                track.CreateInfiniteClip(AnimationTrackRecorder.GetUniqueRecordedClipName(track, AnimationTrackRecorder.kRecordClipDefaultName));

            return track.infiniteClip;
        }

        internal static TimelineClip CreateClip(this TrackAsset track, double time)
        {
            var attr = track.GetType().GetCustomAttributes(typeof(TrackClipTypeAttribute), true);

            if (attr.Length == 0)
                return null;

            if (TimelineWindow.instance.state == null)
                return null;

            if (attr.Length == 1)
            {
                var clipClass = (TrackClipTypeAttribute)attr[0];

                var clip = TimelineHelpers.CreateClipOnTrack(clipClass.inspectedType, track, time);
                return clip;
            }

            return null;
        }

        static bool Overlaps(TimelineClip blendOut, TimelineClip blendIn)
        {
            if (blendIn == blendOut)
                return false;

            if (Math.Abs(blendIn.start - blendOut.start) < TimeUtility.kTimeEpsilon)
            {
                return blendIn.duration >= blendOut.duration;
            }

            return blendIn.start >= blendOut.start && blendIn.start < blendOut.end;
        }

        internal static void ComputeBlendsFromOverlaps(this TrackAsset asset)
        {
            ComputeBlendsFromOverlaps(asset.clips);
        }

        internal static void ComputeBlendsFromOverlaps(TimelineClip[] clips)
        {
            foreach (var clip in clips)
            {
                clip.blendInDuration = -1;
                clip.blendOutDuration = -1;
            }

            Array.Sort(clips, (c1, c2) =>
                Math.Abs(c1.start - c2.start) < TimeUtility.kTimeEpsilon ? c1.duration.CompareTo(c2.duration) : c1.start.CompareTo(c2.start));

            for (var i = 0; i < clips.Length; i++)
            {
                var clip = clips[i];
                if (!clip.SupportsBlending())
                    continue;
                var blendIn = clip;
                TimelineClip blendOut = null;

                var blendOutCandidate = clips[Math.Max(i - 1, 0)];
                if (Overlaps(blendOutCandidate, blendIn))
                    blendOut = blendOutCandidate;

                if (blendOut != null)
                {
                    UpdateClipIntersection(blendOut, blendIn);
                }
            }
        }

        static void UpdateClipIntersection(TimelineClip blendOutClip, TimelineClip blendInClip)
        {
            if (!blendOutClip.SupportsBlending() || !blendInClip.SupportsBlending())
                return;

            if (blendInClip.start - blendOutClip.start < blendOutClip.duration - blendInClip.duration)
                return;

            double duration = Math.Max(0, blendOutClip.start + blendOutClip.duration - blendInClip.start);
            duration = duration <= kMinOverlapTime ? 0 : duration;
            blendOutClip.blendOutDuration = duration;
            blendInClip.blendInDuration = duration;

            var blendInMode = blendInClip.blendInCurveMode;
            var blendOutMode = blendOutClip.blendOutCurveMode;

            if (blendInMode == TimelineClip.BlendCurveMode.Manual && blendOutMode == TimelineClip.BlendCurveMode.Auto)
            {
                blendOutClip.mixOutCurve = CurveEditUtility.CreateMatchingCurve(blendInClip.mixInCurve);
            }
            else if (blendInMode == TimelineClip.BlendCurveMode.Auto && blendOutMode == TimelineClip.BlendCurveMode.Manual)
            {
                blendInClip.mixInCurve = CurveEditUtility.CreateMatchingCurve(blendOutClip.mixOutCurve);
            }
            else if (blendInMode == TimelineClip.BlendCurveMode.Auto && blendOutMode == TimelineClip.BlendCurveMode.Auto)
            {
                blendInClip.mixInCurve = null; // resets to default curves
                blendOutClip.mixOutCurve = null;
            }
        }

        static void RecursiveSubtrackClone(TrackAsset source, TrackAsset duplicate, IExposedPropertyTable sourceTable, IExposedPropertyTable destTable, PlayableAsset assetOwner)
        {
            var subtracks = source.GetChildTracks();
            foreach (var sub in subtracks)
            {
                var newSub = TimelineHelpers.Clone(duplicate, sub, sourceTable, destTable, assetOwner);
                duplicate.AddChild(newSub);
                RecursiveSubtrackClone(sub, newSub, sourceTable, destTable, assetOwner);

                // Call the custom editor on Create
                var customEditor = CustomTimelineEditorCache.GetTrackEditor(newSub);
                customEditor.OnCreate_Safe(newSub, sub);

                // registration has to happen AFTER recursion
                TimelineCreateUtilities.SaveAssetIntoObject(newSub, assetOwner);
                TimelineUndo.RegisterCreatedObjectUndo(newSub, L10n.Tr("Duplicate"));
            }
        }

        internal static TrackAsset Duplicate(this TrackAsset track, IExposedPropertyTable sourceTable, IExposedPropertyTable destTable,
            TimelineAsset destinationTimeline = null)
        {
            if (track == null)
                return null;

            // if the destination is us, clear to avoid bad parenting (case 919421)
            if (destinationTimeline == track.timelineAsset)
                destinationTimeline = null;

            var timelineParent = track.parent as TimelineAsset;
            var trackParent = track.parent as TrackAsset;
            if (timelineParent == null && trackParent == null)
            {
                Debug.LogWarning("Cannot duplicate track because it is not parented to known type");
                return null;
            }

            // Determine who the final parent is. If we are pasting into another track, it's always the timeline.
            //  Otherwise it's the original parent
            PlayableAsset finalParent = destinationTimeline != null ? destinationTimeline : track.parent;

            // grab the list of tracks to generate a name from (923360) to get the list of names
            // no need to do this part recursively
            var finalTrackParent = finalParent as TrackAsset;
            var finalTimelineAsset = finalParent as TimelineAsset;
            var otherTracks = (finalTimelineAsset != null) ? finalTimelineAsset.trackObjects : finalTrackParent.subTracksObjects;

            // Important to create the new objects before pushing the original undo, or redo breaks the
            //  sequence
            var newTrack = TimelineHelpers.Clone(finalParent, track, sourceTable, destTable, finalParent);
            newTrack.name = TimelineCreateUtilities.GenerateUniqueActorName(otherTracks, newTrack.name);

            RecursiveSubtrackClone(track, newTrack, sourceTable, destTable, finalParent);
            TimelineCreateUtilities.SaveAssetIntoObject(newTrack, finalParent);
            TimelineUndo.RegisterCreatedObjectUndo(newTrack, L10n.Tr("Duplicate"));
            UndoExtensions.RegisterPlayableAsset(finalParent, L10n.Tr("Duplicate"));

            if (destinationTimeline != null) // other timeline
                destinationTimeline.AddTrackInternal(newTrack);
            else if (timelineParent != null) // this timeline, no parent
                ReparentTracks(new List<TrackAsset> { newTrack }, timelineParent, timelineParent.GetRootTracks().Last(), false);
            else // this timeline, with parent
                trackParent.AddChild(newTrack);

            // Call the custom editor. this check prevents the call when copying to the clipboard
            if (destinationTimeline == null || destinationTimeline == TimelineEditor.inspectedAsset)
            {
                var customEditor = CustomTimelineEditorCache.GetTrackEditor(newTrack);
                customEditor.OnCreate_Safe(newTrack, track);
            }

            return newTrack;
        }

        // Reparents a list of tracks to a new parent
        //  the new parent cannot be null (has to be track asset or sequence)
        //  the insertAfter can be null (will not reorder)
        internal static bool ReparentTracks(List<TrackAsset> tracksToMove, PlayableAsset targetParent,
            TrackAsset insertMarker = null, bool insertBefore = false)
        {
            var targetParentTrack = targetParent as TrackAsset;
            var targetSequenceTrack = targetParent as TimelineAsset;

            if (tracksToMove == null || tracksToMove.Count == 0 || (targetParentTrack == null && targetSequenceTrack == null))
                return false;

            // invalid parent type on a track
            if (targetParentTrack != null && tracksToMove.Any(x => !TimelineCreateUtilities.ValidateParentTrack(targetParentTrack, x.GetType())))
                return false;

            // no valid tracks means this is simply a rearrangement
            var validTracks = tracksToMove.Where(x => x.parent != targetParent).ToList();
            if (insertMarker == null && !validTracks.Any())
                return false;

            var parents = validTracks.Select(x => x.parent).Where(x => x != null).Distinct().ToList();
            // push the current state of the tracks that will change
            foreach (var p in parents)
            {
                UndoExtensions.RegisterPlayableAsset(p, "Reparent");
            }
            UndoExtensions.RegisterTracks(validTracks, "Reparent");
            UndoExtensions.RegisterPlayableAsset(targetParent, "Reparent");

            // need to reparent tracks first, before moving them.
            foreach (var t in validTracks)
            {
                if (t.parent != targetParent)
                {
                    TrackAsset toMoveParent = t.parent as TrackAsset;
                    TimelineAsset toMoveTimeline = t.parent as TimelineAsset;
                    if (toMoveTimeline != null)
                    {
                        toMoveTimeline.RemoveTrack(t);
                    }
                    else if (toMoveParent != null)
                    {
                        toMoveParent.RemoveSubTrack(t);
                    }

                    if (targetParentTrack != null)
                    {
                        targetParentTrack.AddChild(t);
                        targetParentTrack.SetCollapsed(false);
                    }
                    else
                    {
                        targetSequenceTrack.AddTrackInternal(t);
                    }
                }
            }


            if (insertMarker != null)
            {
                // re-ordering track. This is using internal APIs, so invalidation of the tracks must be done manually to avoid
                //  cache mismatches
                var children = targetParentTrack != null ? targetParentTrack.subTracksObjects : targetSequenceTrack.trackObjects;
                TimelineUtility.ReorderTracks(children, tracksToMove, insertMarker, insertBefore);
                if (targetParentTrack != null)
                    targetParentTrack.Invalidate();
                if (insertMarker.timelineAsset != null)
                    insertMarker.timelineAsset.Invalidate();
            }

            return true;
        }

        internal static IEnumerable<TrackAsset> FilterTracks(IEnumerable<TrackAsset> tracks)
        {
            var nTracks = tracks.Count();
            // Duplicate is recursive. If should not have parent and child in the list
            var hash = new HashSet<TrackAsset>(tracks);
            var take = new Dictionary<TrackAsset, bool>(nTracks);

            foreach (var track in tracks)
            {
                var parent = track.parent as TrackAsset;
                var foundParent = false;
                // go up the hierarchy
                while (parent != null && !foundParent)
                {
                    if (hash.Contains(parent))
                    {
                        foundParent = true;
                    }

                    parent = parent.parent as TrackAsset;
                }

                take[track] = !foundParent;
            }

            foreach (var track in tracks)
            {
                if (take[track])
                    yield return track;
            }
        }

        internal static bool GetShowMarkers(this TrackAsset track)
        {
            return TimelineWindowViewPrefs.IsShowMarkers(track);
        }

        internal static void SetShowMarkers(this TrackAsset track, bool collapsed)
        {
            TimelineWindowViewPrefs.SetTrackShowMarkers(track, collapsed);
        }

        internal static bool GetShowInlineCurves(this TrackAsset track)
        {
            return TimelineWindowViewPrefs.GetShowInlineCurves(track);
        }

        internal static void SetShowInlineCurves(this TrackAsset track, bool inlineOn)
        {
            TimelineWindowViewPrefs.SetShowInlineCurves(track, inlineOn);
        }

        internal static bool ShouldShowInfiniteClipEditor(this AnimationTrack track)
        {
            return track != null && !track.inClipMode && track.infiniteClip != null;
        }

        // Special method to remove a track that is in a broken state. i.e. the script won't load
        internal static bool RemoveBrokenTrack(PlayableAsset parent, ScriptableObject track)
        {
            var parentTrack = parent as TrackAsset;
            var parentTimeline = parent as TimelineAsset;

            if (parentTrack == null && parentTimeline == null)
                throw new ArgumentException("parent is not a valid parent type", "parent");

            // this object must be a Unity null, but not actually null;
            object trackAsObject = track;
            if (trackAsObject == null || track as TrackAsset != null) // yes, this is correct
                throw new ArgumentException("track is not in a broken state");

            // this belongs to a parent track
            if (p����������������������������������������������20��EC��������������������������������������������������������������������������������������������������������������������������������������������������������������������������&$��-+��;9�����=���	                                                                                    ���-���]'%��-+��HF��������������������������������������������������������������������������������������������������������������)'������������������������������������������������������������������������������������������������������������������������������������������������������ ��-+��20�����8���                                                                                    ���:nl�z'%��-+��fe����������������������������������������������������������������������������������������������������������&$��-+��!��������������������������������������������������������������������������������������������������������������������������������������������������$"��-+��+)�����;���                                                                                ������=TR�+)��(&������������������������������������������������������������������������������������������������ ��'%��20��-+��-+��ML��������������������������������������������������������������������������������������������������������������������������������������������������31��-+��)'�����I���%                                                                                ������>=;��-+��$"����������������������������������������������������������������������������������������������)'��-+��-+��-+��-+��-+��-+��97��US������������������������������������������������������������������������������������������������������������������������������������������GF��-+��%#�����Z���0                                                                                ������B+*��-+��)'������������������������������%#��ZY����������������������������������������������������������|z��FE��,*��-+��-+��-+��-+��-+��-+��)'��$"��+)��EC��ki��������������������������������������QP������������������������������������������������������������������������������gf��-+��$"�����v���;                                                                                ���'���K��-+��53��������������������������64��-+��%$������������������������������������������������������������������*(��-+��64��\[��42��'%��'%��+)��-+��-+��-+��-+��*(�� ��)'��IG������������������$#��-+��;:������������������������������������������������������������������������������*(��)'��]]����?                                                                                ���4���g$"��-+��RP����������42��(&��(&��54��+)��-+��53��������������������������������������������������������������%#��-+��%#��������������������������HG��(&��" ��+)��,*��-+��-+��-+��*(��(&��)'��/-��-+��-+��;9������������������������������������������������������������������������������'%��,*��BA����?���                                                                        ������>a_�+)��-+��nm���������� ��-+��-+��-+��-+��-+��-+��*(��<;��a`����������������������������������������������NM��-+��-+��__������������������������������������������wv��CA��'%��!��+)��-+��-+��-+��-+��-+��-+��53��������������������������������������������������������������������������'%��-+��53�����A���                                                                        ������AMK�,*��+)������������������XV��64��,*��-+��-+��-+��-+��-+��-+��,*��" ��#!��75��hg��������������������������&$��-+��$"������������������������������������������������������������������/-��-+��-+��*(��+)��-+��HG��������������������������������������������������������������������������'%��-+��0.�����@���                                                                        ������9>=��-+��" ��������������������������+)��,*��KI��SR��.,�� ��)'��,*��-+��-+��-+��,*��(&��(&��-+��DB��ig��0.��-+��)'����������������������������������������������������������������������)'��-+��&$������~��DC��rq��������������������������������������������������������������������������(&��-+��('�����?���                                                                        ������:0.��-+��!����������������������10��-+��'%��������������������������VT��0.�� ��&$��-+��-+��-+��-+��,*��-+��-+��,*��ON��yx����������������������������������������������������������&$��-+��(&��{{������������������������������������������������������������������������������������������42��-+��&$�����H���)                                                                        ���%���F$"��-+��.,����������������������'%��-+��@>������������������������������������������on��FD��*(��-+��-+��-+��-+��-+��-+��HG������������������������������������������������������&$��-+��+)��UT����������������������������������������������������������������������������������������������KI��-+��(&�����]���0                                                                        ���5���a ��-+��JH������������������LJ��-+��'%����������������������������������������������