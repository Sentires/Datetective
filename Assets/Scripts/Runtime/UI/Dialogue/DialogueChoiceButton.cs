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
        [SerializeField] private Image selectionPointer;

        public int choiceIndex { get; private set; }  = -1;

        public void SetChoiceText(string textString) {
            choiceText.text = textString;
        }

        public void SetChoiceIndex(int index) {
            choiceIndex = index;
        }

        public void SetSelectionPointer(bool isSelected) {
            selectionPointer.gameObject.SetActive(isSelected);
        }

        public void SelectButton() {
            button.Select();
        }

        public void OnSelect(BaseEventData eventData) {
            GameEventsManager.Instance.DialogueEvents.UpdateChoiceIndex(choiceIndex);
        }
    }
}
