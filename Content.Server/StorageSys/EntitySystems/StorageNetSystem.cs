using System.Diagnostics.CodeAnalysis;
using Content.Server.StorageSys.Components;
using Content.Server.StorageSys.Events;
using Content.Server.StorageSys.NodeGroups;
using Content.Server.NodeContainer;
using Content.Server.Power.EntitySystems;
using Robust.Server.Containers;
using Robust.Shared.Containers;
using Content.Shared.StorageSys.Components;
using Content.Shared.Power;

namespace Content.Server.StorageSys.EntitySystems;

public sealed partial class StorageNetSystem : EntitySystem
{
    [Dependency] private readonly PowerReceiverSystem _powerReceiverSystem = default!;
    [Dependency] private readonly ContainerSystem _containerSystem = default!;
    [Dependency] private readonly SharedAppearanceSystem _sharedAppearanceSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StorageControllerComponent, ComponentStartup>(OnStorageControllerStartup);

        SubscribeLocalEvent<StorageControllerComponent, StorageNetLoadNodeEvent>(OnStorageControllerLoadNode);
        SubscribeLocalEvent<StorageControllerComponent, StorageNetRemoveNodeEvent>(OnStorageControllerRemoveNode);

        SubscribeLocalEvent<StorageControllerComponent, EntInsertedIntoContainerMessage>(OnStorageControllerContainerInsert);
        SubscribeLocalEvent<StorageControllerComponent, EntRemovedFromContainerMessage>(OnStorageControllerContainerRemove);

