using UnityEngine;
using UnityEngine.UIElements;

public class Control : MonoBehaviour
{
    Label _amount;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        var universe = GetComponentInParent<Universe>();

        _amount = root.Q("Amount") as Label;

        root.Query<Button>().ForEach(btn => btn.RegisterCallback<ClickEvent>(evt => {
            if (btn.name == "Add")
                universe.Amount += (ushort)(universe.Amount < 65535 ? 1 : 0);
            else if (btn.name == "Del")
                universe.Amount -= (ushort)(universe.Amount > 0     ? 1 : 0);
            else
                universe.Shape = Shapes.ToEnum(btn.name);
        }));
    }

    public void SetAmount(int amount)
    {
        _amount.text = $"Amount: {amount}";
    }
}
