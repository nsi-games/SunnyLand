using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SunnyLand
{
    public class OpossumController : Enemy
    {
        public override void PerformMove()
        {
            Vector3 right = Vector3.Cross(groundNormal, Vector3.forward);
            rigid.AddForce(right * horizontal * speed);
            LimitVelocity();
        }
    }
}