using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates
{
    // This just allows me to easily change a minigame's prefab without breaking references
    [CreateAssetMenu(fileName = "NewMiniGameAsset", menuName = GameExtensions.AssetCreationRoot + "Asset Reference")]
    public class MiniGameAssetReference : ScriptableObject {
        [field: SerializeField] public GameObject miniGamePrefab { get; private set; }
    }
}
