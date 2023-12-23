// Here is, after a few weeks, IMO the scope of this little project.
//
// - Eventually store and document "how to build it" on GitHub.
// - Simply a rotating sphere.
// - Light source might be an issue, and not been solved currently.
//
// Ref: "How to set up a Unity project in GitHub"
// https://unityatscale.com/unity-version-control-guide/how-to-setup-unity-project-on-github/
//
// Future works:
//
// - Have a GUI to control it
//   Ref: https://docs.unity3d.com/Manual/GUIScriptingGuide.html
//
// - Walk around and look around the space.
// - Make the space more like a Universe.
//
// Other related:
//
// - `MonoBehaviour.InvokeRepeating`
// - `Time.timeScale`
// - `MonoBehaviour.Awake`
// - `MonoBehaviour.OnValidate`

using UnityEngine;


// `nullable` is a Swift Optional type.
// `#` is a "preprocessor directives".
#nullable enable

// Define the object behavior.
public class universe : MonoBehaviour {

    // * Must declare propertiese before using them. Unlike Python.
    // * The public properties will be interactive in Unity object inspector.
    // * Name in camelCase to gracefully display on field name of Unity object inspector.
    public Vector3Int rotationAxis = new Vector3Int(60, 80, 0);
    public int degreesPerSecond = 36;
    // NOTE: "property with setter" would not reflect to inspector; an alternative might be `MonoBehaviour.OnValidate`.

    // Constants won't be interactiv in Unity object inspector even if public.
    const int drawAreaWidth = 20;
    const double starsRadiusRatio = 0.9f;
    const double shapeRadius = drawAreaWidth * 0.5 * starsRadiusRatio;

    // Start is called before the first frame update
    void Start() {
        CreateChildSphere(Vector3Int.zero, Color.yellow, name: "Center");
        CreateChildSphere(new Vector3(+1, 0, 0), Color.black);
        CreateChildSphere(new Vector3(-1, 0, 0), Color.clear);
        CreateChildSphere(new Vector3(0, +1, 0), Color.magenta);
        CreateChildSphere(new Vector3(0, -1, 0), Color.magenta);
        CreateChildSphere(new Vector3(0, 0, +1), Color.cyan);
        CreateChildSphere(new Vector3(0, 0, -1), Color.cyan);
    }

    // Update is called once per frame
    void Update() {
        transform.Rotate(axis: rotationAxis, angle: degreesPerSecond * Time.deltaTime);
    }

    /* ******************** */

    GameObject CreateChildSphere(Vector3 position, Color color, string? name = null) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // transform === gameObject.transform
        obj.transform.parent = transform;
        obj.name = name ?? $"Star-{transform.childCount-1}";
        obj.transform.localPosition = position;
        obj.GetComponent<Renderer>().material.SetColor("_Color", color);
        return obj;
    }
}
