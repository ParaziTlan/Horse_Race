using System.Collections.Generic;
using UnityEngine;

namespace HorseRace.GamePlay
{
    [System.Serializable]
    public class Horse
    {
        public int horseIndex;
        public string horseName;
    }

    public class HorseContainer : MonoBehaviour
    {
        public static HorseContainer Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        [SerializeField] private List<Horse> _horseList;

        public (int, int) GetHorseAndJockeyIndex(int index)
        {
            int horseMaterialCount = 4;
            int jockeyMaterialCount = 3;

            if (index >= horseMaterialCount * jockeyMaterialCount)
            {
                Debug.LogError("There is not enough materials for this index: " + index);
            }

            (int, int) horseAndJockeyIndex;

            horseAndJockeyIndex.Item1 = index % horseMaterialCount;
            horseAndJockeyIndex.Item2 = index / horseMaterialCount;

            return horseAndJockeyIndex;
        }

        public string GetHorseName(int index)
        {
            return _horseList[index].horseName;
        }
    }
}
