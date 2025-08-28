using System.Collections.Generic;
using UnityEngine;
namespace UnityColliderOptimizer.Core.P
{
    public interface IPolySimpStrat
    {
        List<List<Vector2>> Simplify(List<List<Vector2>> __src, PolyOptParams __p, Transform __t = null);
    }
}