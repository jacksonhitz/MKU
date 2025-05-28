using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHitable
{
    void Hit(float dmg);
}

public interface IInteractable
{
    void Interact();
}
