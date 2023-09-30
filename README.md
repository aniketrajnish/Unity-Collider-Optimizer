# Unity Collider Optimizer

Optimizes Mesh & Polygon colliders in Unity.

## Polygon Collider Optimizer

| ![Original Sprite](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/7a30f557-e3dd-4a79-82a4-7b28e5196fe3) | ![Unity Polygon Collider](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/b776b4f7-5107-4f81-b2b1-e1a60e514059) | ![Optimized Polygon Collider](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/7c8340f3-3125-4ee3-ae31-689e2a3439c0) |
|:---:|:---:|:---:|
| Original Sprite | Unity Polygon Collider | Optimized Polygon Collider |
|**Path Count**| *213 paths* | *23 paths* |

The tool uses a [C# implementation](https://www.codeproject.com/Articles/18936/A-C-Implementation-of-Douglas-Peucker-Line-Appro) of the Ramer Douglas Peucker Algorithm to smooth the polylines and reduce the number of paths created by a Polygon Collider in Unity.

#### Performance Comparision

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/ce96837d-041b-4492-9304-11d025e0bcfd

You can run this test on your machine by cloning the project and going to the `Polygon Collider Optimization Test` scene in Unity.

#### Usage
* Download the `collideroptimizationpackage_2d_v003.unitypackage` package from the [Releases](https://github.com/aniketrajnish/Unity-Collider-Optimizer/releases/).
* Import all the assets from the package in your unity project.
* Attach the `PolygonColliderOptimizer.cs` scrpit on your 2D sprite.
* Adjust the `optimizationFactor` to control the amount of optimzation you need.
  
https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/7040aeb1-2801-4aac-89c8-3a3412e430f3

## Mesh Collider Optimizer 

| ![MCO_V1](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/d0f1ef6f-fa1c-43e5-9a0e-f2ee0ec9e87a) | ![MCO_V2](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/298ab0b6-cbd4-49a9-80a2-edc42107524a) | ![MCO_V3](https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/e6306124-c612-4c4b-8142-c869858b74c1) |
|:---:|:---:|:---:|
| Original Mesh | Unity Mesh Collider | Optimized Mesh Collider |
|**Triangles Count**| *9132 tris* | *2416 tris* |

The tool uses the [Computational Geometry Unity Library](https://github.com/Habrador/Computational-geometry) by Erik Nordeus 🐐 to perfrom the Quadric Error Metric simplification on the shared mesh of the mesh collider.

#### Performance Comparision

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/e7b7d6dc-dd44-4b29-86e2-c5144aefec35

You can run this test on your machine by cloning the project and going to the `Mesh Collider Optimization Test` scene in Unity.

#### Usage
* Download the `collideroptimizationpackage_3d_v003.unitypackage` package from the [Releases](https://github.com/aniketrajnish/Unity-Collider-Optimizer/releases/).
* Import all the assets from the package in your unity project.
* Attach the `MeshColliderOptimizer.cs` scrpit on your 2D sprite.
* Choose the connecting mode between _Fast, Precise, and No._
* Adjust the `optimizationFactor` to control the amount of optimzation you need.
* Choose the mesh style to be _Soft, Hard or both_.
* **Warning: Choosing the "Fast" mode would likely lead to errors, Precise mode is recommended for most of the meshes.**
  
https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/9b978b70-5907-4417-b542-e2e286db2a9e

## Contribution
Contributions to the project are welcome. Currently working on conevrting the QEM algorithm to a couroutine to stop the main Unity thread from freezing.

## Known Bugs
If a prefab is made out of a gameobject having Polygon Collider Optimizer, it keeps refreshing itself.

## License
MIT License
