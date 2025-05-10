using Robust.Shared.Map;
using Robust.Shared.Physics.Systems;
using System.Numerics;
using Robust.Shared.Timing;
using Content.Shared.Weapons.Ranged;

namespace Content.Server._Mono.Radar;

/// <summary>
/// System that handles radar visualization for hitscan projectiles
/// </summary>
public sealed partial class HitscanRadarSystem : EntitySystem
{
    [Dependency] private readonly IMapManager _mapManager = default!;
    [Dependency] private readonly RadarBlipSystem _radarBlipSystem = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly SharedPhysicsSystem _physics = default!;
    [Dependency] private readonly IGameTiming _timing = default!;

    // Dictionary to track entities that should be deleted after a specific time
    private readonly Dictionary<EntityUid, TimeSpan> _pendingDeletions = new();

    /// <summary>
    /// Event raised before firing the effects for a hitscan projectile.
    /// </summary>
    public sealed class HitscanFireEffectEvent : EntityEventArgs
    {
        public EntityCoordinates FromCoordinates { get; }
        public float Distance { get; }
        public Angle Angle { get; }
        public HitscanPrototype Hitscan { get; }
        public EntityUid? HitEntity { get; }
        public EntityUid? Shooter { get; }

        public HitscanFireEffectEvent(EntityCoordinates fromCoordinates, float distance, Angle angle, HitscanPrototype hitscan, EntityUid? hitEntity = null, EntityUid? shooter = null)
        {
            FromCoordinates = fromCoordinates;
            Distance = distance;
            Angle = angle;
            Hitscan = hitscan;
            HitEntity = hitEntity;
            Shooter = shooter;
        }
    }

    public override void Initialize()
    {
        base.Initialize();

        // Subscribe to our custom hitscan fire effect event
        SubscribeLocalEvent<HitscanFireEffectEvent>(OnHitscanEffect);

        // Handle component lifecycle
        SubscribeLocalEvent<HitscanRadarComponent, ComponentShutdown>(OnHitscanRadarShutdown);
    }

    private void OnHitscanEffect(HitscanFireEffectEvent ev)
    {
        if (ev.Shooter == null)
            return;

        // Create a new entity for the hitscan radar visualization
        // Use the shooter's position to spawn the entity
        var shooterCoords = new EntityCoordinates(ev.Shooter.Value, Vector2.Zero);
        var uid = Spawn(null, shooterCoords);

        // Add the radar blip component so it shows up on radar
        var blip = EnsureComp<RadarBlipComponent>(uid);
        blip.RequireNoGrid = true;
        blip.VisibleFromOtherGrids = true;
        blip.RadarColor = Color.Transparent; // Make the point invisible
        blip.Enabled = true;

        // Add the hitscan radar component
        var hitscanRadar = EnsureComp<HitscanRadarComponent>(uid);

        // Determine start position using proper coordinate transformation
        var startPos = _transform.ToMapCoordinates(ev.FromCoordinates).Position;

        // Compute end position in map space (world coordinates)
        var dir = ev.Angle.ToVec().Normalized();
        var endPos = startPos + dir * ev.Distance;

        // Set the origin grid if available
        hitscanRadar.OriginGrid = Transform(ev.Shooter.Value).GridUid;

        // Set the start and end coordinates
        hitscanRadar.StartPosition = startPos;
        hitscanRadar.EndPosition = endPos;

        // If the hitscan has a travel flash, use its color for radar
        if (ev.Hitscan.TravelFlash != null)
        {
            hitscanRadar.RadarColor = Color.Magenta; // Default color for hitscan beams
        }
        else
        {
            // Fallback color
            hitscanRadar.RadarColor = Color.Red;
        }

        // Schedule entity for deletion after its lifetime expires
        var deleteTime = _timing.CurTime + TimeSpan.FromSeconds(hitscanRadar.LifeTime);
        _pendingDeletions[uid] = deleteTime;
    }

    private void OnHitscanRadarShutdown(Entity<HitscanRadarComponent> ent, ref ComponentShutdown args)
    {
        // Ensure the entity is properly deleted when the component is removed
        QueueDel(ent);

        // Remove from pending deletions if it's there
        _pendingDeletions.Remove(ent);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        // Handle pending deletions
        if (_pendingDeletions.Count > 0)
        {
            var currentTime = _timing.CurTime;
            var toRemove = new List<EntityUid>();

            foreach (var (entity, deleteTime) in _pendingDeletions)
            {
                if (currentTime >= deleteTime)
                {
                    if (!Deleted(entity))
                        QueueDel(entity);
                    toRemove.Add(entity);
                }
            }

            foreach (var entity in toRemove)
            {
                _pendingDeletions.Remove(entity);
            }
        }
    }
}
