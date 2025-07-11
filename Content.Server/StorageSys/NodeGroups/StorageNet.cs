using Content.Server.StorageSys.EntitySystems;
using Content.Server.StorageSys.Nodes;
using Content.Server.NodeContainer.NodeGroups;
using Content.Server.NodeContainer.Nodes;
using Content.Server.StorageSys.Data;

namespace Content.Server.StorageSys.NodeGroups;

[NodeGroup(NodeGroupID.Storage)]
public sealed partial class StorageNet : BaseNodeGroup
{
    private StorageNetSystem _storageSystem = default!;

    public readonly List<EntityUid> Controllers = [];

    public StorageControllerData? ControllerData;

    public override void Initialize(Node sourceNode, IEntityManager entMan)
    {
        base.Initialize(sourceNode, entMan);

        _storageSystem = entMan.System<StorageNetSystem>();
    }

    public override void LoadNodes(List<Node> groupNodes)
    {
        base.LoadNodes(groupNodes);

        foreach (var node in groupNodes)
            if (node is not IStoragePassiveNode)
                _storageSystem.RaiseStorageNetLoadNodeEvent(node.Owner, new(node, this));
    }

    public override void RemoveNode(Node node)
    {
        if (node is not IStoragePassiveNode)
            _storageSystem.RaiseStorageNetRemoveNodeEvent(node.Owner, new(node, this));
    }
}