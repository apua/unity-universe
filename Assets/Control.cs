using UnityEngine;
using UnityEngine.UIElements;

public class Control : MonoBehaviour {

    Button _sphere, _ring, _2ring, _cage, _2cube, _2sphere, _tetrahedron, _escherianKnot;
    //Button _add, _del;

    void OnEnable() {
        var document = GetComponent<UIDocument>();
        var root = document.rootVisualElement;
        //Debug.Log($"{root}");

        _sphere = root.Q("sphere") as Button;
        _ring = root.Q("ring") as Button;
        _2ring = root.Q("2ring") as Button;
        _cage = root.Q("cage") as Button;
        _2cube = root.Q("2cube") as Button;
        _2sphere = root.Q("2sphere") as Button;
        _tetrahedron = root.Q("tetrahedron") as Button;
        _escherianKnot = root.Q("escherian-knot") as Button;

        var universe = transform.parent.GetComponent<Universe>();
        Debug.Log($"{universe}");
        root.Q("add").RegisterCallback<ClickEvent>(evt => universe.AddStar());
        root.Q("del").RegisterCallback<ClickEvent>(evt => universe.DelStar());
    }
}
