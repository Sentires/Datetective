using System;
using System.Linq;
using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates.Runtime
{
    [CreateAssetMenu(fileName = "NewCharacterProfile", menuName = GameExtensions.AssetCreationRoot + "Character Profile")]
    public class CharacterProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        // TODO - clean up & refine. The basic idea is there.
        
        [SerializeField]
        private string characterName;
        
        public GridCollection<Sprite> collection = SetupProfiles();
        
        public string CharacterName => characterName;
        
        
        public static readonly string[] Emotions = { "Neutral", "Happy", "Shocked", "Sad", "Angry", "Flustered" };
        public static readonly string[] Appearances = { "Default"};
        
        public string[] TotalEmotionLabels => CustomEmotionLabels.Length != 0 ? Emotions.Concat(CustomEmotionLabels).ToArray() : Emotions;
        public string[] TotalAppearanceLabels => CustomAppearanceLabels.Length != 0 ? Appearances.Concat(CustomAppearanceLabels).ToArray() : Appearances;
        
        [SerializeField]
        private string[] CustomEmotionLabels = Array.Empty<string>();
        [SerializeField]
        private string[] CustomAppearanceLabels = Array.Empty<string>();

        public static GridCollection<Sprite> SetupProfiles() {
            var grid = new GridCollection<Sprite>(Appearances, Emotions);
            return grid;
        }
        
        public void OnBeforeSerialize() {
            collection.columnLabels = TotalAppearanceLabels;
            collection.rowLabels = TotalEmotionLabels;
            collection.Initialize();
            
        }

        public void OnAfterDeserialize() {
            
        }

        public Sprite GetPortrait(int index) {
            if (!TotalEmotionLabels.IsWithinBounds(index)) {
                Debug.Log($"Portrait index {index} is invalid");
                return null;
            }
            
            return collection.columns[0].rows[index];
        }
    }
}
