using Content.Shared.Whitelist;

namespace Content.Server.StorageSys.Components;

[RegisterComponent]
public sealed partial class StorageContainerComponent : Component
{
    [DataField]
    public int Capacity = 1000;

    [DataField]
    public EntityWhitelist Whitelist;
}