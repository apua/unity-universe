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

        foreach (var name in new string[] { "sphere", "ring", "2ring", "cage", "2cube", "2sphere", "tetrahedron", "escherian-knot" })
            root.Q(name).RegisterCallback<ClickEvent>(evt => universe.Shape = name switch {
                "sphere" => Shapes.Sphere,
                _ => Shapes.Ring,
            });
    }

    public void SetAmount(int amount)
    {
        _amount.text = $"Amount: {amount}";
    }
}
