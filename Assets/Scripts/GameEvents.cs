using System;
using UnityEngine;
using UnityEngine.Events;

//using unity events instead of messenger system from chap. 7 to improve scalability

public static class GameEvents
{
    public static event Action OnEnemyKilled;
    public static event Action<float> OnSpeedChanged;
    
    public static void EnemyKilled()
    {
        OnEnemyKilled?.Invoke();
    }

    public static void SpeedChanged(float value)
    {
        OnSpeedChanged?.Invoke(value);
    }
}
