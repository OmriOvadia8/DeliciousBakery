using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DB_Core
{ 
    public static class DBExtension
    {
        public static void WaitForAnimationComplete(this Animator animator, DBMonoBehaviour monoBehaviour, Action onComplete)
        {
            monoBehaviour.WaitForFrame(() =>
            {
                //var animationTime = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;

                //monoBehaviour.WaitForSeconds(animationTime, delegate
                //{
                //    onComplete?.Invoke();
                //});

                var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
                if (clipInfo.Length > 0)
                {
                    var animationTime = clipInfo[0].clip.length;
                    monoBehaviour.WaitForSeconds(animationTime, delegate
                    {
                        onComplete?.Invoke();
                    });
                }
                else
                {
                    onComplete?.Invoke();
                }

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

        public static string GetRandomString(this string[] stringArray)
        {
            return stringArray[Random.Range(0, stringArray.Length)];
        }

        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            //return string.Format("{0:D2}:{1:D2}", timeSpan.Minutes, timeSpan.Seconds);
            string formatString = "";
            if (timeSpan.TotalHours >= 1)
            {
                formatString = @"hh\:mm\:ss";
            }
            else
            {
                formatString = @"mm\:ss";
            }
            return timeSpan.ToString(formatString);
        }

        public static void WatchAd()
        {
            DBManager.Instance.AdsManager.ShowAd(null);
        }

        public static string GetFormattedTimeSpan(int seconds)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(seconds);
            if (timeSpan.TotalHours >= 1)
            {
                return string.Format("{0}:{1:mm}:{1:ss}", (int)timeSpan.TotalHours, timeSpan);
            }
            else
            {
                return timeSpan.ToString("mm':'ss");
            }
        }

    }
}