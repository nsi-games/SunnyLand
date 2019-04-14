using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    public delegate void EventCallback();
    public delegate void FloatCallback(float value);
    public delegate void BoolCallback(bool value);
    public delegate void RaycastHit2DCallback(RaycastHit2D value);
}