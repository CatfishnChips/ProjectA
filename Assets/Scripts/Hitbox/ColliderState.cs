[System.Serializable]
public enum ColliderState
{
    Closed,
    Open,
    Colliding
}

[System.Serializable]
[System.Flags]
public enum ColliderProperty
{
    Static = 1 << 0,
    Priority = 1 << 1,
}