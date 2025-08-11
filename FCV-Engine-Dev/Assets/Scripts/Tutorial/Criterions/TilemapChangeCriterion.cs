using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace Unity.Tutorials.Core.Editor
{
    public class TilemapChangedCriterion : Criterion
    {

        private bool tilemapChanged = false;

        public override void StartTesting()
        {
            base.StartTesting();
            Tilemap.tilemapTileChanged += (tilemap1, tiles) =>
            {
                tilemapChanged = true;
                UpdateCompletion();
            };
            
        }

        protected override bool EvaluateCompletion()
        {

            return tilemapChanged;
        }

        public override bool AutoComplete()
        {
            return true;
        }
    }
}
#endif