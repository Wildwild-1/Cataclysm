using System.Diagnostics.CodeAnalysis;
using Content.Server._Cataclysm.Storage.Components;
using Content.Server._Cataclysm.Storage.Events;
using Content.Server._Cataclysm.Storage.NodeGroups;
using Content.Server.NodeContainer;
using Content.Server.Power.EntitySystems;

namespace Content.Server._Cataclysm.Storage.EntitySystems;

public sealed partial class StorageSystem : EntitySystem
{
    [Dependency] private readonly PowerReceiverSystem _powerReceiverSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<StorageControllerComponent, StorageNetLoadNodeEvent>(OnStorageControllerLoadNode);
        SubscribeLocalEvent<StorageControllerComponent, StorageNetRemoveNodeEvent>(OnStorageControllerRemoveNode);
    }

    public void OnStorageControllerLoadNode(EntityUid uid, StorageControllerComponent comp, StorageNetLoadNodeEvent args)
    {
        args.Net.Controllers.Add(uid);
    }

    public void OnStorageControllerRemoveNode(EntityUid uid, StorageControllerComponent comp, StorageNetRemoveNodeEvent args)
    {
        args.Net.Controllers.Remove(uid);
    }

    public void RaiseStorageNetLoadNodeEvent(EntityUid uid, StorageNetLoadNodeEvent args)
    {
        RaiseLocalEvent(uid, args);
    }

    public void RaiseStorageNetRemoveNodeEvent(EntityUid uid, StorageNetRemoveNodeEvent args)
    {
        RaiseLocalEvent(uid, args);
    }

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

    public bool HasPoweredController(StorageNet net)
    {
        foreach (var controller in net.Controllers)
            if (_powerReceiverSystem.IsPowered(controller))
                return true;

        return false;
    }

    public bool IsConnectedToPoweredController(EntityUid uid)
    {
        if (!TryGetStorageNet(uid, out var net))
            return false;
        if (!HasPoweredController(net))
            return false;

        return true;
    }
}