using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IListenerBlock
{
    int IType();
    void ISkin(Material material, int index);
}
