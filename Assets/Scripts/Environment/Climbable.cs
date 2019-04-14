using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Climbable : MonoBehaviour
    {
        public Vector2 offset;
        public Vector2 zoneCenter;
        public Vector2 zoneSize;
        [Header("Debug")]
        public Color zoneColor = new Color(1, 0, 0, 0.5f);
        public Color lineColor = Color.red;
        private Bounds zone;

        private BoxCollider2D col;
        private Bounds box;
        public Vector2 size, top, bottom;

        #region Unity Functions
        void Start()
        {
            RecalculateBounds();
        }
        void OnDrawGizmos()
        {
            if (Application.isEditor)
            {
                RecalculateBounds();

                Gizmos.color = zoneColor;
                Gizmos.DrawCube(zone.center, zone.size);

                Gizmos.color = lineColor;
                Gizmos.DrawLine(top, bottom);
            }
        }
        #endregion

        #region Custom Functions
        private void RecalculateBounds()
        {
            col = GetComponent<BoxCollider2D>();
            box = col.bounds;
            size = box.size;

            zone = new Bounds(box.center + (Vector3)zoneCenter, 
                              box.size + (Vector3)zoneSize);
            
            Vector2 position = transform.position;

            top = position + new Vector2(offset.x, 0);
            bottom = position + new Vector2(offset.x, -size.y + offset.y);
        }

        #region Accessors
        public float GetX()
        {
            return zone.center.x;
        }
        public bool IsAtTop(Vector3 point)
        {
            if (point.y > zone.max.y)
            {
                return true;
            }
            return false;
        }
        public bool IsAtBottom(Vector3 point)
        {
            if (point.y < zone.min.y)
            {
                return true;
            }
            return false;
        }
        #endregion
        #endregion
    }
}