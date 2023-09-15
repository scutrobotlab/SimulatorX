using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEditor;

namespace Smooth
{
    [CustomEditor(typeof(SmoothSyncMirror))]
    public class SmoothSyncMirrorEditor : Editor
    {
        bool showExtrapolation = false;
        bool showThresholds = false;
        bool showWhatToSync = false;
        bool showCompressions = false;
        bool showMiscellaneous = false;
        public override void OnInspectorGUI()
        {
            SmoothSyncMirror myTarget = (SmoothSyncMirror)target;

            if (myTarget.childObjectToSync)
            {
                Color oldColor = GUI.contentColor;
                GUI.contentColor = Color.green;
                EditorGUILayout.LabelField("Syncing child", myTarget.childObjectToSync.name);
                GUI.contentColor = oldColor;
            }

            GUIContent contentWhenToUpdateTransform = new GUIContent("When to Update Transform", "Update will have smoother results but FixedUpdate may be better for physics.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("whenToUpdateTransform"), contentWhenToUpdateTransform);
            GUIContent contentInterpolationBackTime = new GUIContent("Interpolation Back Time", "How much time in the past non-owned objects should be. This is so if you hit a latency spike, you still have a buffer of the interpolationBackTime of known States before you start extrapolating into the unknown. Increasing will make interpolation more likely to be used, decreasing will make extrapolation more likely to be used. Keep above 1/SendRate to attempt to always interpolate. In seconds.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("interpolationBackTime"), contentInterpolationBackTime);

            GUIContent contentSendRate = new GUIContent("Send Rate", "How many times per second to send network updates.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sendRate"), contentSendRate);
            GUIContent contentTimeCorrectionSpeed = new GUIContent("Time Correction Speed", "The estimated owner time will shift by at most this amount per second. Lower values will be smoother but may take time to adjust to larger jumps in latency. Keep this lower than ~.5 unless you are having serious latency issues.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("timeCorrectionSpeed"), contentTimeCorrectionSpeed);
            GUIContent contentPositionLerpSpeed = new GUIContent("Position Easing Speed", "How fast to ease to the new position on non-owned objects. 0 is never, 1 is instant.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("positionLerpSpeed"), contentPositionLerpSpeed);
            GUIContent contentRotationLerpSpeed = new GUIContent("Rotation Easing Speed", "How fast to ease to the new rotation on non-owned objects. 0 is never, 1 is instant.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("rotationLerpSpeed"), contentRotationLerpSpeed);
            GUIContent contentScaleLerpSpeed = new GUIContent("Scale Easing Speed", "How fast to ease to the new scale on non-owned objects. 0 is never, 1 is instant.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("scaleLerpSpeed"), contentScaleLerpSpeed);
            //GUIContent contentNetworkChannel = new GUIContent("Network Channel", "The channel to send network updates on.");
            //EditorGUILayout.PropertyField(serializedObject.FindProperty("networkChannel"), contentNetworkChannel);

            GUIContent contentChildObjectToSync = new GUIContent("Child Object to Sync", "Set this to sync a child object, leave blank to sync this object. Must leave one blank to sync the parent in order to sync children.");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("childObjectToSync"), contentChildObjectToSync);

            GUIContent contentVariablesToSync = new GUIContent("Variables to Sync", "Fine tune what variables to sync.");
            showWhatToSync = EditorGUILayout.Foldout(showWhatToSync, contentVariablesToSync);
            if (showWhatToSync)
            {
                EditorGUI.indentLevel++;
                GUIContent contentSyncPosition = new GUIContent("Sync Position", "Fine tune what variables to sync.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncPosition"), contentSyncPosition);
                GUIContent contentSyncRotation = new GUIContent("Sync Rotation", "Fine tune what variables to sync");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncRotation"), contentSyncRotation);
                GUIContent contentSyncScale = new GUIContent("Sync Scale", "Fine tune what variables to sync");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncScale"), contentSyncScale);
                GUIContent contentSyncVelocity = new GUIContent("Sync Velocity", "Fine tune what variables to sync");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncVelocity"), contentSyncVelocity);
                GUIContent contentSyncAngularVelocity = new GUIContent("Sync Angular Velocity", "Fine tune what variables to sync");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("syncAngularVelocity"), contentSyncAngularVelocity);
                EditorGUI.indentLevel--;
            }

            GUIContent contentExtrapolation = new GUIContent("Extrapolation", "Extrapolation is going into the unknown based on information we had in the past. Generally, you'll want extrapolation to help fill in missing information during lag spikes.");
            showExtrapolation = EditorGUILayout.Foldout(showExtrapolation, contentExtrapolation);
            if (showExtrapolation)
            {
                EditorGUI.indentLevel++;
                GUIContent contentExtrapolationMode = new GUIContent("Extrapolation Mode", "None: No extrapolation. Limited: Some extrapolation. Unlimited: Unlimited extrapolation.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("extrapolationMode"), contentExtrapolationMode);
                if (myTarget.extrapolationMode == SmoothSyncMirror.ExtrapolationMode.Limited)
                {
                    GUIContent contentUseExtrapolationTimeLimit = new GUIContent("Use Extrapolation Time Limit", "Whether or not you want to use extrapolationTimeLimit. You can use only the extrapolationTimeLimit and save a distance check every extrapolation frame.");
                    GUIContent contentUseExtrapolationDistanceLimit = new GUIContent("Use Extrapolation Distance Limit", "Whether or not you want to use extrapolationDistanceLimit. You can use only the extrapolationTimeLimit and save a distance check every extrapolation frame.");
                    GUIContent contentExtrapolationDistanceLimit = new GUIContent("Extrapolation Distance Limit", "How much distance into the future a non-owned object is allowed to extrapolate. In distance units.");
                    GUIContent contentExtrapolationTimeLimit = new GUIContent("Extrapolation Time Limit", "How much time into the future a non-owned object is allowed to extrapolate. In seconds.");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useExtrapolationTimeLimit"), contentUseExtrapolationTimeLimit);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("extrapolationTimeLimit"), contentExtrapolationTimeLimit);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("useExtrapolationDistanceLimit"), contentUseExtrapolationDistanceLimit);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("extrapolationDistanceLimit"), contentExtrapolationDistanceLimit);
                }
                EditorGUI.indentLevel--;
            }

            GUIContent contentThresholds = new GUIContent("Thresholds", "Use thresholds to control when to send and set the transform.");
            showThresholds = EditorGUILayout.Foldout(showThresholds, contentThresholds);
            if (showThresholds)
            {
                EditorGUI.indentLevel++;
                GUIContent contentSnapTimeThreshold = new GUIContent("Snap Time Threshold", "The estimated owner time will change instantly if the difference is larger than this amount (in seconds) when receiving an update. Generally keep at default unless you have a very low send rate and expect large variance in your latencies.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapTimeThreshold"), contentSnapTimeThreshold);
                GUIContent contentSnapPositionThreshold = new GUIContent("Snap Position Threshold", "If the position is more than snapThreshold units from the target position, it will jump to the target position immediately instead of easing. Set to 0 to not use at all. In distance units.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapPositionThreshold"), contentSnapPositionThreshold);
                GUIContent contentSnapRotationThreshold = new GUIContent("Snap Rotation Threshold", "If the rotation is more than snapThreshold units from the target rotation, it will jump to the target rotation immediately instead of easing. Set to 0 to not use at all. In degrees.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapRotationThreshold"), contentSnapRotationThreshold);
                GUIContent contentSnapScaleThreshold = new GUIContent("Snap Scale Threshold", "If the scale is more than snapThreshold units from the target scale, it will jump to the target scale immediately instead of easing. Set to 0 to not use at all. In degrees.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("snapScaleThreshold"), contentSnapScaleThreshold);

                GUIContent contentSendPositionThreshold = new GUIContent("Send Position Threshold", "A synced object's position is only sent if it is off from the last sent position by more than the threshold. In distance units.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendPositionThreshold"), contentSendPositionThreshold);
                GUIContent contentSendRotationThreshold = new GUIContent("Send Rotation Threshold", "A synced object's rotation is only sent if it is off from the last sent rotation by more than the threshold. In degrees.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendRotationThreshold"), contentSendRotationThreshold);
                GUIContent contentSendScaleThreshold = new GUIContent("Send Scale Threshold", "A synced object's scale is only sent if it is off from the last sent scale by more than the threshold. In distance units.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendScaleThreshold"), contentSendScaleThreshold);
                GUIContent contentSendVelocityThreshold = new GUIContent("Send Velocity Threshold", "A synced object's velocity is only sent if it is off from the last sent velocity by more than the threshold. In distance per second.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendVelocityThreshold"), contentSendVelocityThreshold);
                GUIContent contentSendAngularVelocityThreshold = new GUIContent("Send Angular Velocity Threshold", "A synced object's angular velocity is only sent if it is off from the last sent angular velocity by more than the threshold. In degrees per second.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("sendAngularVelocityThreshold"), contentSendAngularVelocityThreshold);

                GUIContent contentReceivedPositionThreshold = new GUIContent("Received Position Threshold", "A synced object's position is only updated if it is off from the target position by more than the threshold. Set to 0 to always update. Usually keep at 0 unless you notice problems with backtracking on stops. In distance units.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedPositionThreshold"), contentReceivedPositionThreshold);
                GUIContent contentReceivedRotationThreshold = new GUIContent("Received Rotation Threshold", "A synced object's rotation is only updated if it is off from the target rotation by more than the threshold. Set to 0 to always update. Usually keep at 0 unless you notice problems with backtracking on stops. In degrees.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("receivedRotationThreshold"), contentReceivedRotationThreshold);
                EditorGUI.indentLevel--;
            }

