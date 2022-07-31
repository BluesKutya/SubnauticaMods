using System;
using UnityEngine;

namespace BuildInSeatruck
{
    class SeaTruckSegmentHelper
    {
        public static bool isPlayerInSeaTruckSegment()
        {
            return Player.main.currentInterior as SeaTruckSegment != null;
        }

        public static SeaTruckSegment getCurrentSeaTruckSegment()
        {
            return Player.main.currentInterior as SeaTruckSegment;
        }

        public static SeaTruckSegment getParentSeaTruckSegment(GameObject gameObject)
        {
            return gameObject.GetComponentInParent<SeaTruckSegment>();
        }

        public static SubRoot getCurrentSeaTruckSegmentSubRoot()
        {
            SeaTruckSegment sts = getCurrentSeaTruckSegment();
            if (sts == null)
                return null;

            return sts.gameObject.GetComponent<SubRoot>();
        }

    }
}
