using Content.Server.StorageSys.Data;
using Content.Server.StorageSys.NodeGroups;

namespace Content.Server.StorageSys.Components;

[RegisterComponent]
public sealed partial class StorageControllerDriveComponent : Component
{
    public StorageControllerData Data = new();
    public StorageNet? ConnectedNet;
}