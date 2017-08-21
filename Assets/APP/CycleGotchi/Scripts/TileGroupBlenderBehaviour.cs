using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace CycleGotchi
{
    /// <summary>
    /// This Class manages separating th Tiles into distinct groups,
    /// to build sharedMaterials for blending them synchronously (or for other effects),
    /// without each tile having to create an own instanced material
    /// 
    /// THIS IS STILL A STUB
    /// </summary>
    public class TileGroupBlenderBehaviour : MonoBehaviour
    {
        [Range(0f, 10f)]
        public float BlinkSpeed = 1.5f;
        [Range(0f, 1f)]
        public float BlinkStrength = 0.2f;
        [Range(0f, 2f)]
        public float BlendSpeed = 1f;

        #region Childtypes

        public class TileGroup
        {
            public List<TileBehaviour> Tiles = new List<TileBehaviour>();
            public Dictionary<Material, Material> SharedMaterials = null;
            public Effect CurrentEffect = Effect.NONE;

            public enum Effect
            {
                NONE,
                BLENDING,
                BLINKING
            }

            public TileGroup(int id)
            {
                mGroupID = id;
            }

            public float BlendProgress
            {
                get
                {
                    return mBlendProgress;
                }
                set
                {
                    mBlendProgress = Mathf.Clamp01(value);
                }
            }

            /// <summary>
            /// Returns a value between 0f and 1f
            /// When setting, pingpongs automatically
            /// </summary>
            public float BlinkProgress
            {
                get
                {
                    if (mBlinkProgress > 1f)
                        return 2f - mBlinkProgress;
                    else
                        return mBlinkProgress;
                }
            }
            public float BlinkProgressRaw
            {
                get
                {
                    return mBlinkProgress;
                }
                set
                {
                    mBlinkProgress = value;
                    while (mBlinkProgress > 2f)
                        mBlinkProgress -= 2f;
                    while (mBlinkProgress < 0f)
                        mBlinkProgress += 2f;
                }
            }

            public int GroupID
            {
                get
                {
                    return mGroupID;
                }
            }

            float mBlendProgress = 0f;
            //PingPongs from 0f to 2f
            float mBlinkProgress = 0f;

            int mGroupID = 0;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Searches if this tile is present in any group.
        /// </summary>
        /// <param name="tile"></param>
        /// <returns>Returns the Griooup containing the searched Tile, null if not found</returns>
        public TileGroup getGroupContaining(TileBehaviour tile)
        {
            foreach (TileGroup group in mGroups)
            {
                if (group.Tiles.Contains(tile))
                    return group;
            }
            return null;
        }

        /// <summary>
        /// Defines a new Group.
        /// Only tiles are inserted, that are not in another group already
        /// </summary>
        /// <param name="tiles"></param>
        /// <returns></returns>
        public TileGroup defineNewGroup(List<TileBehaviour> tiles)
        {
            mGroupCounter++;
            TileGroup group = new TileGroup(mGroupCounter);
            foreach (TileBehaviour tile in tiles)
            {
                if (getGroupContaining(tile) == null)
                    group.Tiles.Add(tile);
            }
            mGroups.Add(group);
            return group;
        }

        public void removeGroupEffects(TileGroup group)
        {
            if (group.SharedMaterials != null)
            {
                revertMaterials(group, group.SharedMaterials);
                clearOutMaterials(group.SharedMaterials);
                group.SharedMaterials = null;
            }
        }

        public void releaseGroup(TileGroup group)
        {
            removeGroupEffects(group);
            mGroups.Remove(group);
        }

        public void releaseAllGroups()
        {
            foreach (TileGroup group in mGroups)
            {
                removeGroupEffects(group);
            }
            mGroups.Clear();
        }

        #endregion


        #region Private Static

        static HashSet<Material> collectMaterials(TileGroup group)
        {
            HashSet<Material> materials = new HashSet<Material>();
            foreach (TileBehaviour tile in group.Tiles)
            {
                if (tile == null)
                    continue;
                foreach (Renderer renderer in tile.Blendables)
                {
                    if (renderer == null)
                        continue;
                    foreach (Material material in renderer.sharedMaterials)
                    {
                        materials.Add(material);
                    }
                }
            }

            return materials;
        }

        /// <summary>
        /// For each Material in the HashSet, create a new entry in a Dictionary, and set an new instatiated copy in the value
        /// </summary>
        /// <param name="originalMaterials"></param>
        /// <returns>A materialMap, where the eky is the ooriginal material</returns>
        static Dictionary<Material, Material> cloneMaterials(int groupID, HashSet<Material> originalMaterials)
        {
            Dictionary<Material, Material> materialMap = new Dictionary<Material, Material>();

            foreach (Material originalMaterial in originalMaterials)
            {
                Material clone = new Material(originalMaterial);
                clone.name = clone.name + "[TileGroup:"+groupID+"]";
                materialMap.Add(originalMaterial, clone);
            }

            return materialMap;
        }

        /// <summary>
        /// Wlaks through the Blendable renderers of each tile, and replaces their shared materials with new materials, defined by
        /// the materialMap. If the original material is not present as key in the map, nothing is done.
        /// </summary>
        /// <param name="group"></param>
        /// <param name="materialMap"></param>
        static void replaceMaterials(TileGroup group, Dictionary<Material, Material> materialMap)
        {
            foreach (TileBehaviour tile in group.Tiles)
            {
                if (tile == null)
                    continue;
                foreach (Renderer renderer in tile.Blendables)
                {
                    if (renderer == null)
                        continue;
                    //Get a copy of the shared materials
                    Material[] sharedMaterials = renderer.sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        Material material = sharedMaterials[i];
                        Material replacementMaterial;

                        if (materialMap.TryGetValue(material, out replacementMaterial))
                        {
                            //A replacement is provided
                            sharedMaterials[i] = replacementMaterial;
                        }
                    }
                    //Overwrite the original with the copy
                    renderer.sharedMaterials = sharedMaterials;
                }
            }
        }

        /// <summary>
        /// Reverts the effect of replaceMaterials, 
        /// by looking up the values instead of keys and setting the materials back to the original value
        /// </summary>
        /// <param name="group"></param>
        /// <param name="materialMap"></param>
        static void revertMaterials(TileGroup group, Dictionary<Material, Material> materialMap)
        {
            foreach (TileBehaviour tile in group.Tiles)
            {
                if (tile == null)
                    continue;

                foreach (Renderer renderer in tile.Blendables)
                {
                    if (renderer == null)
                        continue;

                    //Get a copy of the shared materials
                    Material[] sharedMaterials = renderer.sharedMaterials;
                    for (int i = 0; i < sharedMaterials.Length; i++)
                    {
                        Material material = sharedMaterials[i];

                        foreach (KeyValuePair<Material, Material> pair in materialMap)
                        {
                            if (pair.Value == material)
                            {
                                //Revert back to the old material
                                sharedMaterials[i] = pair.Key;
                            }
                        }
                    }
                    //Overwrite the original with the copy
                    renderer.sharedMaterials = sharedMaterials;
                }
            }
        }

        /// <summary>
        /// Deletes clones of materials (the values in the materialMap)
        /// </summary>
        /// <param name="materialMap"></param>
        static void clearOutMaterials(Dictionary<Material, Material> materialMap)
        {
            foreach (Material material in materialMap.Values)
            {
                Destroy(material);
            }
        }

        #endregion


        #region Private Fields

        List<TileGroup> mGroups = new List<TileGroup>();
        int mGroupCounter = 0;

        #endregion

        #region Private Methods

        void enableGroupForEffects(TileGroup group)
        {
            if (group.SharedMaterials == null)
            {
                group.SharedMaterials = cloneMaterials(group.GroupID, collectMaterials(group));
                replaceMaterials(group, group.SharedMaterials);
            }
        }

        void applyEffectsToTileGroupMaterials(TileGroup group)
        {
            foreach (Material material in group.SharedMaterials.Values)
            {
                switch (group.CurrentEffect)
                {
                    case TileGroup.Effect.BLINKING:
                        material.SetFloat("_BlendProgress", group.BlinkProgress * BlinkStrength);
                        break;
                    case TileGroup.Effect.BLENDING:
                        material.SetFloat("_BlendProgress", group.BlendProgress);
                        break;
                    default:
                        material.SetFloat("_BlendProgress", 0);
                        break;
                }
            }
        }

        #endregion

        #region Unity Messages / Events

        void Update()
        {
            foreach (TileGroup group in mGroups)
            {
                if (group.CurrentEffect != TileGroup.Effect.NONE)
                {
                    enableGroupForEffects(group);

                    switch (group.CurrentEffect)
                    {
                        case TileGroup.Effect.BLINKING:
                            group.BlinkProgressRaw += Time.deltaTime * BlinkSpeed;
                            break;
                        case TileGroup.Effect.BLENDING:
                            group.BlendProgress += Time.deltaTime * BlendSpeed;
                            break;
                    }
                }
                
                if (group.SharedMaterials != null)
                {
                    applyEffectsToTileGroupMaterials(group);
                }
            }
        }

        void OnDisable()
        {

            foreach (TileGroup group in mGroups)
            {
                removeGroupEffects(group);
            }
        }


        #endregion
    }
}