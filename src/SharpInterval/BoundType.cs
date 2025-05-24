using System;

namespace SharpInterval;

/// <summary>
/// Specifies whether an interval's endpoint is included in the interval ('Closed') or excluded ('Open').
/// For intervals that are unbounded on a side (extending to infinity), that side doesn't have a
/// conventional open or closed bound.
/// </summary>
[Serializable]
public enum BoundType
{
    Open,
    Closed
}
