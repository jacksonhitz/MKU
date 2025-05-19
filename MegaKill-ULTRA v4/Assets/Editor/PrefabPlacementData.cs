using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorTools
{
    public class PrefabPlacementData : ScriptableObject
    {
        [Serializable]
        public class ItemStack { }

        private static PrefabPlacementData instance;

        [Serializable]
        public class PrefabEntry
        {
            [HideInInspector]
            public GameObject prefab;

            [HideInInspector]
            public int category;
            public PlacementData placementData;

            public PrefabEntry()
            {
                placementData = new PlacementData();
            }

            public PrefabEntry(PrefabEntry other)
            {
                prefab = other.prefab;
                category = other.category;
                placementData = new PlacementData(other.placementData);
            }
        }

        [Serializable]
        public class ContainerEntry : PrefabEntry
        {
            public ContainerData containerData;

            public ContainerEntry()
                : base()
            {
                containerData = new ContainerData();
            }

            public ContainerEntry(ContainerEntry other)
                : base(other)
            {
                containerData = new ContainerData(other.containerData);
            }
        }

        [Serializable]
        public class ContainerData
        {
            [Serializable]
            public struct Content
            {
                public ItemStack item;
                public float chance;
            }

            public Content[] possibleItems;

            public enum Mode
            {
                PerItemChance,
                ChooseOne,
            }

            public Mode mode;
            public int minStackSize;
            public int maxStackSize;

            public ContainerData() { }

            public ContainerData(ContainerData other)
            {
                possibleItems = other.possibleItems.Clone() as Content[];
                minStackSize = other.minStackSize;
                maxStackSize = other.maxStackSize;
            }
        }

        [Serializable]
        public class PlacementData
        {
            [Tooltip("Euler angle randomness along X, Y, Z axes")]
            public Vector3 rotationRandomness;

            [Tooltip("Euler angle rotation offset along X, Y, Z axes")]
            public Vector3 rotationOffset;

            [Tooltip("Min")]
            public float scaleRandomnessMin = 1f;

            [Tooltip("Max")]
            public float scaleRandomnessMax = 1f;

            [Range(0f, 1f)]
            public float normalAligned = 0f;
            public bool randomFlipX;
            public bool randomFlipY;
            public bool randomFlipZ;

            public PlacementData() { }

            public PlacementData(PlacementData other)
            {
                rotationRandomness = other.rotationRandomness;
                rotationOffset = other.rotationOffset;
                scaleRandomnessMax = other.scaleRandomnessMax;
                scaleRandomnessMin = other.scaleRandomnessMin;
                normalAligned = other.normalAligned;
                randomFlipX = other.randomFlipX;
                randomFlipY = other.randomFlipY;
                randomFlipZ = other.randomFlipZ;
            }
        }

        public List<PrefabEntry> entries = new();
        public List<ContainerEntry> containerEntries = new();

        public ItemStack itemData = new();
        public string[] categoryNames = Array.Empty<string>();

        [HideInInspector]
        public int selectedCategory = -1;

        [HideInInspector]
        public Enemy selectedEnemy;

        public PrefabEntry GetPrefabEntry(GameObject prefab) =>
            entries.Find(x => x.prefab = prefab);

#if UNITY_EDITOR

        public static PrefabPlacementData GetPlacementData()
        {
            if (instance != null)
                return instance;
            var guids = AssetDatabase.FindAssets("t:PrefabPlacementData");
            if (guids.Length > 0)
            {
                instance = AssetDatabase.LoadAssetAtPath<PrefabPlacementData>(
                    AssetDatabase.GUIDToAssetPath(guids[0])
                );
                return instance;
            }
            else
            {
                var newData = CreateInstance<PrefabPlacementData>();
                AssetDatabase.CreateAsset(newData, "Assets/PrefabPlacementData.asset");
                instance = newData;
                return instance;
            }
        }

#endif
    }
}
