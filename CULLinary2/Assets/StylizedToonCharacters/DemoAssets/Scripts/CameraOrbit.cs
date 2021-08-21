using UnityEngine;
using UnityEngine.EventSystems;

namespace Nicoplv.Characters.Demo
{
    [ExecuteInEditMode]
	public class CameraOrbit : MonoBehaviour
	{
        #region Variables

        [SerializeField]
        private Vector3 targetOffset;

        [SerializeField]
        private float rotationSpeedMouse = 5;

        [SerializeField]
        private float smoothness = 0.5f;

        private Vector3 cameraOffset;
        private bool startMove = false;

        #endregion

        #region Methods

        private void Start()
        {
            cameraOffset = transform.position - targetOffset;
            transform.LookAt(targetOffset);
        }

        private void LateUpdate()
        {
            if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && !EventSystem.current.IsPointerOverGameObject())
                startMove = true;

            if ((Input.GetMouseButton(0) || Input.GetMouseButton(1)) && startMove)
            {
                Quaternion camAngle = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * rotationSpeedMouse, Vector3.up);
                Vector3 transcientPosition = targetOffset + cameraOffset;
                cameraOffset = camAngle * cameraOffset;
                transform.position = Vector3.Slerp(transform.position, transcientPosition, smoothness);
            }
            else
                startMove = false;

            transform.LookAt(targetOffset);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            LateUpdate();
        }
#endif

#endregion
    }
}