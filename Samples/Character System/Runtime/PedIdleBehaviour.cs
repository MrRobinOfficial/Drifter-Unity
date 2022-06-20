using UnityEngine;

namespace Drifter.Samples.GTASystem
{
    public class PedIdleBehaviour : StateMachineBehaviour
    {
        private static readonly int IDLE_BLEND_HASH = Animator.StringToHash("IdleBlend");

        [SerializeField] float m_WaitMinThreshold = 3f;
        [SerializeField] float m_WaitMaxThreshold = 5f;

        [SerializeField] int m_AnimationCount;

        private bool isBored;
        private float timer;
        private int animationIndex;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) => ResetIdle();

        private float waitThreshold;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            const float MIN_TIME = 0.02f;
            const float MAX_TIME = 0.98f;
            const float DAMP_TIME = 0.2f;

            if (!isBored)
            {
                timer += Time.deltaTime;

                if (timer > waitThreshold && stateInfo.normalizedTime % 1 < MIN_TIME)
                {
                    timer = waitThreshold;
                    isBored = true;
                    animationIndex = Random.Range(0, m_AnimationCount + 1);
                    animationIndex = animationIndex * 2 - 1;
                    animator.SetFloat(IDLE_BLEND_HASH, animationIndex - 1);
                }
            }
            else if (stateInfo.normalizedTime % 1 > MAX_TIME)
                ResetIdle();

            animator.SetFloat(IDLE_BLEND_HASH, animationIndex, DAMP_TIME, Time.deltaTime);
        }

        private void ResetIdle()
        {
            if (isBored)
                animationIndex--;

            isBored = false;
            timer = 0f;
            waitThreshold = Random.Range(m_WaitMinThreshold, m_WaitMaxThreshold);
        }
    }
}