using System;
using Core;
using UnityEngine;

namespace Game
{ // Offline manual cooking still not working
    public static class HOGExtension
    {
        public static void WaitForAnimationComplete(this Animator animator, HOGMonoBehaviour monoBehaviour, Action onComplete)
        {
            var animationTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            monoBehaviour.WaitForFrame(() =>
            {
                monoBehaviour.WaitForSeconds(animationTime, delegate
                {
                    onComplete?.Invoke();
                });
            });
        }

        public static int HoursToSeconds(this int hours)
        {
            return hours * 60 * 60;
        }

        public static int HoursToMin(this int hours)
        {
            return hours * 60;
        }

        public static int MinToSeconds(this int min)
        {
            return min * 60;
        }
    }
}