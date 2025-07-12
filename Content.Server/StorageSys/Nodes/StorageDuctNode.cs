using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;

namespace Content.Server.StorageSys.Nodes;

[DataDefinition]
public sealed partial class StorageDuctNode : Node, IStoragePassiveNode
{
    public override IEnumerable<Node> GetReachableNodes(TransformComponent xform, EntityQuery<NodeContainerComponent> nodeQuery, EntityQuery<TransformComponent> xformQuery, MapGridComponent? grid, IEntityManager entMan)
    {
        if (!xform.Anchored || grid == null || xform.GridUid == null)
            yield break;

        var mapSystem = entMan.System<MapSystem>();

        var gridUid = xform.GridUid.Value;
        var gridIndex = mapSystem.TileIndicesFor(gridUid, grid, xform.Coordinates);

        foreach (var (dir, node) in NodeHelpers.GetCardinalNeighborNodes(nodeQuery, grid, gridIndex))
        {
            if (node is StorageDuctNode && node != this)
                yield return node;
            if (node is StorageDeviceNode && dir == Direction.Invalid)
                yield return node;
        }
    }
}