# Unity Collider Optimizer

Optimizes Mesh & Polygon colliders in Unity.

## Polygon Collider Optimizer

| ![PCO_V1](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/62a658c3-487c-4d80-b303-b96a74804a99) | ![PCO_V2](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/cc114920-5bcd-4f06-81ad-9df9a176055d) | ![PCO_V3](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/dfd8c63e-11ed-4a74-a2e1-45e2829f017e) |
|:---:|:---:|:---:|
| Original Sprite | Unity Polygon Collider | Optimized Polygon Collider |
|**Path Count**| *213 paths* | *23 paths* |


The tool uses a [C# implementation](https://www.codeproject.com/Articles/18936/A-C-Implementation-of-Douglas-Peucker-Line-Appro) of the Ramer Douglas Peucker Algorithm to smooth the polylines and reduce the number of paths created by a Polygon Collider in Unity.

#### Performance Comparison

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/e134f0c6-4c08-4552-b69f-22e7f3b61ebe

You can run this test on your machine by cloning the project and going to the `Polygon Collider Optimization Test` scene in Unity.

#### Usage
* Download the `collideroptimizationpackage_2d_v004.unitypackage` package from the [Releases](https://github.com/aniketrajnish/Unity-Collider-Optimizer/releases/).
* Import all the assets from the package in your unity project.
* Attach the `PolygonColliderOptimizer.cs` script on your 2D sprite.
* Adjust the `optimizationFactor` to control the amount of optimzation you need. 

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/74f5bdd1-f5c8-4c44-b745-78211919aae3

## Mesh Collider Optimizer 

| ![MCO_V1](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/e23b8db9-c301-41b1-8ef0-31b2216057d6) | ![MCO_V2](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/7a88b61c-2c35-40df-a181-23e7d0d7c05c) | ![MCO_V3](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/06a5fb01-3c08-4a1c-bb82-2b30a534693e) |
|:---:|:---:|:---:|
| Original Mesh | Unity Mesh Collider | Optimized Mesh Collider |
|**Triangles Count**| *9132 tris* | *2416 tris* |


The tool uses the [Computational Geometry Unity Library](https://github.com/Habrador/Computational-geometry) by Erik Nordeus 🐐 to perfrom the Quadric Error Metrics simplification on the shared mesh of the mesh collider.

#### Performance Comparison

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/cfba7837-a81e-4891-9594-501fbf31680f

You can run this test on your machine by cloning the project and going to the `Mesh Collider Optimization Test` scene in Unity.

#### Usage
* Download the `collideroptimizationpackage_3d_v004.unitypackage` package from the [Releases](https://github.com/aniketrajnish/Unity-Collider-Optimizer/releases/).
* Import all the assets from the package in your unity project.
* Attach the `MeshColliderOptimizer.cs` script on your 2D sprite.
* Choose the connecting mode between _Fast, Precise, and No._
* Adjust the `optimizationFactor` to control the amount of optimzation you need.
* Choose the mesh style to be _Soft, Hard or both_.
* **Warning: Choosing the "Fast" mode would likely lead to errors, Precise mode is recommended for most of the meshes.** 

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/ddb08b48-d241-494c-a6f4-0a822424964a

## Updates
#### v004
* Added the functionality to save and load the optimized colliders as assets.
* Fixed the refresh bug.
  
https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/ba42c61c-4ea1-419d-bf81-324304a218b8

## Contribution
Contributions to the project are welcome. Currently working on converting the QEM algorithm to a coroutine to stop the main Unity thread from freezing.

## Known Bugs
* ~~If a prefab is made out of a gameobject having Polygon Collider Optimizer, it keeps refreshing itself.~~ **[FIXED]**
* Choosing Hard Edge Mesh Style decreases the number of triangles but induces additional vertices (doesn't affect the performance).

## License
MIT License
