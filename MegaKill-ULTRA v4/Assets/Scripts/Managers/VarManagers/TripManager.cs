using UnityEngine;

public class TripManager : MonoBehaviour
{
    public static TripManager Instance { get; private set; }

    public int trip;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        trip = 1;
    }
}
