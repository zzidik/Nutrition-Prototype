using System;
using System.Collections.Generic;
using UnityEngine;

namespace NutritionPrototype
{

    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Contains a question and answer.
        /// The answer is an int for multiple choice questions.
        /// </summary>
        public class QA
        {
            public string question;
            public int answer;
        }
        /// <summary>
        /// Extands the QA class to include a set of displayVols,
        /// which are used to display an index from a VolumeSelector volumes list.
        /// </summary>
        public class QAVol : QA
        {
            public int[] displayVols = new int[4];
        }
        /// <summary>
        /// A list of all the controls related to a player.
        /// </summary>
        public List<PlayerStation> playerStations;
        /// <summary>
        /// A list of Lanes.
        /// </summary>
        public List<Lane> lanes;

        /// <summary>
        /// Assign ID numbers to the PlayerStations.
        /// </summary>
        private void OnEnable()
        {
            int numplayerStations = playerStations.Count;
            for (int ps = 0; ps < numplayerStations; ps++)
            {
                playerStations[ps].IDNum = ps;
            }
        }

        public void LoadLevel(int lvl)
        {
            // define sets of indexs for volumes
            int[] cubeVolSet = new int[4] { 0, 1, 2, 3 };
            int[] cubeVolSet_B = new int[4] { 3, 2, 0, 1 };

            int numplayerStations = playerStations.Count;
            for (int ps = 0; ps < numplayerStations; ps++)
            {
                playerStations[ps].quiz.Clear();
                playerStations[ps].quiz = new List<QAVol>();

                // builds a quiz for each player according to level
                // (NOTE - could make players be on different levels)
                switch (lvl)
                {
                    case 0:
                        playerStations[ps].quiz.Add(CreateQAVol("1", 0, cubeVolSet));
                        playerStations[ps].quiz.Add(CreateQAVol("2", 1, cubeVolSet));
                        playerStations[ps].quiz.Add(CreateQAVol("3", 2, cubeVolSet));
                        playerStations[ps].quiz.Add(CreateQAVol("4", 3, cubeVolSet));
                        break;
                    case 1:
                        playerStations[ps].quiz.Add(CreateQAVol("1", 0, cubeVolSet_B));
                        playerStations[ps].quiz.Add(CreateQAVol("2", 1, cubeVolSet_B));
                        playerStations[ps].quiz.Add(CreateQAVol("3", 2, cubeVolSet_B));
                        playerStations[ps].quiz.Add(CreateQAVol("4", 3, cubeVolSet_B));
                        break;
                    case 2:
                        playerStations[ps].quiz.Add(CreateQAVol("1 + 1", 1, ShuffleIntArray(cubeVolSet)));
                        playerStations[ps].quiz.Add(CreateQAVol("10 - 6", 3, ShuffleIntArray(cubeVolSet)));
                        playerStations[ps].quiz.Add(CreateQAVol("27 / 9", 2, ShuffleIntArray(cubeVolSet)));
                        playerStations[ps].quiz.Add(CreateQAVol("1 x 1", 0, ShuffleIntArray(cubeVolSet)));
                        break;
                }

                // set each player to the first question and displayy the volumes
                playerStations[ps].question = 0;
                playerStations[ps].inGameDisplay.question.text = playerStations[ps].quiz[0].question;
                playerStations[ps].SetVolumeSelectors(playerStations[ps].quiz[0].displayVols);
                playerStations[ps].ActivateVolumeSelectors(true);
            }

            // reset all Lanes
            int numtracks = lanes.Count;
            for (int l = 0; l < numtracks; l++)
            {
                lanes[l].Reset();
            }
        }

        /// <summary>
        /// A way of randomizing the order of an array of integers.
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        private int[] ShuffleIntArray(int[] nums)
        {
            for (int i = 0; i < nums.Length; i++)
            {
                int tmp = nums[i];
                int r = UnityEngine.Random.Range(i, nums.Length);
                nums[i] = nums[r];
                nums[r] = tmp;
            }

            return nums;
        }

        /// <summary>
        /// Creates and returns a QAVol
        /// </summary>
        /// <param name="q">The question</param>
        /// <param name="a">The answer</param>
        /// <param name="dvs">The display volumes</param>
        /// <returns></returns>
        private QAVol CreateQAVol(string q, int a, int[] dvs)
        {
            QAVol qavol = new QAVol();
            qavol.question = q;
            qavol.answer = a;
            qavol.displayVols = dvs;
            return qavol;
        }
    }
}
