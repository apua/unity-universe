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
                "sphere" => Shapes.Name.Sphere,
                "ring" => Shapes.Name.Ring,
                "pie" => Shapes.Name.Pie,
                "2ring" => Shapes.Name.TwoRing,
                "cage" => Shapes.Name.Cage,
                "2cube" => Shapes.Name.TwoCube,
                "2sphere" => Shapes.Name.TwoSphere,
                "tetrahedron" => Shapes.Name.Tetrahedron,
                "escherian-knot" => Shapes.Name.EscherianKnot,
                _ => throw new System.Exception(),
            });
    }

    public void SetAmount(int amount)
    {
        _amount.text = $"Amount: {amount}";
    }
}
