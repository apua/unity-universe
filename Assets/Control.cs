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

        root.Q("add").RegisterCallback<ClickEvent>(evt => universe.Amount += (ushort)(universe.Amount < 65535 ? 1 : 0));
        root.Q("del").RegisterCallback<ClickEvent>(evt => universe.Amount -= (ushort)(universe.Amount > 0     ? 1 : 0));

        foreach (var name in new string[] { "sphere", "ring", "pie", "2ring", "cage", "2cube", "2sphere", "tetrahedron", "escherian-knot" })
            root.Q(name).RegisterCallback<ClickEvent>(evt => universe.Shape = name switch {
                "sphere" => Shapes.Sphere,
                "ring" => Shapes.Ring,
                "pie" => Shapes.Pie,
                "2ring" => Shapes.TwoRing,
                "cage" => Shapes.Cage,
                "2cube" => Shapes.TwoCube,
                "2sphere" => Shapes.TwoSphere,
                "tetrahedron" => Shapes.Tetrahedron,
                "escherian-knot" => Shapes.EscherianKnot,
                _ => throw new System.Exception(),
            });
    }

    public void SetAmount(int amount)
    {
        _amount.text = $"Amount: {amount}";
    }
}
