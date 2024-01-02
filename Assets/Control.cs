using UnityEngine;
using UnityEngine.UIElements;

public class Control : MonoBehaviour {

    Button _sphere, _ring, _2ring, _cage, _2cube, _2sphere, _tetrahedron, _escherianKnot;
    Label _amount;

    void OnEnable() {
        var root = GetComponent<UIDocument>().rootVisualElement;

        _sphere = root.Q("sphere") as Button;
        _ring = root.Q("ring") as Button;
        _2ring = root.Q("2ring") as Button;
        _cage = root.Q("cage") as Button;
        _2cube = root.Q("2cube") as Button;
        _2sphere = root.Q("2sphere") as Button;
        _tetrahedron = root.Q("tetrahedron") as Button;
        _escherianKnot = root.Q("escherian-knot") as Button;

        _amount = root.Q("Amount") as Label;

        var universe = GetComponentInParent<Universe>();
        root.Q("add").RegisterCallback<ClickEvent>(evt => universe.AddStar(action: SetAmount));
        root.Q("del").RegisterCallback<ClickEvent>(evt => universe.DelStar(action: SetAmount));

    }

    public void SetAmount(int amount) {
        _amount.text = $"Amount: {amount}";
    }
}
