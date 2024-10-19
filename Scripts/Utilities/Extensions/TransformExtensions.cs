﻿using UnityEngine;

namespace FPTemplate.Utilities.Extensions
{
	public static class TransformExtensions
    {
        public static Matrix4x4 GetRelativeTransformationMatrix(this Transform sourceTransform, Transform targetTransform)
        {
            // Calculate the inverse matrix of the source transform
            Matrix4x4 sourceInverse = Matrix4x4.TRS(sourceTransform.position, sourceTransform.rotation, sourceTransform.lossyScale).inverse;

            // Calculate the matrix of the target transform
            Matrix4x4 targetMatrix = Matrix4x4.TRS(targetTransform.position, targetTransform.rotation, targetTransform.lossyScale);

            // Compute the relative transformation matrix
            return targetMatrix * sourceInverse;
        }

        public static void SetLayerRecursive(this Transform t, int layer)
        {
            t.gameObject.layer = layer;
            foreach (Transform child in t)
            {
                child.SetLayerRecursive(layer);
            }
        }

        public static void RotateTowardsPosition(this Transform t, Vector3 worldPos, Vector3 upVector, float maxAngle, Quaternion extraRotation)
        {
            t.rotation = RotateTowardsPosition(t.rotation, t.position, upVector, worldPos, maxAngle, extraRotation);
        }

        public static Quaternion RotateTowardsPosition(this Quaternion rootRotation, Vector3 rootPosition, Vector3 upVector, Vector3 targetPosition, float maxAngle, Quaternion extraRotation)
        {
            var diff = targetPosition - rootPosition;
            var rot = Quaternion.LookRotation(diff.normalized, upVector);
            var spinAngle = rot.eulerAngles.y;
            var tiltAngle = rot.eulerAngles.x;
            return extraRotation * Quaternion.RotateTowards(Quaternion.Inverse(extraRotation) * rootRotation, Quaternion.Euler(tiltAngle, spinAngle, 0), maxAngle);
        }

        public static void RotateTowardsPosition(this Rigidbody rb, Vector3 upVector, Vector3 worldPos, float maxAngle, Quaternion extraRotation)
        {
            rb.rotation = RotateTowardsPosition(rb.rotation, rb.position, upVector, worldPos, maxAngle, extraRotation);
        }

        public static int GetHierarchyDepth(this Transform transform)
        {
            int depth = 0;
            MarchUpHierarchy(transform, ref depth);
            return depth;
        }

        private static void MarchUpHierarchy(Transform t, ref int count)
        {
            if (t.parent == null)
            {
                return;
            }
            count++;
            MarchUpHierarchy(t.parent, ref count);
        }

        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            if (aName == aParent.name)
            {
                return aParent;
            }
            var result = aParent.Find(aName);
            if (result != null)
                return result;
            foreach (Transform child in aParent)
            {
                result = child.FindDeepChild(aName);
                if (result != null)
                    return result;
            }
            return null;
        }

        public static void ApplyTRSMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.rotation = matrix.GetRotation();
            transform.position = matrix.GetPosition();
        }

        public static void ApplyLocalTRSMatrix(this Transform transform, Matrix4x4 matrix)
        {
            transform.localScale = matrix.GetScale();
            transform.localRotation = matrix.GetRotation();
            transform.localPosition = matrix.GetPosition();
        }

        public static Matrix4x4 GetGlobalTRS(this Transform transform)
        {
            return Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
        }

        public static Matrix4x4 GetLocalTRS(this Transform transform)
        {
            return Matrix4x4.TRS(transform.localPosition, transform.localRotation, transform.localScale);
        }

        public static Quaternion GetRotation(this Matrix4x4 m)
        {
            return Quaternion.LookRotation(m.GetColumn(2), m.GetColumn(1));
        }

        public static Vector3 GetPosition(this Matrix4x4 matrix)
        {
            var x = matrix.m03;
            var y = matrix.m13;
            var z = matrix.m23;

            return new Vector3(x, y, z);
        }

        public static Vector3 GetScale(this Matrix4x4 m)
        {
            return new Vector3(m.GetColumn(0).magnitude,
                                m.GetColumn(1).magnitude,
                                m.GetColumn(2).magnitude);
        }

    }
}