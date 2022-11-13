using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltraMan
{
    public static class LayerMasks
    {
        public static LayerMask Stage => LayerMask.GetMask("Stage");

        public static LayerMask Entity => LayerMask.GetMask("Entity");
    }
}

