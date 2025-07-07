using System.Numerics;
using Content.Shared.Power.EntitySystems;
using Content.Shared._Cataclysm.StorageMarket.Components;
using Robust.Shared.Serialization;

namespace Content.Shared._Cataclysm.StorageMarket.EntitySystems;

public sealed partial class StorageMarketSystem : EntitySystem
{
    [Dependency] private readonly SharedPowerReceiverSystem _powerReceiverSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
    }

    public IEnumerable<(EntityUid uid, StorageMarketComputerComponent computer, TransformComponent transform)> GetActiveComputers()
    {
        var query = EntityQueryEnumerator<StorageMarketComputerComponent, TransformComponent>();

        while (query.MoveNext(out var uid, out var computer, out var transform))
        {
            if (!transform.Anchored || !_powerReceiverSystem.IsPowered(uid))
                continue;

            yield return (uid, computer, transform);
        }
    }

    public IEnumerable<(EntityUid uid, StorageMarketPalletComponent pallet, TransformComponent transform)> GetConnectedPallets(EntityUid computerUid, StorageMarketComputerComponent computer, TransformComponent computerTransform)
    {
        if (!computerTransform.Anchored || !_powerReceiverSystem.IsPowered(computerUid))
            yield break;

        var query = EntityQueryEnumerator<StorageMarketPalletComponent, TransformComponent>();

        while (query.MoveNext(out var palletUid, out var pallet, out var palletTransform))
        {
            if (!palletTransform.Anchored || computerTransform.GridUid != palletTransform.GridUid)
                continue;

            if (Vector2.Distance(palletTransform.LocalPosition, computerTransform.LocalPosition) > computer.MaxPalletDistance)
                continue;

            yield return (palletUid, pallet, palletTransform);
        }
    }
}

[NetSerializable, Serializable]
public enum StorageMarketComputerUiKey : byte
{
    Default
}