using Content.Server.StorageSys.NodeGroups;
using Content.Server.NodeContainer.Nodes;

namespace Content.Server.StorageSys.Events;

public sealed class StorageNetLoadNodeEvent : EventArgs
{
    public Node Node { get; }
    public StorageNet Net { get; }

    public StorageNetLoadNodeEvent(Node node, StorageNet net)
    {
        Node = node;
        Net = net;
    }
}

public sealed class StorageNetRemoveNodeEvent : EventArgs
{
    public Node Node { get; }
    public StorageNet Net { get; }

    public StorageNetRemoveNodeEvent(Node node, StorageNet net)
    {
        Node = node;
        Net = net;
    }
}