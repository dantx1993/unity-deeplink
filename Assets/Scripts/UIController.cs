using TMPro;
using UnityEngine;

namespace UnityDeeplinkDemo
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _txtValue;

        public void ChangeValueText(string valueString)
        {
            _txtValue.text = valueString;
        }
    }
}
