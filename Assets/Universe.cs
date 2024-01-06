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

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Universe : MonoBehaviour
{
    public Control control;
    public GameObject prototype;
    public GameObject stars;
    public int DegreePerSecond;
    public Vector3Int RotationAxis;
    public ushort Amount;
    public Shapes Shape;

    Shapes _currentShape, _transformShape;
    float _transformDeltaTime = 0;
    List<Vector3> _transformVelocities = new();

    void Start()
    {
        _currentShape = _transformShape = Shape;
        AdjustQuantity();
        SetColor();
    }

    void Update()
    {
        AdjustQuantity();
        SetColor();
        Rotate();
        TransformShape();
    }

    void AdjustQuantity()
    {
        var count = stars.transform.childCount;
        if (count < Amount)
        {
            AddStar();
            control.SetAmount(count + 1);
        }
        else if (count > Amount)
        {
            DelStar();
            control.SetAmount(count - 1);
        }
        else
        {}
    }

    void SetColor()
    {
        // Every color changing takes 2.5 seconds.
        var t = Time.time * 0.4f % 3;
        Color color;
        if (t <= 1)
        {   // Blue (0, 0, 1) → Green (0, 1, 0)
            color = new(0, t, 1 - t);
        }
        else if (t <= 2)
        {   // Green (0, 1, 0) → Red (1, 0, 0)
            t -= 1;
            color = new(t, 1 - t, 0);
        }
        else
        {   // Red (1, 0, 0) → Blue (0, 0, 1)
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

    void TransformShape()
    {
        if (_currentShape == Shape)
            return;

        if (_transformShape != Shape)
        {   // Start or restart a transformation.
            _transformShape = Shape;
            _transformDeltaTime = 1.25f;
            _transformVelocities.AddRange(Enumerable.Range(0, stars.transform.childCount).Select(index =>
                (GenerateRandomPoint(_transformShape) - stars.transform.GetChild(index).localPosition) / _transformDeltaTime
            ));
        }
        else
        {   // `childCount` changes during a transformation.
            var addition = stars.transform.childCount - _transformVelocities.Count;
            if (addition > 0)
            {
                var count = addition;
                var start = stars.transform.childCount - count;
                _transformVelocities.AddRange(Enumerable.Range(start, count).Select(index =>
                    (GenerateRandomPoint(_transformShape) - stars.transform.GetChild(index).localPosition) / _transformDeltaTime
                ));
            }
            else if (addition < 0)
            {
                var count = -addition;
                var index = _transformVelocities.Count - count;
                _transformVelocities.RemoveRange(index, count);
            }
            else
            {}
        }

        if (_transformDeltaTime > Time.deltaTime)
        {
            for (int i = 0; i < stars.transform.childCount; i++)
                stars.transform.GetChild(i).localPosition += _transformVelocities[i] * Time.deltaTime;

            _transformDeltaTime -= Time.deltaTime;
        }
        else
        {
            for (int i = 0; i < stars.transform.childCount; i++)
                stars.transform.GetChild(i).localPosition += _transformVelocities[i] * _transformDeltaTime;

            _currentShape = _transformShape = Shape;
            _transformDeltaTime = 0f;
            _transformVelocities.Clear();
        }
    }

    public void AddStar()
    {
        var obj = Instantiate(prototype);
        obj.transform.parent = stars.transform;
        obj.name = $"Star-{stars.transform.childCount}";
        obj.transform.localPosition = GenerateRandomPoint(_currentShape);
        obj.SetActive(true);
    }

    public void DelStar()
    {
        var index = stars.transform.childCount - 1;
        Destroy(stars.transform.GetChild(index).gameObject);
    }

    public Vector3 GenerateRandomPoint(Shapes shape)
    {
        return shape switch {
            Shapes.Sphere => PointGenerators.Sphere(),
            _ => PointGenerators.Ring(),
        };
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
            Radius * Mathf.Sin(φ) * Mathf.Sin(θ)
        );
    }

    public static Vector3 Ring()
    {
        var θ = Random.value * 2 * Mathf.PI;
        return new(
            Radius * Mathf.Cos(θ),
            0,
            Radius * Mathf.Sin(θ)
        );
    }
}

public enum Shapes
{
    Sphere,
    Ring,
}
