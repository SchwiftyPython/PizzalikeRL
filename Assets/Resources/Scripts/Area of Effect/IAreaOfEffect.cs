using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public interface IAreaOfEffect
{
    /// <summary>
    /// After an AOE has been constructed, it may need to have the affected area shifted over to a different position
    /// without changing any other properties of the AOE. Some AOE implementations may have an origin where the AOE
    /// starts emanating from, but the origin will not be affected by this method; instead the cell specified by target
    /// must be enough on its own to select a different target area without the producer of the AOE needing to move.
    /// </summary>
    /// <param name="point">Used to change the location of the AOE without its producer needing to move</param>
    void Shift(Vector2 point);

    /// <summary>
    /// Given a Set of Points that the producer of the AOE wants to include in the region of this AOE, this method does
    /// a quick approximation to see if there is any possibility that the AOE as currently configured might include one
    /// of those Points within itself.It does not do a full, detailed scan, nor does it count how many opponents might
    /// be included. It does not check the map to verify that there is any sort of path to a target.It is recommended
    /// that the Set of Points consist only of enemies that are within FOV, which cuts down a lot on the amount of checks
    /// this needs to make; if the game doesn't restrict the player's FOV, this is still recommended(just with a larger
    /// FOV radius) because it prevents checking enemies on the other side of the map and through multiple walls.
    /// </summary>
    /// <param name="targets">A Collection (usually a Set) of Points that are desirable targets to include in this AOE</param>
    /// <returns>True if there could be at least one target within the AOE, false otherwise. Very approximate.</returns>
    bool MayContainTarget(List<Vector2> targets);

    /// <summary>
    /// Returns a OrderedDictionary of Vector2 keys and List of Vector2 values, where each Vector2 key is an ideal location to
    /// hit as many of the Points in targets as possible without hitting any Points in requiredExclusions, and each value
    /// is the collection of targets that will be hit if the associated key is used.The length of any List in the
    /// returned collection's values will be the number of targets likely to be affected by the AOE when shift() is
    /// called with the Vector2 key as an argument; all of the List should have the same length.The second argument
    /// may be null, in which case this will initialize it to an empty Set of Vector2 and disregard it.
    /// With complex maps and varied arrangements of obstacles and desirable targets, calculating the best points to
    /// evaluate for AI can be computationally difficult.This method provides a way to calculate with good accuracy
    /// the best Points to pass to shift(Vector2) before calling findArea(). For "blackened thrash industrial death metal"
    /// levels of brutality for the AI, the results of this can be used verbatim, but for more reasonable AI levels, you
    /// can intentionally alter the best options to simulate imperfect aim or environmental variance on the AOE.
    /// Beast-like creatures that do not need devious AI should probably not use this method at all and instead use
    /// shift(Vector2) with the location of some enemy (probably the closest) as its argument.
    /// </summary>
    /// <param name="targets">a Set of Points that are desirable targets to include in this AOE</param>
    /// <param name="requiredExclusions">a Set of Points that this tries strongly to avoid including in this AOE</param>
    /// <returns>a OrderedDictionary of Vector2 keys and List of Vector2 values where keys are ideal locations and values are the target points that will be hit when that key is used.</returns>
    OrderedDictionary IdealLocations(List<Vector2> targets, List<Vector2> requiredExclusions);

    /// <summary>
    ///  A variant of idealLocations that takes two groups of desirable targets, and will rate locations by how many
    /// priorityTargets are in the AOE, then by how many lesserTargets are in the AOE, and will only consider locations
    /// that do not affect a Vector2 in requiredExclusions.Unlike the variant of idealLocations that only takes one group
    /// of targets, this variant can return a collection with ArrayList values where the same Vector2 appears four times
    /// in the same List; this is done only for priorityTargets that are affected by the target Vector2 at the
    /// associated key, and is done so that the length of each similar-quality List should be identical(since a
    /// priorityTarget is worth four times what a lesserTarget is worth in the calculation this uses).
    /// </summary>
    /// <param name="priorityTargets">A Set of Points that are the most-wanted targets to include in this AOE</param>
    /// <param name="lesserTargets">A Set of Points that are the less-wanted targets to include in this AOE, should not overlap with priorityTargets</param>
    /// <param name="requiredExclusions">a Set of Points that this tries strongly to avoid including in this AOE</param>
    /// <returns>a OrderedDictionary of Vector2 keys and List of Vector2 values where keys are ideal locations and values are the target points that will be hit when that key is used.</returns>
    OrderedDictionary IdealLocations(List<Vector2> priorityTargets, List<Vector2> lesserTargets, List<Vector2> requiredExclusions);

    /// <summary>
    /// This must be called before any other methods, and takes a Tile[,]. 
    /// </summary>
    /// <param name="areaMap">map height first, width second, 2D Tile array.</param>
    void SetAreaMap(Tile[,] areaMap);

    /// <summary>
    /// This returns an OrderedDictionary of Vector2 keys to float values; if a cell is 100% affected by the AOE then the value
    /// should be 1.0; if it is 50% affected it should be 0.5, if unaffected should be 0.0, etc.The Vector2 keys should
    /// have the same x and y as the x, y map positions they correspond to.
    /// </summary>
    /// <returns>a OrderedDictionary of Vector2 keys to float values from 1.0 (fully affected) to 0.0 (unaffected)</returns>
    OrderedDictionary FindArea();

    /// <summary>
    /// This returns an OrderedDictionary of Vector2 keys to float values; if a cell is 100% affected by the AOE then the value
    /// should be 1.0; if it is 50% affected it should be 0.5, if unaffected should be 0.0, etc.The Vector2 keys should
    /// have the same x and y as the x, y map positions they correspond to.
    /// </summary>
    /// <returns>a OrderedDictionary of Vector2 keys to float values from 1.0 (fully affected) to 0.0 (unaffected)</returns>
    Vector2 GetOrigin();

    /// <summary>
    /// Set the position from which the AOE originates, which may be related to the location of the AOE's effect, as for
    /// lines, cones, and other emitted effects, or may be unrelated except for determining which enemies can be seen
    /// or targeted from a given origin point(as for distant effects that radiate from a chosen central point, but
    /// have a maxRange at which they can deliver that effect).
    /// </summary>
    /// <param name="origin">The position from which the AOE originates</param>
    void SetOrigin(Vector2 origin);

    /// <summary>
    /// Gets the AimLimit enum that can be used to restrict points this checks (defaults to null if not set).
    /// You can use limitType to restrict any Points that might be processed based on the given origin(which will be
    /// used as the geometric origin for any calculations this makes) with AimLimit values having the following meanings:
    /// AimLimit.FREE makes no restrictions; it is equivalent here to passing null for limit.
    /// AimLimit.EIGHT_WAY will only consider Points to be valid targets if they are along a straight line with an angle
    /// that is a multiple of 45 degrees, relative to the positive x
    /// axis.Essentially, this limits the points to those a queen could move to in chess.
    /// AimLimit.ORTHOGONAL will cause the AOE to only consider Points to be valid targets if they are along a straight
    /// line with an angle that is a multiple of 90 degrees, relative to the positive x
    /// axis. Essentially, this limits the points to those a rook could move to in chess.
    /// AimLimit.DIAGONAL will cause the AOE to only consider Points to be valid targets if they are along a straight
    /// line with an angle that is 45 degrees greater than a multiple of 90 degrees, relative to the
    /// positive x axis. Essentially, this limits the points to those a bishop could move to in chess.
    /// null will cause the AOE to consider all points.
    /// </summary>
    /// <returns>The AimLimit enum that can be used to restrict points this checks (defaults to null if not set)</returns>
    AimLimit GetLimitType();

    /// <summary>
    /// Sets the AimLimit enum that can be used to restrict points this checks (defaults to null if not set).
    /// You can use limitType to restrict any Points that might be processed based on the given origin(which will be
    /// used as the geometric origin for any calculations this makes) with AimLimit values having the following meanings:
    /// AimLimit.FREE makes no restrictions; it is equivalent here to passing null for limit.
    /// AimLimit.EIGHT_WAY will only consider Points to be valid targets if they are along a straight line with an angle
    /// that is a multiple of 45 degrees, relative to the positive x
    /// axis.Essentially, this limits the points to those a queen could move to in chess.
    /// AimLimit.ORTHOGONAL will cause the AOE to only consider Points to be valid targets if they are along a straight
    /// line with an angle that is a multiple of 90 degrees, relative to the positive x
    /// axis. Essentially, this limits the points to those a rook could move to in chess.
    /// AimLimit.DIAGONAL will cause the AOE to only consider Points to be valid targets if they are along a straight
    /// line with an angle that is 45 degrees greater than a multiple of 90 degrees, relative to the
    /// positive x axis. Essentially, this limits the points to those a bishop could move to in chess.
    /// null will cause the AOE to consider all points.
    /// </summary>
    /// <param name="limitType">An AimLimit enum that can be used to restrict points this checks (defaults to null if not set)</param>
    void SetLimitType(AimLimit limitType);

    /// <summary>
    /// The minimum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.
    /// </summary>
    /// <returns>The minimum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.</returns>
    int GetMinRange();

    /// <summary>
    /// The maximum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.
    /// </summary>
    /// <returns>The maximum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.</returns>
    int GetMaxRange();

    /// <summary>
    /// The minimum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.
    /// </summary>
    void SetMinRange(int minRange);

    /// <summary>
    /// The maximum inclusive range that the AOE can be shift()-ed to using the distance measurement from radiusType.
    /// </summary>
    void SetMaxRange(int maxRange);
}
