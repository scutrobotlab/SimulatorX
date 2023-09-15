// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using UnityEngine;
using static UnityEngine.Mathf;
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.Reactor
{
    public static class ReactorUtils
    {
        public static MoveDirection Reverse(this MoveDirection target)
        {
            switch (target)
            {
                case MoveDirection.Left: return MoveDirection.Right;
                case MoveDirection.Top: return MoveDirection.Bottom;
                case MoveDirection.Right: return MoveDirection.Left;
                case MoveDirection.Bottom: return MoveDirection.Top;
                case MoveDirection.TopLeft: return MoveDirection.BottomRight;
                case MoveDirection.TopCenter: return MoveDirection.BottomCenter;
                case MoveDirection.TopRight: return MoveDirection.BottomLeft;
                case MoveDirection.MiddleLeft: return MoveDirection.MiddleRight;
                case MoveDirection.MiddleCenter: return MoveDirection.MiddleCenter;
                case MoveDirection.MiddleRight: return MoveDirection.MiddleLeft;
                case MoveDirection.BottomLeft: return MoveDirection.TopRight;
                case MoveDirection.BottomCenter: return MoveDirection.TopCenter;
                case MoveDirection.BottomRight: return MoveDirection.TopLeft;
                case MoveDirection.CustomPosition: return MoveDirection.CustomPosition;
                default: throw new ArgumentOutOfRangeException(nameof(target), target, null);
            }
        }

        /// <summary> Get the 'to' (end) position por a move out (hide) animation </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="moveToDirection"> Direction to where the animation goes to </param>
        /// <param name="fromPosition"> From (start) position for the move out (hide) animation </param>
        public static Vector3 GetMoveOutPosition(RectTransform target, MoveDirection moveToDirection, Vector3 fromPosition) =>
            GetTargetPosition(target, moveToDirection, fromPosition, target.localScale, target.localEulerAngles);

        /// <summary> Get the 'to' (end) position por a move out (hide) animation </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="moveToDirection"> Direction to where the animation goes to </param>
        /// <param name="fromPosition"> From (start) position for the move out (hide) animation </param>
        /// <param name="toLocalScale"> If there is a to local scale (whe UIAnimation modifies the target's scale) the 'to' scale value needs to be passed </param>
        /// <param name="toLocalEulerAngles"> If there is a 'to' local rotation (when UIAnimation modifies the target's rotation) the 'to' rotation value needs to be passed </param>
        public static Vector3 GetMoveOutPosition(RectTransform target, MoveDirection moveToDirection, Vector3 fromPosition, Vector3 toLocalScale, Vector3 toLocalEulerAngles) =>
            GetTargetPosition(target, moveToDirection, fromPosition, toLocalScale, toLocalEulerAngles);

        /// <summary> Get the 'from' (start) position for a move in (show) animation </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="moveFromDirection"> Direction from where the animation comes from </param>
        /// <param name="toValue"> To (end) position for a move in (show) animation </param>
        public static Vector3 GetMoveInPosition(RectTransform target, MoveDirection moveFromDirection, Vector3 toValue) =>
            GetTargetPosition(target, moveFromDirection, toValue, target.localScale, target.localEulerAngles);

        /// <summary> Get the 'from' (start) position for a move in (show) animation </summary>
        /// <param name="target"> Target RectTransform </param>
        /// <param name="moveFromDirection"> Direction from where the animation comes from </param>
        /// <param name="toValue"> To (end) position for a move in (show) animation </param>
        /// <param name="fromLocalScale"> If there is a 'from' local scale (when UIAnimation modifies the target's scale) the 'from' scale value needs to be passed </param>
        /// <param name="fromLocalEulerAngles"> If there is a 'from' local rotation (when UIAnimation modifies the target's rotation) the 'from' rotation value needs to be passed </param>
        public static Vector3 GetMoveInPosition(RectTransform target, MoveDirection moveFromDirection, Vector3 toValue, Vector3 fromLocalScale, Vector3 fromLocalEulerAngles) =>
            GetTargetPosition(target, moveFromDirection, toValue, fromLocalScale, fromLocalEulerAngles);

        public static Vector3 GetTargetPosition(RectTransform target, MoveDirection moveDirection, Vector3 startPosition, Vector3 targetLocalScale, Vector3 targetLocalEulerAngles)
        {
            if (target == null) return Vector3.zero; //null target
            // Canvas canvas = target.GetComponent<Canvas>();                          //get canvas
            // if (canvas != null && canvas == canvas.rootCanvas) return Vector3.zero; //found canvas, but it's root canvas (cannot calculate)

            RectTransform parent = target.parent.GetComponent<RectTransform>(); //get parent RectTransform
            parent.ForceUpdateRectTransforms();

            Rect parentRect = parent.rect;
            Rect targetRect = target.rect;
            Vector2 targetPivot = target.pivot;

            //calculate xOffset
            float xOffset = 0;

            switch (moveDirection)
            {
                case MoveDirection.Left:
                case MoveDirection.TopLeft:
                case MoveDirection.MiddleLeft:
                case MoveDirection.BottomLeft:
                    xOffset =
                        targetRect.width * targetLocalScale.x * (1f - targetPivot.x)
                        + parentRect.width * (1f - targetPivot.x) * target.anchorMin.x
                        + parentRect.width * targetPivot.x * target.anchorMax.x;
                    break;
                case MoveDirection.Right:
                case MoveDirection.TopRight:
                case MoveDirection.MiddleRight:
                case MoveDirection.BottomRight:
                    xOffset =
                        parentRect.width
                        + targetRect.width * targetLocalScale.x * targetPivot.x
                        - parentRect.width * (1f - targetPivot.x) * target.anchorMin.x
                        - parentRect.width * targetPivot.x * target.anchorMax.x;
                    break;
            }


            //calculate yOffset
            float yOffset = 0;

            switch (moveDirection)
            {
                case MoveDirection.Top:
                case MoveDirection.TopLeft:
                case MoveDirection.TopCenter:
                case MoveDirection.TopRight:
                    yOffset =
                        parentRect.height
                        + targetRect.height * targetLocalScale.y * targetPivot.y
                        - parentRect.height * (1f - targetPivot.y) * target.anchorMin.y
                        - parentRect.height * targetPivot.y * target.anchorMax.y;
                    break;
                case MoveDirection.Bottom:
                case MoveDirection.BottomLeft:
                case MoveDirection.BottomCenter:
                case MoveDirection.BottomRight:
                    yOffset =
                        targetRect.height * targetLocalScale.y * (1f - targetPivot.y)
                        + parentRect.height * (1f - targetPivot.y) * target.anchorMin.y
                        + parentRect.height * targetPivot.y * target.anchorMax.y;
                    break;
            }


            //return calculated position
            float x = startPosition.x;
            float y = startPosition.y;
            float z = startPosition.z;

            Vector3 position;
            float xDirection = 0;
            float yDirection = 0;

            switch (moveDirection)
            {
                case MoveDirection.Left:
                    position = new Vector3(-xOffset, y, z);
                    xDirection = -1;
                    yDirection = 0;
                    break;
                case MoveDirection.Right:
                    position = new Vector3(xOffset, y, z);
                    xDirection = 1;
                    yDirection = 0;
                    break;
                case MoveDirection.Top:
                    position = new Vector3(x, yOffset, z);
                    xDirection = 0;
                    yDirection = 1;
                    break;
                case MoveDirection.Bottom:
                    position = new Vector3(x, -yOffset, z);
                    xDirection = 0;
                    yDirection = -1;
                    break;
                case MoveDirection.TopLeft:
                    position = new Vector3(-xOffset, yOffset, z);
                    xDirection = -1;
                    yDirection = 1;
                    break;
                case MoveDirection.TopCenter:
                    position = new Vector3(0, yOffset, z);
                    xDirection = 0;
                    yDirection = 1;
                    break;
                case MoveDirection.TopRight:
                    position = new Vector3(xOffset, yOffset, z);
                    xDirection = 1;
                    yDirection = 1;
                    break;
                case MoveDirection.MiddleLeft:
                    position = new Vector3(-xOffset, 0, z);
                    xDirection = -1;
                    yDirection = 0;
                    break;
                case MoveDirection.MiddleCenter:
                    position = new Vector3(0, 0, z);
                    xDirection = 0;
                    yDirection = 0;
                    break;
                case MoveDirection.MiddleRight:
                    position = new Vector3(xOffset, 0, z);
                    xDirection = 1;
                    yDirection = 0;
                    break;
                case MoveDirection.BottomLeft:
                    position = new Vector3(-xOffset, -yOffset, z);
                    xDirection = -1;
                    yDirection = -1;
                    break;
                case MoveDirection.BottomCenter:
                    position = new Vector3(0, -yOffset, z);
                    xDirection = 0;
                    yDirection = -1;
                    break;
                case MoveDirection.BottomRight:
                    position = new Vector3(xOffset, -yOffset, z);
                    xDirection = 1;
                    yDirection = -1;
                    break;
                // ReSharper disable once RedundantCaseLabel
                case MoveDirection.CustomPosition:
                default:
                    position = startPosition;
                    break;
            }

            if (Approximately(0, targetLocalEulerAngles.z))
                return position;

            //Rotation offset calculation
            //https://en.wikipedia.org/wiki/Rotation_(mathematics)#Two_dimensions
            //https://iiif.io/api/annex/notes/rotation/
            float angle = Abs(targetLocalEulerAngles.z % 180);
            float theta = angle * Deg2Rad;

            float width = targetRect.width * targetLocalScale.x;
            float height = targetRect.height * targetLocalScale.y;

            float newWidth;
            float newHeight;

            if (Approximately(angle, 0) || Approximately(angle, 90))
            {
                newWidth = width;
                newHeight = height;
            }
            else if (angle < 90)
            {
                newWidth = width * Cos(theta) + height * Sin(theta);
                newHeight = width * Sin(theta) + height * Cos(theta);
            }
            else
            {
                angle -= 90;
                theta = angle * Deg2Rad;
                newWidth = height * Cos(theta) + width * Sin(theta);
                newHeight = height * Sin(theta) + width * Cos(theta);
            }

            float offsetX = (newWidth - width) / 2f;
            float offsetY = (newHeight - height) / 2f;

            position = new Vector3
            (
                position.x + offsetX * xDirection,
                position.y + offsetY * yDirection,
                position.z
            );
            return position;

            //ToDo: re-check pivot options for rotation - it only works in limited cases (pivot should be x: 0.5f y: 0.5f to work as expected) 
        }
    }
}
