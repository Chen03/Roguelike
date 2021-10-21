using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IController
{
    public void GetHit(float damage, Vector2 direction);
}
