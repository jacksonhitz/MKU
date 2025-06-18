using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void Hit(float dmg);
}

public interface IThrowable
{
    void Thrown();
}

public interface IInteractable
{
    void Interact();
}
