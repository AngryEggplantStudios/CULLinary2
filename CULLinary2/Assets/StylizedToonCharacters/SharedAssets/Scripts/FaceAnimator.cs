using UnityEngine;

namespace Nicoplv.Characters
{
    public class FaceAnimator : MonoBehaviour
    {
        #region Enums

        public enum EyeExpressions
        {
            Default,
            Sad,
            Determined,
            Angry,
            Crying,
            Excited,
            Tired,
            Surprised,
            Happy,
            Exhausted,
            Laughing,
            Relieved,
            Resolute,
            Unhappy,
            Annoyed,
            Furious
        }

        public enum MouthExpressions
        {
            Default,
            Unhappy,
            Happy,
            Sad,
            Neutral,
            Worried,
            Dazed,
            Angry,
            SuspiciousLeft,
            SuspiciousRight,
            Excited,
            Scared,
            Surprised,
            Confused,
            Embarassed,
            Crying
        }

        #endregion

        #region Variables

        [SerializeField]
        private new Renderer renderer;

        [SerializeField]
        private int mouthMaterialIndex = 0;

        [SerializeField]
        private int leftEyeMaterialIndex = 0;

        [SerializeField]
        private int rightEyeMaterialIndex = 0;

        #endregion

        #region Methods

        private void Awake()
        {
            renderer.materials[mouthMaterialIndex] = Instantiate(renderer.materials[mouthMaterialIndex]);
            renderer.materials[mouthMaterialIndex].name = "Mouth";
            renderer.materials[leftEyeMaterialIndex] = Instantiate(renderer.materials[leftEyeMaterialIndex]);
            renderer.materials[leftEyeMaterialIndex].name = "EyeLeft";
            renderer.materials[rightEyeMaterialIndex] = Instantiate(renderer.materials[rightEyeMaterialIndex]);
            renderer.materials[rightEyeMaterialIndex].name = "EyeRight";
        }

        /// <summary>
        /// Set the mouth expression
        /// </summary>
        /// <param name="_mouthExpression">Mouth expression</param>
        public void SetMouth(MouthExpressions _mouthExpression)
        {
            renderer.materials[mouthMaterialIndex].SetInt("_Frame", (int)_mouthExpression);
        }

        /// <summary>
        /// Set the left eye expression
        /// </summary>
        /// <param name="_eyeExpression">Eye expression</param>
        public void SetLeftEye(EyeExpressions _eyeExpression)
        {
            renderer.materials[leftEyeMaterialIndex].SetInt("_Frame", (int)_eyeExpression);
        }

        /// <summary>
        /// Set the right eye expression
        /// </summary>
        /// <param name="_eyeExpression">Eye expression</param>
        public void SetRightEye(EyeExpressions _eyeExpression)
        {
            renderer.materials[rightEyeMaterialIndex].SetInt("_Frame", (int)_eyeExpression);
        }

        #endregion
    }
}