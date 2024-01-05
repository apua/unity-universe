/*
 * TODO: Refactorign
 * 1. coding style, ref: https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md
 * 2. MVC
 * 3. Dynamic set shapes
 *
 * The model have some variables "fields" allow to be modified in runtime:
 * - amount
 * - shape
 */

using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Universe : MonoBehaviour
{
    public Control control;
    public GameObject prototype;
    public GameObject stars;
    public int DegreePerSecond;
    public Vector3Int RotationAxis;
    public ushort amount;
    public Shapes Shape;

    Color _color = new(0, 0, 1);
    Vector3[] _finalPositions;
    Vector3[] _velocities;
    float _transformShapeDelay = 0f;

    const float TotalTransformShapeDelay = 1.25f;

    void Start()
    {
        UpdateAmount();
    }

    void Update()
    {
        UpdateAmount();
        // Rotate.
        stars.transform.Rotate(axis: RotationAxis, angle: DegreePerSecond * Time.deltaTime);
        // Color.
        UpdateCurrentColor();
        foreach (Transform starTransform in stars.transform)
            starTransform.gameObject.GetComponent<Renderer>().material.color = _color;
        // Transform.
        if (_transformShapeDelay > 0)
            if (_transformShapeDelay > Time.deltaTime)
            {
                for (int i = 0; i < stars.transform.childCount; i++)
                    stars.transform.GetChild(i).localPosition += _velocities[i] * Time.deltaTime;
                _transformShapeDelay -= Time.deltaTime;
            }
            else
            {
                for (int i = 0; i < stars.transform.childCount; i++)
                    //stars.transform.GetChild(i).localPosition += _velocities[i] * _transformShapeDelay;
                    stars.transform.GetChild(i).localPosition = _finalPositions[i];
                _transformShapeDelay = 0f;
            }
    }

    void UpdateAmount()
    {
        var count = stars.transform.childCount;
        if (amount > count)
        {
            AddStar();
            control.SetAmount(count + 1);
        }
        if (amount < count)
        {
            DelStar();
            control.SetAmount(count - 1);
        }
    }

    /* ******************** */

    public void AddStar()
    {
        // New.
        var obj = Instantiate(prototype);
        // Set parent to `Stars`.
        obj.transform.parent = stars.transform;
        // Name "Star-{N}".
        obj.name = $"Star-{stars.transform.childCount}";
        // Color.
        obj.GetComponent<Renderer>().material.color = _color;
        // Set random position.
        //obj.transform.localPosition = PointGenerators.Ring();
        obj.transform.localPosition = PointGenerators.Sphere();
        // Enable.
        obj.SetActive(true);
    }

    public void DelStar()
    {
        var index = stars.transform.childCount - 1;
        Destroy(stars.transform.GetChild(index).gameObject);
    }

    void UpdateCurrentColor()
    {
        // Change color:
        // B   -> G   -> R   -> B i.e.
        // 001 -> 010 -> 100 -> 001 i.e.
        // 0   -> 1   -> 2   -> 3
        // 0 -> 1 taks 2.5 seconds; per second is 0 -> 0.4
        var colorAcc = Time.time * 0.4f % 3;
        if (colorAcc <= 1)
        {
            var x = colorAcc;
            _color.r = 0; _color.g = x; _color.b = 1 - x;
        }
        else if (colorAcc <= 2)
        {
            var x = colorAcc - 1;
            _color.b = 0; _color.r = x; _color.g = 1 - x;
        }
        else
        {
            var x = colorAcc - 2;
            _color.g = 0; _color.b = x; _color.r = 1 - x;
        }
    }

    public void SetShape(string name)
    {
        Func<Vector3> generator = name switch
        {
            "sphere" => PointGenerators.Sphere,
            _ => PointGenerators.Ring,
        };

        var N = stars.transform.childCount;
        _finalPositions = new Vector3[N];
        _velocities = new Vector3[N];
        _transformShapeDelay = TotalTransformShapeDelay;
        for (int i = 0; i < N; i++)
        {
            _finalPositions[i] = generator();
            _velocities[i] = (_finalPositions[i] - stars.transform.GetChild(i).localPosition) / TotalTransformShapeDelay;
        }
    }
}

class PointGenerators
{
    const ushort Radius = 10;

    public static Vector3 Sphere()
    {
        var θ = Random.value * 2 * Mathf.PI;
        var φ = Mathf.Sqrt(Random.value) * Mathf.PI / 2;
        return new(
            Radius * Mathf.Sin(φ) * Mathf.Cos(θ),
            Radius * Mathf.Cos(φ) * (Random.value > 0.5 ? 1 : -1),
            Radius * Mathf.Sin(φ) * Mathf.Sin(θ));
    }

    public static Vector3 Ring()
    {
        var θ = Random.value * 2 * Mathf.PI;
        return new(
            Radius * Mathf.Cos(θ),
            0,
            Radius * Mathf.Sin(θ));
    }
}

public enum Shapes
{
    Sphere,
    Ring,
}
