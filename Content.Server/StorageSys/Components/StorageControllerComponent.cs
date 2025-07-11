using Robust.Shared.Prototypes;

namespace Content.Server.StorageSys.Components;

[RegisterComponent]
public sealed partial class StorageControllerComponent : Component
{
    public const string DriveSlotName = "storage_controller_drive";

    [DataField]
    public EntProtoId? DriveSlotPrototype;
}