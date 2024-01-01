using UnityEngine;

public class Universe : MonoBehaviour {

    GameObject _prototype, _stars;
    Color _color = new(0, 0, 1, 0.5f);

    const ushort InitialAmount = 42;

    public int DegreePerSecond = 36;
    public Vector3Int RotationAxis = new(60, 80, 0);

    void Start() {
        _prototype = transform.Find("StarPrototype").gameObject;
        _stars = transform.Find("Stars").gameObject;
        for (int i = 0; i < InitialAmount; i++) AddStar();
    }

    void Update() {
        // Rotate.
        _stars.transform.Rotate(axis: RotationAxis, angle: DegreePerSecond * Time.deltaTime);
        // Color.
        UpdateCurrentColor();
        foreach (Transform starTransform in _stars.transform) {
            starTransform.gameObject.GetComponent<Renderer>().material.color = _color;
        }
    }

    /* ******************** */

    void AddStar() {
        // New.
        var obj = Instantiate(_prototype);
        // Set parent to `Stars`.
        obj.transform.parent = _stars.transform;
        // Name "Star-{N}".
        obj.name = $"Star-{_stars.transform.childCount}";
        // Color.
        obj.GetComponent<Renderer>().material.color = _color;
        // Set random position.
        obj.transform.localPosition = PointGenerators.Ring();
        //obj.transform.localPosition = PointGenerators.Sphere();
        // Enable.
        obj.SetActive(true);
    }

    void UpdateCurrentColor() {
        // Change color:
        // B   -> G   -> R   -> B i.e.
        // 001 -> 010 -> 100 -> 001 i.e.
        // 0   -> 1   -> 2   -> 3
        // 0 -> 1 taks 2.5 seconds; per second is 0 -> 0.4
        var colorAcc = Time.time * 0.4f % 3;
        if (colorAcc <= 1) {
            var x = colorAcc;
            _color.r = 0; _color.g = x; _color.b = 1 - x;
        } else if (colorAcc <= 2) {
            var x = colorAcc - 1;
            _color.b = 0; _color.r = x; _color.g = 1 - x;
        } else {
            var x = colorAcc - 2;
            _color.g = 0; _color.b = x; _color.r = 1 - x;
        }
    }
}

class PointGenerators {
    const ushort Radius = 10;

    public static Vector3 Sphere() {
        var θ = Random.value * 2 * Mathf.PI;
        var φ = Mathf.Sqrt(Random.value) * Mathf.PI / 2;
        return new(
            Radius * Mathf.Sin(φ) * Mathf.Cos(θ),
            Radius * Mathf.Cos(φ) * (Random.value > 0.5 ? 1 : -1),
            Radius * Mathf.Sin(φ) * Mathf.Sin(θ));
    }

    public static Vector3 Ring() {
        var θ = Random.value * 2 * Mathf.PI;
        return new(
            Radius * Mathf.Cos(θ),
            0,
            Radius * Mathf.Sin(θ));
    }
}