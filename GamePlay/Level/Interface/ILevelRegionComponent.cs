using UnityEngine;
using System.Collections;

namespace LevelDirector
{
    public interface ILevelRegionComponent
    {
        int         RegionID { get; }
        LevelRegion Region   { get; set; }
    }

}
