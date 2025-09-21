# Unity Collider Optimizer

This package is meant for:
- `MeshCollider` optimization (3D) via bundled gltfpack binaries
    | ![MCO_V1](https://github.com/user-attachments/assets/a2cb0aa8-543a-437c-9daa-572fa1ec1b18) | ![MCO_V2](https://github.com/user-attachments/assets/8fc2e9a5-cdb0-4513-b391-27f570a95023) | ![MCO_V3](https://github.com/user-attachments/assets/4004e1d4-38e8-49ce-8fac-a95f6e686dcc) |
    |:---:|:---:|:---:|
    | Original Mesh | Unity Mesh Collider | Optimized Mesh Collider |
    |**Triangles Count**| *3032 tris, 2512 verts* | *918 tris, 592 verts* |

    
- `PolygonCollider2D` optimization (2D) using RDP line simplification
    | ![PCO_V1](https://github.com/user-attachments/assets/f5314480-bed7-47fb-88b8-545499765a92) | ![PCO_V2](https://github.com/user-attachments/assets/ed26af25-5e1d-48a8-bc6a-a2f417be3cf6) | ![PCO_V3](https://github.com/user-attachments/assets/a4923535-59be-4896-b629-e83cc42db7d4) |
    |:---:|:---:|:---:|
    | Original Sprite | Unity Polygon Collider | Optimized Polygon Collider |
    |**Path Count**| *214 paths* | *23 paths* |

## Requirements
- Unity 2020.2 or newer recommended
(for `MeshColliderCookingOptions`)
- A glTF/GLB importer package in your project:
  - `com.unity.cloud.gltfast` (glTFast) or
  - `com.unity.formats.glTF` (UnityGLTF)
- Supported OS for the included gltfpack binaries:
  - Windows
  - macOS (Intel & Apple Silicon)
  - Linux
> [!NOTE]
> If you don’t have a GLB importer, the tool can’t re-import the simplified mesh and will log an error telling you to install one

## Usage

#### Install
* Download the `collider-opt-pkg-v005.unitypackage` package from the [Releases](https://github.com/aniketrajnish/Unity-Collider-Optimizer/releases/tag/005) section and import in your Unity project
- In Package Manager, install one of:
  - glTFast (`com.unity.cloud.gltfast`) -> **recommended**
  - UnityGLTF (`com.unity.formats.glTF`)
- (macOS/Linux) If the `gltfpack` binary isn’t executable, the tool will attempt to `chmod +x` automatically

#### Optimize Colliders
- Select a GameObject with a `MeshCollider` (3D) or `PolygonCollider2D` (2D)
- Open context menu by right clicking the component header -> `Optimize Collider`
- If needed you have option to Load/Save/Reset Collider aswell

#### Adjust Params
- To adjust params for the optimizer go to `Tools -> Collider Optimizer`
- **Mesh Optimization Params**: 
  - `Contraction` : higher contraction -> fewer triangles kept
  - `Recalc Normals` : recalculate normals after import
  - `Convex` : sets `MeshCollider.convex = true` (Unity may auto-reduce to ≤255 tris)
  - `Aggressive (-sa)` : simplifies aggressively (more tris reduced)
  - `Permissive (-sp)` : simplifies permissively (less tris reduced)
- **Polygon Optimization Params**:
  - `Tolerance` : maximum perpendicular distance new path is allowed to deviate from the original path (higher tolerance -> fewer lines kept)
  - `Tolerance Mode` :
    - `World` : interpreted in world units
    - `Relative` : interpreted as fraction of per-path bbox diagonal
  - `Scale By Bounds` : In `World` mode, multiply tolerance by each path’s bounds diagonal (useful for differently sized shapes)

#### Presets
- You get the option to define presets for different param sets you'd like to save
- Create presets via `Assets -> Create -> ColliderOptimizer -> Mesh Preset or Poly Preset`
- In `Tools -> Collider Optimizer` window assign these presets to make them active
- Without an active preset the settings are stored in `Project Settings` at `ProjectSettings/ColliderOptimizerSettings.asset`
- `Reset to Defaults` will update the preset (if assigned) or the project settings to the default values

## Performance Comparison

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/e134f0c6-4c08-4552-b69f-22e7f3b61ebe

https://github.com/aniketrajnish/Unity-Collider-Optimizer/assets/58925008/cfba7837-a81e-4891-9594-501fbf31680f

## Gotchas
- For skinned meshes, the tool bakes a static combined mesh for collider usage (as intended for physics), not skinning, anyways I won't use a `MeshCollider` for a skinned mesh
- Extremely degenerate or non-triangular topologies are skipped
- `glb import produced no loadable assets` <br>
    Install a GLB importer package (UnityGLTF or glTFast) & reoptimize
- Unity limits convex colliders to ≤255 tris, it may auto-simplify further
- If you see warnings about very large triangles (>500 units), check your model scale. The tool bakes transforms, but mismatched import scales can still yield oversized geometry
- The tool attempts to `chmod +x` the `gltfpack` binary, if gatekeeper still blocks it, allow it in System Settings or remove quarantine attributes manually

## Contribution
Contributions to the project are welcome, consider making a PR

## License
MIT License
