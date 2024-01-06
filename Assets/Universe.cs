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
    public ushort Amount;
    public Shapes Shape;

    Vector3[] _finalPositions;
    Vector3[] _velocities;
    float _transformShapeDelay = 0f;

    const float TotalTransformShapeDelay = 1.25f;

    void Start()
    {
        AdjustQuantity();
        SetColor();
    }

    void Update()
    {
        AdjustQuantity();
        SetColor();
        Rotate();
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

    void AdjustQuantity()
    {
        var count = stars.transform.childCount;
        if (Amount > count)
        {
            AddStar();
            control.SetAmount(count + 1);
        }
        else if (Amount < count)
        {
            DelStar();
            control.SetAmount(count - 1);
        }
        else
        {}
    }

    void SetColor()
    {
        // Change color:
        // B   -> G   -> R   -> B i.e.
        // 001 -> 010 -> 100 -> 001 i.e.
        // 0   -> 1   -> 2   -> 3
        // 0 -> 1 taks 2.5 seconds; per second is 0 -> 0.4
        var t = Time.time * 0.4f % 3;
        Color color;
        if (t <= 1)
        {
            color = new(0, t, 1 - t);
        }
        else if (t <= 2)
        {
            t -= 1;
            color = new(t, 1 - t, 0);
        }
        else
        {
            t -= 2;
            color = new(1 - t, 0, t);
        }
        foreach (Transform starTransform in stars.transform)
            starTransform.gameObject.GetComponent<Renderer>().material.color = color;
    }

    void Rotate()
    {
        stars.transform.Rotate(axis: RotationAxis, angle: DegreePerSecond * Time.deltaTime);
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
