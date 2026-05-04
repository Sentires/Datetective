using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace TheDates.Runtime.UI
{
    public class DialogueChoiceButton : MonoBehaviour, ISelectHandler
    {
        [Header("Components")]
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI choiceText;

        private int choiceIndex = -1;

        public void SetChoiceText(string textString) {
            choiceText.text = textString;
        }

        public void SetChoiceIndex(int index) {
            choiceIndex = index;
        }

        public void SelectButton() {
            button.Select();
        }

        public void OnSelect(BaseEventData eventData) {
            GameEventsManager.Instance.DialogueEvents.UpdateChoiceIndex(choiceIndex);
        }
    }
}
