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
// ::
//      #nullable enable

// Define the object behavior.
public class universe : MonoBehaviour {

    // * Must declare propertiese before using them. Unlike Python.
    // * The public properties will be interactive in Unity object inspector.
    public GameObject Stars; // = default!;
    public int DegreesPerSecond = 36;
    public Vector3Int RotationAxis = new(60, 80, 0);
    // NOTE: "property with setter" would not reflect to inspector; an alternative might be `MonoBehaviour.OnValidate`.

    // * Constants won't be interactiv in Unity object inspector even if public.
    const int DrawAreaWidth = 20;
    const double StarsRadiusRatio = 0.9f;
    const double ShapeRadius = DrawAreaWidth * 0.5 * StarsRadiusRatio;

    // Start is called before the first frame update
    void Start() {
        CreateChildSphere(+1, 0, 0, Color.black);
        CreateChildSphere(-1, 0, 0, Color.clear);
        CreateChildSphere(0, +1, 0, Color.magenta);
        CreateChildSphere(0, -1, 0, Color.magenta);
        CreateChildSphere(0, 0, +1, Color.cyan);
        CreateChildSphere(0, 0, -1, Color.cyan);
    }

    // Update is called once per frame
    void Update() {
        Stars.transform.Rotate(axis: RotationAxis, angle: DegreesPerSecond * Time.deltaTime);
    }

    /* ******************** */

    GameObject CreateChildSphere(int x, int y, int z, Color color) {
        var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        // transform === gameObject.transform
        obj.transform.parent = Stars.transform;
        obj.name = $"Star-{Stars.transform.childCount}";
        obj.transform.localPosition = new Vector3Int(x, y, z);
        obj.GetComponent<Renderer>().material.SetColor("_Color", color);
        return obj;
    }
}
