using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nicoplv.Characters
{
    public class IdleRandomizerStateMachineBehaviour : StateMachineBehaviour
    {
        #region Variables

        [SerializeField, Range(0f, 1f)]
        private float breakChance = 0.25f;
        private int randomizeValue = -1;
        [SerializeField]
        private float idleMovementFadeDuration = 2f;

        #endregion

        #region Methods

        //public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    animator.SetFloat("IdleMovement", (Mathf.Sin(Time.time * 12.9898f) * 43758.5453f) % 1);
        //}

        // OnStateMachineEnter is called when entering a statemachine via its Entry Node
        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            switch (randomizeValue)
            {
                case -1:

                    // Just enter in idle behaviour randomize between 0 and 1
                    randomizeValue = Random.Range(0, 2);

                    break;

                case 0:
                case 1:

                    // Already in idle behaviour randomize a break
                    if (Random.value < breakChance)
                    {
                        // Randomize bewteen 2 and 3
                        randomizeValue = 2 + Random.Range(0, 2);
                    }

                    break;

                case 2:
                case 3:

                    // Already in idle behaviour but in break randomize between 0 and 1
                    randomizeValue = Random.Range(0, 2);

                    break;
            }

            // Apply current idle animation
            animator.SetInteger("IdleRandomizer", randomizeValue);

            // Apply noise movement in some case
            if (randomizeValue < 2)
            {
                CoroutineHelper.Start(FadeIdleMovement(animator, animator.GetFloat("IdleMovement"), Random.value, idleMovementFadeDuration));
            }
        }

        private IEnumerator FadeIdleMovement(Animator _animator, float _from, float _to, float _duration)
        {
            float duration = 0;
            float percent = 0;
            while (duration < _duration)
            {
                duration += Time.deltaTime;
                percent = Mathf.Min(duration / _duration, 1f);
                _animator.SetFloat("IdleMovement", Mathf.Lerp(_from, _to, percent));
                yield return null;
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            // If exiting state with speed different of 0, reset the randomizer
            if (animator.GetFloat("Speed") > 0.01)
            {
                randomizeValue = -1;
                animator.SetInteger("IdleRandomizer", randomizeValue);
            }
        }

        #endregion
    }

}