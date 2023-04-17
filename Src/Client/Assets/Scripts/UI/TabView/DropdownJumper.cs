using UnityEngine;
using UnityEngine.UI;

public class DropdownJumper : MonoBehaviour
{
    public TabButton[] buttons;
    private Dropdown dropdown;

    void Start()
    {
        dropdown = GetComponent<Dropdown>();
        dropdown.onValueChanged.AddListener(TriggerButton);
    }

    private void TriggerButton(int arg0)
    {
        buttons[arg0].OnClick();
    }
}
