using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class GridPaletteTileCountCriterion : Criterion
    {
        [SerializeField] private string gridPalettePath;
        [SerializeField] private ArrayChangeCriterion.Operation operation;
        [SerializeField] private int value = 0;
        public override void StartTesting()
        {
            base.StartTesting();
            UpdateCompletion();

            EditorApplication.update += UpdateCompletion;
        }

        public override void StopTesting()
        {
            base.StopTesting();

            EditorApplication.update -= UpdateCompletion;
        }

        protected override bool EvaluateCompletion()
        {
            GameObject palettePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(gridPalettePath);
            
            if (palettePrefab == null)
            {
                Debug.LogError("Palette GameObject could not be loaded from the asset.");
                return true;
            }

            int tileCount = 0;

            // Loop through child objects in the palette's prefab (these are Tilemap layers)
            foreach (Transform child in palettePrefab.transform)
            {
                Tilemap tilemap = child.GetComponent<Tilemap>();
                if (tilemap != null)
                {
                    // Get the bounds of the Tilemap and count all tiles
                    BoundsInt bounds = tilemap.cellBounds;
                    for (int x = bounds.xMin; x < bounds.xMax; x++)
                    {
                        for (int y = bounds.yMin; y < bounds.yMax; y++)
                        {
                            for (int z = bounds.zMin; z < bounds.zMax; z++)
                            {
                                Vector3Int pos = new Vector3Int(x, y, z);
                                if (tilemap.HasTile(pos))
                                {
                                    tileCount++;
                                }
                            }
                        }
                    }
                }
            }
            
            Debug.Log(tileCount);

            switch (operation)
            {
                case ArrayChangeCriterion.Operation.Greater:
                    return tileCount > value;
                case ArrayChangeCriterion.Operation.Less:
                    return tileCount < value;
                default:
                    return true;
            }
        }

        public override bool AutoComplete()
        {
            return false;
        }
    }
}
#endif
