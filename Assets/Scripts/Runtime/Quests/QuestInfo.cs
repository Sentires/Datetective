using TheDates.Runtime.General;
using UnityEngine;

namespace TheDates.Runtime.Quests
{
    [CreateAssetMenu(fileName = "NewQuest", menuName = GameExtensions.AssetCreationRoot + "Quest Info")]
    public class QuestInfo : ScriptableObject, ISerializationCallbackReceiver
    {
        [Header("General")]
        public string displayName;
        [field: SerializeField, ReadOnly] public string namedID { get; private set; }
        [field: SerializeField, ReadOnly] public int hashID { get; private set; }
        [field: SerializeField, ReadOnly] public string path { get; private set; }
        
        [Header("Requirements")]
        public QuestInfo[] prerequisites;
        
        [Header("Steps")] 
        public GameObject[] stepPrefabs;

        [Header("Outcomes")] 
        public string outcome;

        public void OnBeforeSerialize()
        {
        #if UNITY_EDITOR
            namedID = name;
            hashID = GetHashCode();
            path = UnityEditor.AssetDatabase.GetAssetPath(this);
            UnityEditor.EditorUtility.SetDirty(this);
        #endif
        }

        public void OnAfterDeserialize()
        { }
    }
}
