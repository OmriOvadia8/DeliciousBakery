using System;
using Core;
using UnityEngine;

namespace Test
{
    public class HOGTester : HOGMonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
            }

            var user = new UserID
            {
                name = "Shahar",
                age = 29,
                height = 171
            };

            user.age++;
        }


        //private StudentA _studentA = new StudentA();

        //    private void ShaharTeaching()
        //    {
        //        _studentA.DoHomeWork(ShaharHappy);
        //    }

        //    private void ShaharHappy(string homeworkType)
        //    {
        //        Debug.Log("Smile");
        //    }
        //}


        public class StudentA : MonoBehaviour
        {
            public Action<string> onComplete;

            public void DoHomeWork(Action<string> onCompleteHW)
            {
                onComplete = onCompleteHW;
                Debug.Log("HomeWork Start");
            }

            private void FinishedHomeWork()
            {
                onComplete.Invoke("dsdads");
            }

            private void Update()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    FinishedHomeWork();
                }
            }
        }
        public struct UserID
        {
            public string name;
            public int age;
            public float height;
        }
    }
}