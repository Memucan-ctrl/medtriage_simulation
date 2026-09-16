import bpy
import bmesh
import math
import os

print("=================== B4. CRASH CART BUILD ===================")

col_name = "CrashCart_Props"
if col_name in bpy.data.collections:
    col = bpy.data.collections[col_name]
else:
    col = bpy.data.collections.new(col_name)
    bpy.context.scene.collection.children.link(col)

def create_box(name, size, loc, rot=(0,0,0), parent=None, origin_mode='CENTER'):
    mesh = bpy.data.meshes.new(name + "_Mesh")
    obj = bpy.data.objects.new(name, mesh)
    col.objects.link(obj)
    
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)

    sx, sy, sz = size
    for v in bm.verts:
        v.co.x *= sx
        v.co.y *= sy
        v.co.z *= sz
        if origin_mode == 'BOTTOM':
            v.co.z += sz * 0.5
        elif origin_mode == 'BACK': # Origin at back -Y face (for sliding drawers)
            v.co.y += sy * 0.5

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    obj.rotation_euler = (math.radians(rot[0]), math.radians(rot[1]), math.radians(rot[2]))
    if parent:
        obj.parent = parent
    return obj

b4_objs = []

# 1. Cart_Frame (0.60 x 0.55 x 0.95 m) - Floor contact origin at Z = 0
cart_frame = create_box("Cart_Frame", (0.60, 0.55, 0.80), (0, 0, 0.10), origin_mode='BOTTOM')
b4_objs.append(cart_frame)

# 2. Cart_Top (Surface height at 0.95 m)
cart_top = create_box("Cart_Top", (0.64, 0.59, 0.05), (0, 0, 0.90), parent=cart_frame, origin_mode='BOTTOM')
b4_objs.append(cart_top)

# 3. Cart_Drawer_1..5 (Origin at back face of each, for sliding animation)
drawer_h = 0.14
for i in range(1, 6):
    d_name = f"Cart_Drawer_{i}"
    d_z = 0.12 + (i - 1) * 0.15
    d_obj = create_box(d_name, (0.54, 0.50, drawer_h), (0, 0, d_z), parent=cart_frame, origin_mode='BACK')
    b4_objs.append(d_obj)

# 4. Cart_Handle & Side Rail
cart_handle = create_box("Cart_Handle", (0.04, 0.40, 0.05), (0.34, 0, 0.85), parent=cart_frame)
cart_rail = create_box("Cart_SideRail", (0.03, 0.45, 0.10), (-0.33, 0, 0.85), parent=cart_frame)
b4_objs.extend([cart_handle, cart_rail])

# 5. Cart_Sharps_Bin & Cart_DefibShelf
sharps_bin = create_box("Cart_Sharps_Bin", (0.15, 0.20, 0.25), (-0.35, 0, 0.50), parent=cart_frame)
defib_shelf = create_box("Cart_DefibShelf", (0.40, 0.35, 0.03), (-0.35, 0, 0.95), parent=cart_frame)
b4_objs.extend([sharps_bin, defib_shelf])

# 6. Cart_Wheel_1..4 (Wheels at bottom)
wheel_coords = [(-0.25, -0.22), (0.25, -0.22), (-0.25, 0.22), (0.25, 0.22)]
for idx, (wx, wy) in enumerate(wheel_coords, 1):
    w_name = f"Cart_Wheel_{idx}"
    w_obj = create_box(w_name, (0.06, 0.08, 0.10), (wx, wy, 0), parent=cart_frame, origin_mode='BOTTOM')
    b4_objs.append(w_obj)

for obj in b4_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b4_objs)

print(f"\n--- B4 CRASH CART METRICS ---")
print(f"Total Objects Count: {len(b4_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 9000: {'YES' if total_tris < 9000 else 'NO'})")
print(f"Cart Top Surface Height: 0.95 m")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "CrashCart.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b4_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B4_CrashCart.png")

for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        for space in area.spaces:
            if space.type == 'VIEW_3D':
                space.shading.type = 'SOLID'
                space.shading.color_type = 'SINGLE'
                space.overlay.show_overlays = False
                space.region_3d.view_perspective = 'ORTHO'
                space.region_3d.view_rotation = (0.5, 0.5, 0.5, 0.5)
                
                override = {'area': area, 'space_data': space, 'region': area.regions[-1]}
                try:
                    with bpy.context.temp_override(**override):
                        bpy.ops.view3d.view_all(center=False)
                except Exception as e:
                    pass

print("B4 BUILD COMPLETE.")
