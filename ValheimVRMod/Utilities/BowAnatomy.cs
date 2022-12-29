using System;
using System.Collections.Generic;
using UnityEngine;

/**
 * Manages hardcoded bow anatomy data.
 */
namespace ValheimVRMod.Utilities
{
    public class BowAnatomy
    {
        public readonly Vector3 stringTop;
        public readonly Vector3 stringBottom;
        public readonly Vector3 handleTop;
        public readonly Vector3 handleBottom;
        public readonly float handleWidth;
        public readonly float handleHeight;
        public readonly float softLimbHeight;
        public readonly float stringRadius;

        private static BowAnatomy DefaultBowAnatomy = new BowAnatomy(
            /* stringTop= */ new Vector3(0, 0.75f, 0.325f),
            /* stringBottom= */ new Vector3(0, -0.75f, 0.325f),
            /* handleTop= */ new Vector3(0, 0.312f, 0),
            /* handleBottom= */ new Vector3(0, -0.312f, 0),
            /* handleWidth= */ 0.05f,
            /* handleHeight= */ 0.624f,
            /* softLimbHeight= */ 0.125f,
            /* stringRadius= */ 0.008f);

        private static Dictionary<string, BowAnatomy> BowAnatomies = new Dictionary<string, BowAnatomy>
        {
            {
                "$item_bow_snipesnap", // Note: item name is snipesnap not spinesnap
                new BowAnatomy(
                    /* stringTop= */ new Vector3(0, 0.75f, 0.325f),
                    /* stringBottom= */ new Vector3(0, -0.75f, 0.325f),
                    /* handleTop= */ new Vector3(0, 0.23f, 0),
                    /* handleBottom= */ new Vector3(0, -0.23f, 0),
                    /* handleWidth= */ 0.05f,
                    /* handleHeight= */ 0.46f,
                    /* softLimbHeight= */ 0.1f,
                    /* stringRadius= */ 0.008f)
            }
        };

        public static BowAnatomy getBowAnatomy(string bowName)
        {
            if (BowAnatomies.ContainsKey(bowName))
            {
                return BowAnatomies[bowName];
            }
            else
            {
                return DefaultBowAnatomy;
            }
        }

        protected BowAnatomy(
            Vector3 stringTop,
            Vector3 stringBottom,
            Vector3 handleTop,
            Vector3 handleBottom,
            float handleWidth,
            float handleHeight,
            float softLimbHeight,
            float stringRadius)
        {
            this.stringTop = stringTop;
            this.stringBottom = stringBottom;
            this.handleTop = handleTop;
            this.handleBottom = handleBottom;
            this.handleWidth = handleWidth;
            this.handleHeight = handleHeight;
            this.softLimbHeight = softLimbHeight;
            this.stringRadius = stringRadius;
        }
    }
}
