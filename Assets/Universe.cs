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
    public Shapes.Name Shape;


    Shapes.Name _currentShape, _transformShape;
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
                (Shapes.GeneratePoint(_transformShape) - stars.transform.GetChild(index).localPosition) / _transformDeltaTime
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
                    (Shapes.GeneratePoint(_transformShape) - stars.transform.GetChild(index).localPosition) / _transformDeltaTime
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
        obj.transform.localPosition = Shapes.GeneratePoint(_currentShape);
        obj.SetActive(true);
    }

    public void DelStar()
    {
        var index = stars.transform.childCount - 1;
        Destroy(stars.transform.GetChild(index).gameObject);
    }
}

public class Shapes
{
    const ushort Radius = 10;

    public enum Name
    {
        Sphere,
        Ring,
        Pie,
        TwoRing,
        Cage,
        TwoCube,
        TwoSphere,
        Tetrahedron,
        EscherianKnot,
    }

    public static Name ToEnum(string shapeName)
    {
        return (Name)System.Enum.Parse(typeof(Name), shapeName);
    }

    public static Vector3 GeneratePoint(Shapes.Name shape)
    {
        var methodInfo = typeof(Shapes).GetMethod(shape.ToString());
        return (Vector3)methodInfo.Invoke(null, null);
        //return shape switch
        //{
        //    Shapes.Name.Sphere => Sphere(),
        //    Shapes.Name.Ring => Ring(),
        //    Shapes.Name.Pie => Pie(),
        //    Shapes.Name.TwoRing => TwoRing(),
        //    Shapes.Name.Cage => Cage(),
        //    Shapes.Name.TwoCube => TwoCube(),
        //    Shapes.Name.TwoSphere => TwoSphere(),
        //    Shapes.Name.Tetrahedron => Tetrahedron(),
        //    Shapes.Name.EscherianKnot => EscherianKnot(),
        //    _ => throw new System.Exception(),
        //};
    }

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

    public static Vector3 Pie()
    {
        var θ = Random.value * 2 * Mathf.PI;
        var r = Mathf.Sqrt(Random.value);
        return new(
            Radius * r * Mathf.Cos(θ),
            0,
            Radius * r * Mathf.Sin(θ)
        );
    }

    public static Vector3 TwoRing()
    {
        var θ = Random.value * 2 * Mathf.PI;
        if (Random.value < 0.5)
            return new(
                Radius * 2/3 * Mathf.Cos(θ) + Radius / 3,
                0,
                Radius * 2/3 * Mathf.Sin(θ)
            );
        else
            return new(
                Radius * 2/3 * Mathf.Cos(θ) - Radius / 3,
                Radius * 2/3 * Mathf.Sin(θ),
                0
            );
    }

    public static Vector3 Cage()
    {
        var θ = Random.value * 2 * Mathf.PI;
        return (Random.value * 3) switch
        {
            < 1 => new(0, Radius * Mathf.Cos(θ), Radius * Mathf.Sin(θ)),
            < 2 => new(Radius * Mathf.Cos(θ), 0, Radius * Mathf.Sin(θ)),
            < 3 => new(Radius * Mathf.Cos(θ), Radius * Mathf.Sin(θ), 0),
            _ => throw new System.Exception(),
        };
    }

    public static Vector3 TwoCube()
    {
        var θ = Random.value * 2 * Mathf.PI;
        var r = Radius / Mathf.Sqrt(3) / (Random.value * 3 < 1 ? 2 : 1);
        var p = Random.value * 2 - 1;
        return (Random.value * 12) switch
        {
            <  1 => new(p * r,  r,  r),
            <  2 => new(p * r,  r, -r),
            <  3 => new(p * r, -r,  r),
            <  4 => new(p * r, -r, -r),
            <  5 => new( r, p * r,  r),
            <  6 => new( r, p * r, -r),
            <  7 => new(-r, p * r,  r),
            <  8 => new(-r, p * r, -r),
            <  9 => new( r,  r, p * r),
            < 10 => new( r, -r, p * r),
            < 11 => new(-r,  r, p * r),
            < 12 => new(-r, -r, p * r),
            _ => throw new System.Exception(),
        };
    }

    public static Vector3 TwoSphere()
    {
        var θ = Random.value * 2 * Mathf.PI;
        var φ = Random.value * 2 * Mathf.PI;
        var x = Random.value < 0.5 ? 1 : -1;
        return new(
            Radius / 2 * Mathf.Cos(φ) * Mathf.Cos(θ) + Radius * 2/3 * x,
            Radius / 2 * Mathf.Sin(φ),
            Radius / 2 * Mathf.Cos(φ) * Mathf.Sin(θ)
        );
    }

    public static Vector3 Tetrahedron()
    {
        var (v, w) = (Random.value * 6) switch
        {
            <  1 => ((X:  1, Y:  1, Z: 1), (X: -1, Y: -1, Z:  1)),
            <  2 => ((X:  1, Y:  1, Z: 1), (X:  1, Y: -1, Z: -1)),
            <  3 => ((X:  1, Y:  1, Z: 1), (X: -1, Y:  1, Z: -1)),
            <  4 => ((X: -1, Y: -1, Z: 1), (X:  1, Y: -1, Z: -1)),
            <  5 => ((X: -1, Y: -1, Z: 1), (X: -1, Y:  1, Z: -1)),
            <  6 => ((X:  1, Y: -1, Z:-1), (X: -1, Y:  1, Z: -1)),
            _ => throw new System. Exception(),
        };
        var p = Random.value;
        return new(
            Radius * 2/3 * (v.X + p * (w.X - v.X)),
            Radius * 2/3 * (v.Y + p * (w.Y - v.Y)),
            Radius * 2/3 * (v.Z + p * (w.Z - v.Z))
        );
    }

    public static Vector3 EscherianKnot()
    {
        var θ = Random.value * 2 * Mathf.PI;
        var φ = (Mathf.Cos(3 * θ) + 3) * Mathf.PI / 6;
        var r = (Mathf.Sin(3 * θ) + 3) * Radius / 4;
        return new(
            r * Mathf.Sin(φ) * Mathf.Cos(2 * θ),
            r * Mathf.Cos(φ),
            r * Mathf.Sin(φ) * Mathf.Sin(2 * θ)
        );
    }
}
