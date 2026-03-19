using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace TheDates
{
    [CreateAssetMenu(fileName = "NewCharacterProfile", menuName = Helpers.AssetCreationRoot + "Character Profile")]
    public class CharacterProfile : ScriptableObject, ISerializationCallbackReceiver
    {
        // TODO - clean up & refine. The basic idea is there.
        
        [SerializeField]
        private string characterName;
        
        public GridCollection<Texture2D> collection = SetupProfiles();
        
        public string CharacterName => characterName;
        
        
        public static readonly string[] Emotions = { "Neutral", "Happy", "Surprised", "Sad", "Angry", "Flustered" };
        public static readonly string[] Appearances = { "Default"};
        
        public string[] TotalEmotionLabels => CustomEmotionLabels.Length != 0 ? Emotions.Concat(CustomEmotionLabels).ToArray() : Emotions;
        public string[] TotalAppearanceLabels => CustomAppearanceLabels.Length != 0 ? Appearances.Concat(CustomAppearanceLabels).ToArray() : Appearances;
        
        [SerializeField]
        private string[] CustomEmotionLabels = Array.Empty<string>();
        [SerializeField]
        private string[] CustomAppearanceLabels = Array.Empty<string>();

        public static GridCollection<Texture2D> SetupProfiles() {
            var grid = new GridCollection<Texture2D>(Appearances, Emotions);
            return grid;
        }
        
        public void OnBeforeSerialize() {
            collection.columnLabels = TotalAppearanceLabels;
            collection.rowLabels = TotalEmotionLabels;
            collection.Initialize();
            
        }

        public void OnAfterDeserialize() {
            
        }
    }
}
