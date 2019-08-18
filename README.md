# Procedural Cities

A procedural house/city generator as described in the [Build a System that Generates Houses & Castles](https://www.udemy.com/course/unity-5-build-a-system-that-generates-houses-castles-auto) Udemy course.
This project also uses assets from the [Low Poly Nature Pack](https://free3d.com/3d-model/free-low-poly-nature-pack-16603.html).

So far, the following is implemented:

- 3D grid sampling for starting positions,
- Disconnected component suppression to encourage "bigger" houses
- Randomized prefabs instantiation for
  - Regular walls,
  - Walls with doors on the ground floor, and
  - Rooftops,
  - Windows with balconies and flowers,
  - Outdoor items (rocks, trees, flower patches),
- Shared wall removal.

Smaller villages at a 25% chance of
clustering horizontally and a 25% chance
of clustering vertically:

![](.readme/added-outdoor-assets.jpg)

Larger clusters of blocks at a 62% chance of clustering horizontally and a 11% chance of
clustering vertically:

![](.readme/bigger-clusters.jpg)

A view from the inside:

![](.readme/roofs-and-doors-interior.jpg)
