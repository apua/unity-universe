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

        root.Q("add").RegisterCallback<ClickEvent>(evt => universe.amount += (ushort)(universe.amount < 65535 ? 1 : 0));
        root.Q("del").RegisterCallback<ClickEvent>(evt => universe.amount -= (ushort)(universe.amount > 0     ? 1 : 0));

        string[] shapeNames = {
            "sphere",
            "ring",
            "2ring",
            "cage",
            "2cube",
            "2sphere",
            "tetrahedron",
            "escherian-knot",
        };
        foreach (var name in shapeNames)
            root.Q(name).RegisterCallback<ClickEvent>(evt => universe.SetShape(name));
    }

    public void SetAmount(int amount)
    {
        _amount.text = $"Amount: {amount}";
    }
}
