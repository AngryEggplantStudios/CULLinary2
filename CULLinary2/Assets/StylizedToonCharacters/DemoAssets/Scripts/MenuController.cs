using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

namespace Nicoplv.Characters.Demo
{
	public class MenuController : MonoBehaviour
    {
        #region Enums

        private enum States
        {
            Default,
            Swimming,
            Climbing
        }

        #endregion

        #region Variables

        [SerializeField]
        private GameObject characterGameObject;
        private Renderer characterRenderer;
        private Animator animator;
        private FaceAnimator faceAnimator;

        [SerializeField]
        private GameObject characterGameObjectMobile;
        private Renderer characterRendererMobile;
        private Animator animatorMobile;
        private FaceAnimator faceAnimatorMobile;

        [Space]
        [SerializeField]
        private Toggle dynamicBonesTogle;
        //[SerializeField]
        //private GameObject[] dynamicBonesGameObjects;
        private List<GameObject> dynamicBonesGameObjects = new List<GameObject>();

        [Space]
        [SerializeField]
        private Dropdown dropdownState;
        private int b_stateValue = 0;
        [SerializeField]
        private Dropdown dropdownMouth;
        [SerializeField]
        private Dropdown dropdownEyeLeft;
        [SerializeField]
        private Dropdown dropdownEyeRight;

        [Space]
        [SerializeField]
        private GameObject inputSpeed;
        [SerializeField]
        private GameObject inputJump;
        [SerializeField]
        private GameObject inputPickup;
        [SerializeField]
        private GameObject inputAttack;
        [SerializeField]
        private GameObject inputClimbX;
        [SerializeField]
        private GameObject inputClimbY;

        #endregion

        #region Methods

        private void Start()
        {
            // Get components
            animator = characterGameObject.GetComponent<Animator>();
            faceAnimator = characterGameObject.GetComponent<FaceAnimator>();
            characterRenderer = characterGameObject.GetComponentInChildren<Renderer>();
            animatorMobile = characterGameObjectMobile.GetComponent<Animator>();
            faceAnimatorMobile = characterGameObjectMobile.GetComponent<FaceAnimator>();
            characterRendererMobile = characterGameObjectMobile.GetComponentInChildren<Renderer>();

            // Populate dropdowns
            dropdownState.AddOptions(Enum.GetNames(typeof(States)).ToList());
            dropdownMouth.AddOptions(Enum.GetNames(typeof(FaceAnimator.MouthExpressions)).ToList());
            dropdownEyeLeft.AddOptions(Enum.GetNames(typeof(FaceAnimator.EyeExpressions)).ToList());
            dropdownEyeRight.AddOptions(Enum.GetNames(typeof(FaceAnimator.EyeExpressions)).ToList());

            // Init menu
            inputSpeed.SetActive(true);
            inputJump.SetActive(true);
            inputPickup.SetActive(true);
            inputClimbX.SetActive(false);
            inputClimbY.SetActive(false);

            // Search dynamic bones and hide toggle if no one is set
            Transform b_dynamicBonesTransform = characterGameObject.transform.Find("DynamicBones");
            if (b_dynamicBonesTransform != null)
                dynamicBonesGameObjects.Add(b_dynamicBonesTransform.gameObject);
            b_dynamicBonesTransform = characterGameObjectMobile.transform.Find("DynamicBones");
            if (b_dynamicBonesTransform != null)
                dynamicBonesGameObjects.Add(b_dynamicBonesTransform.gameObject);

            if (dynamicBonesGameObjects.Count == 0)
                dynamicBonesTogle.gameObject.SetActive(false);

            // Init mesh
            ToggleMobile(false);
            ToggleDynamicBones(false);
        }

        public void OnStateChanged(int _value)
        {
            if(b_stateValue != _value)
            {
                b_stateValue = _value;

                animator.SetInteger("State", _value);
                animator.SetTrigger("StateChange");

                animatorMobile.SetInteger("State", _value);
                animatorMobile.SetTrigger("StateChange");

                switch(_value)
                {
                    case 0:
                        inputSpeed.SetActive(true);
                        inputJump.SetActive(true);
                        inputPickup.SetActive(true);
                        inputAttack.SetActive(true);
                        inputClimbX.SetActive(false);
                        inputClimbY.SetActive(false);
                        break;
                    case 1:
                        inputSpeed.SetActive(true);
                        inputJump.SetActive(false);
                        inputPickup.SetActive(false);
                        inputAttack.SetActive(false);
                        inputClimbX.SetActive(false);
                        inputClimbY.SetActive(false);
                        break;
                    case 2:
                        inputSpeed.SetActive(false);
                        inputJump.SetActive(false);
                        inputPickup.SetActive(false);
                        inputAttack.SetActive(false);
                        inputClimbX.SetActive(true);
                        inputClimbY.SetActive(true);
                        break;
                }
            }
        }

        public void OnMouthChanged(int _value)
        {
            faceAnimator.SetMouth((FaceAnimator.MouthExpressions)_value);
            faceAnimatorMobile.SetMouth((FaceAnimator.MouthExpressions)_value);
        }

        public void OnEyeLeftChanged(int _value)
        {
            faceAnimator.SetLeftEye((FaceAnimator.EyeExpressions)_value);
            faceAnimatorMobile.SetLeftEye((FaceAnimator.EyeExpressions)_value);
        }

        public void OnEyeRightChanged(int _value)
        {
            faceAnimator.SetRightEye((FaceAnimator.EyeExpressions)_value);
            faceAnimatorMobile.SetRightEye((FaceAnimator.EyeExpressions)_value);
        }

        public void SpeedChange(float _value)
        {
            if(animator.gameObject.activeSelf)
            {
                animator.SetFloat("Speed", _value);
            } else {
                animatorMobile.SetFloat("Speed", _value);
            }
        }

        public void Jump()
        {
            if(animator.gameObject.activeSelf)
            {
                animator.SetTrigger("Jump");
            } else {
                animatorMobile.SetTrigger("Jump");
            }
        }

        public void Pickup()
        {
            if(animator.gameObject.activeSelf)
            {
                animator.SetTrigger("Pickup");
            } else {
                animatorMobile.SetTrigger("Pickup");
            }
        }
        
        public void Attack()
        {
            if(animator.gameObject.activeSelf)
            {
                animator.SetTrigger("Attack");
            } else {
                animatorMobile.SetTrigger("Attack");
            }
        }

        public void ClimbXChange(float _value)
        {
            animator.SetFloat("ClimbingMoveX", _value);
            animatorMobile.SetFloat("ClimbingMoveX", _value);
        }

        public void ClimbYChange(float _value)
        {
            animator.SetFloat("ClimbingMoveY", _value);
            animatorMobile.SetFloat("ClimbingMoveY", _value);
        }

        public void ToggleMobile(bool _value)
        {
            characterRenderer.enabled = !_value;
            characterRendererMobile.enabled = _value;
        }

        public void ToggleDynamicBones(bool _value)
        {
            foreach (GameObject iGameObject in dynamicBonesGameObjects)
                iGameObject.SetActive(_value);
        }

        #endregion
    }
}