        SubscribeLocalEvent<StorageControllerComponent, PowerChangedEvent>(OnStorageControllerPowerChanged);
    }

    public void OnStorageControllerStartup(EntityUid uid, StorageControllerComponent comp, ComponentStartup args)
    {
        SpawnInContainerOrDrop(comp.DriveSlotPrototype, uid, StorageControllerComponent.DriveSlotName);
    }

    public void OnStorageControllerLoadNode(EntityUid uid, StorageControllerComponent comp, StorageNetLoadNodeEvent args)
    {
        args.Net.Controllers.Add(uid);
        TryConnectControllerDrive(uid);
    }

    public void OnStorageControllerRemoveNode(EntityUid uid, StorageControllerComponent comp, StorageNetRemoveNodeEvent args)
    {
        args.Net.Controllers.Remove(uid);
        TryDisconnectControllerDrive(uid);
    }

    public void OnStorageControllerContainerInsert(EntityUid uid, StorageControllerComponent comp, ContainerModifiedMessage args)
    {
        if (args.Container.ID != StorageControllerComponent.DriveSlotName)
            return;
        if (!TryComp<StorageControllerDriveComponent>(args.Entity, out _))
            return;

        _sharedAppearanceSystem.SetData(uid, StorageControllerVisuals.Drive, true);

        TryConnectControllerDrive(uid);
    }

    public void OnStorageControllerContainerRemove(EntityUid uid, StorageControllerComponent comp, ContainerModifiedMessage args)
    {
        if (args.Container.ID != StorageControllerComponent.DriveSlotName)
            return;
        if (!TryComp<StorageControllerDriveComponent>(args.Entity, out var drive))
            return;

        _sharedAppearanceSystem.SetData(uid, StorageControllerVisuals.Drive, false);

        if (!TryGetStorageNet(uid, out var net))
            return;

        // Uses the lower-level disconnect method since the drive is no longer in the controller.
        // This means TryDisconnectControllerDrive(uid) would fail to find the drive.
        TryDisconnectControllerDrive(net, drive);
    }

    public void OnStorageControllerPowerChanged(EntityUid uid, StorageControllerComponent comp, PowerChangedEvent args)
    {
        if (args.Powered)
            TryConnectControllerDrive(uid);
        else
            TryDisconnectControllerDrive(uid);
    }

    /// <summary>
    /// Wrapper for 'RaiseLocalEvent(uid, args)'
    /// </summary>
    public void RaiseStorageNetLoadNodeEvent(EntityUid uid, StorageNetLoadNodeEvent args)
    {
        RaiseLocalEvent(uid, args);
    }

    /// <summary>
    /// Wrapper for 'RaiseLocalEvent(uid, args)'
    /// </summary>
    public void RaiseStorageNetRemoveNodeEvent(EntityUid uid, StorageNetRemoveNodeEvent args)
    {
        RaiseLocalEvent(uid, args);
    }

    /// <summary>
    /// Tries to connect a StorageControllerDriveComponent in the entity to its StorageNet.
    /// The connection attempt will fail if the entity is not powered.
    /// </summary>
    public void TryConnectControllerDrive(EntityUid uid)
    {
        if (!_powerReceiverSystem.IsPowered(uid))
            return;
        if (!TryGetStorageNet(uid, out var net))
            return;
        if (!TryGetControllerDrive(uid, out var drive))
            return;

        TryConnectControllerDrive(net, drive);
    }

    /// <summary>
    /// Tries to disconnect a StorageControllerDriveComponent in the entity from its StorageNet.
    /// </summary>
    public void TryDisconnectControllerDrive(EntityUid uid)
    {
        if (!TryGetStorageNet(uid, out var net))
            return;
        if (!TryGetControllerDrive(uid, out var drive))
            return;

        TryDisconnectControllerDrive(net, drive);
    }

    public void TryConnectControllerDrive(StorageNet net, StorageControllerDriveComponent drive)
    {
        if (drive.ConnectedNet != null)
            return;

        net.ControllerData ??= drive.Data;
        drive.ConnectedNet = net;
    }

    public void TryDisconnectControllerDrive(StorageNet net, StorageControllerDriveComponent drive)
    {
        if (drive.ConnectedNet != net)
            return;

        if (net.ControllerData != null)
            drive.Data = net.ControllerData.Value;

        drive.ConnectedNet = null;

        foreach (var uid in net.Controllers)
        {
            if (!TryGetControllerDrive(uid, out var otherDrive))
                continue;
            if (otherDrive.ConnectedNet == net)
                return;
        }

        net.ControllerData = null;
    }

    /// <summary>
    /// Tries to get the StorageNet connected to the entity.
    /// </summary>
    public bool TryGetStorageNet(EntityUid uid, [NotNullWhen(true)] out StorageNet? net)
    {
        net = null;

        if (!TryComp<NodeContainerComponent>(uid, out var nodeContainer))
            return false;
        if (!nodeContainer.Nodes.TryGetValue("storage", out var node))
            return false;
        if (node.NodeGroup is not StorageNet)
            return false;

        net = (StorageNet)node.NodeGroup;
        return true;
    }

    /// <summary>
    /// Tries to get the storage controller drive contained inside of the entity.
    /// </summary>
    public bool TryGetControllerDrive(EntityUid uid, [NotNullWhen(true)] out StorageControllerDriveComponent? drive)
    {
        drive = null;

        if (!TryGetControllerDriveSlot(uid, out var driveSlot))
            return false;
        if (driveSlot.ContainedEntity == null)
            return false;

        return TryComp(driveSlot.ContainedEntity, out drive);
    }

    /// <summary>
    /// Tries to get the storage controller drive slot of the entity.
    /// </summary>
    public bool TryGetControllerDriveSlot(EntityUid uid, [NotNullWhen(true)] out ContainerSlot? driveSlot)
    {
        driveSlot = null;

        if (!_containerSystem.TryGetContainer(uid, StorageControllerComponent.DriveSlotName, out var container))
            return false;
        if (container is not ContainerSlot)
            return false;

        driveSlot = (ContainerSlot)container;
        return true;
    }

    /// <summary>
    /// Returns whether the StorageNet is connected to a powered storage controller.
    /// </summary>
    public bool IsConnectedToPoweredController(StorageNet net)
    {
        foreach (var controller in net.Controllers)
            if (_powerReceiverSystem.IsPowered(controller))
                return true;

        return false;
    }

    /// <summary>
    /// Returns whether the entity is connected to a powered storage controller.
    /// </summary>
    public bool IsConnectedToPoweredController(EntityUid uid)
    {
        if (!TryGetStorageNet(uid, out var net))
            return false;
        if (!IsConnectedToPoweredController(net))
            return false;

        return true;
    }
}