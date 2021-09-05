using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

namespace AutoDynamicBones
{
    //[CreateAssetMenu(fileName = "AutoDynamicBoneData", menuName = "AutoDynamicBoneData", order = 1)]
    public class AutoDynamicBoneData : ScriptableObject
    {
        #region Variables

        [SerializeField]
        public List<DynamicBoneData> dynamicBoneDatas = new List<DynamicBoneData>();

        [SerializeField]
        public List<DynamicBoneColliderData> dynamicBoneColliderDatas = new List<DynamicBoneColliderData>();

        #endregion

        #region Dynamic Bone Data

        [Serializable]
        public class DynamicBoneData
        {
            public string m_Root = "";

            [Range(0, 1)]
            public float m_Damping = 0.1f;
            public AnimationCurve m_DampingDistrib = null;
            [Range(0, 1)]
            public float m_Elasticity = 0.1f;
            public AnimationCurve m_ElasticityDistrib = null;
            [Range(0, 1)]
            public float m_Stiffness = 0.1f;
            public AnimationCurve m_StiffnessDistrib = null;
            [Range(0, 1)]
            public float m_Inert = 0;
            public AnimationCurve m_InertDistrib = null;
            public float m_Radius = 0;
            public AnimationCurve m_RadiusDistrib = null;

            public float m_EndLength = 0;
            public Vector3 m_EndOffset = Vector3.zero;
            public Vector3 m_Gravity = Vector3.zero;
            public Vector3 m_Force = Vector3.zero;
            public List<int> m_Colliders = null;
        }

        [Serializable]
        public class DynamicBoneColliderData
        {
            public string root = "";

            public Vector3 m_Center = Vector3.zero;
            public float m_Radius = 0.5f;
            public float m_Height = 0;

            public enum Direction
            {
                X, Y, Z
            }

            public enum Bound
            {
                Outside,
                Inside
            }

            public Direction m_Direction = Direction.X;
            
            public Bound m_Bound = Bound.Outside;
        }

        #endregion
    }
}