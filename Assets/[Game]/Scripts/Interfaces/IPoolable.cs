using UnityEngine;

public interface IPoolable
{
    public void DisableFromPool(Transform _poolHolder);

    public void SpawnFromPool(params object[] _args);

} // interface
