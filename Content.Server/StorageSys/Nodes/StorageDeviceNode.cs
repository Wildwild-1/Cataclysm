using Content.Server.NodeContainer;
using Content.Server.NodeContainer.Nodes;
using Robust.Server.GameObjects;
using Robust.Shared.Map.Components;

namespace Content.Server.StorageSys.Nodes;

[DataDefinition]
public sealed partial class StorageDeviceNode : Node
{
    public override IEnumerable<Node> GetReachableNodes(TransformComponent xform, EntityQuery<NodeContainerComponent> nodeQuery, EntityQuery<TransformComponent> xformQuery, MapGridComponent? grid, IEntityManager entMan)
    {
        if (!xform.Anchored || grid == null || xform.GridUid == null)
            yield break;

        var mapSystem = entMan.System<MapSystem>();

        var gridUid = xform.GridUid.Value;
        var gridIndex = mapSystem.TileIndicesFor(gridUid, grid, xform.Coordinates);

        foreach (var node in NodeHelpers.GetNodesInTile(nodeQuery, grid, gridIndex))
        {
            if (node is StorageDuctNode)
                yield return node;
        }
    }
}