using System;

namespace DotRange;

/// <summary>
/// Indicates whether an endpoint of some range is contained in the range itself ("closed") or not
/// ("open"). If a range is unbounded on a side, it is neither open nor closed on that side; the
/// bound simply does not exist.
/// </summary>
[Serializable]
public enum BoundType
{
    Open,
    Closed
}
