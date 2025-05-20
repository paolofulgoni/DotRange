using System;

namespace SharpInterval;

/// <summary>
/// Indicates whether an endpoint of some interval is contained in the interval itself ("closed") or not
/// ("open"). If an interval is unbounded on a side, it is neither open nor closed on that side; the
/// bound simply does not exist.
/// </summary>
[Serializable]
public enum BoundType
{
    Open,
    Closed
}