            GUIContent contentCompression = new GUIContent("Compression", "Convert floats sent over the network to Halfs, which use half as much bandwidth but are also half as precise. It'll start becoming noticeably inaccurate over ~500.");
            showCompressions = EditorGUILayout.Foldout(showCompressions, contentCompression);
            if (showCompressions)
            {
                EditorGUI.indentLevel++;
                GUIContent contentCompressPosition = new GUIContent("Compress Position", "Compress floats to save bandwidth.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isPositionCompressed"), contentCompressPosition);
                GUIContent contentCompressRotation = new GUIContent("Compress Rotation", "Compress floats to save bandwidth.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isRotationCompressed"), contentCompressRotation);
                GUIContent contentCompressScale = new GUIContent("Compress Scale", "Compress floats to save bandwidth.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isScaleCompressed"), contentCompressScale);
                GUIContent contentCompressVelocity = new GUIContent("Compress Velocity", "Compress floats to save bandwidth.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isVelocityCompressed"), contentCompressVelocity);
                GUIContent contentCompressAngularVelocity = new GUIContent("Compress Angular Velocity", "Compress floats to save bandwidth.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isAngularVelocityCompressed"), contentCompressAngularVelocity);
                EditorGUI.indentLevel--;
            }

            GUIContent contentMiscellaneous = new GUIContent("Miscellaneous", "Miscellaneous");
            showMiscellaneous = EditorGUILayout.Foldout(showMiscellaneous, contentMiscellaneous);
            if (showMiscellaneous)
            {
                EditorGUI.indentLevel++;
                GUIContent contentSmoothAuthorityChanges = new GUIContent("Smooth Authority Changes", "Sends an extra byte if checked. Authority changes will be smoothed out.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("isSmoothingAuthorityChanges"), contentSmoothAuthorityChanges);
                GUIContent contentUseLocalTransformOnly = new GUIContent("Use Local Transform Only", "Useful for VR applications that always have objects in local space.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("useLocalTransformOnly"), contentUseLocalTransformOnly);
                GUIContent contentUseVelocityForSyncing = new GUIContent("Use Velocity Driven Syncing", "Set velocity instead of position on non-owned objects. Can be smoother but will be less accurate. Is meant to be used for racing or flying player objects but may have other uses.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("setVelocityInsteadOfPositionOnNonOwners"), contentUseVelocityForSyncing);
                if (myTarget.setVelocityInsteadOfPositionOnNonOwners == true)
                {
                    GUIContent contentMaxPositionDifference = new GUIContent("Max Position Difference", "If the difference between where it is and where it should be hits this, then it will go to location next frame. Is on an exponential scale otherwise. Only used for Velocity Driven Syncing.");
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("maxPositionDifferenceForVelocitySyncing"), contentMaxPositionDifference);
                }
                GUIContent contentTransformDeterminedBy = new GUIContent("Transform Source", "Set to Owner to have the owner determine the Transform and rigidbody variables to send out. \nSet to Server to have the server determine the Transform and rigidbody variables to send out. \nOne might set this to Server so that CMDs (like movement) can be sent on these objects but still have the Server determine the position of the object.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("transformSource"), contentTransformDeterminedBy);
                GUIContent contentAutomaticallyResetTime = new GUIContent("Automatically Reset Time", "Enable automatic local time reset to avoid float precision issues in long running games. When enabled localTime will be reset approximately every hour to prevent it from growing too large and introducing float precision issues that can cause jittering and other syncing issues in long running games. This costs an extra byte per network update, so don't enable this if you don't need it. When enabled localTime is also reset on each Scene load.");
                EditorGUILayout.PropertyField(serializedObject.FindProperty("automaticallyResetTime"), contentAutomaticallyResetTime);
            }

            serializedObject.ApplyModifiedProperties();
            //EditorUtility.SetDirty(myTarget);
        }
    }
}