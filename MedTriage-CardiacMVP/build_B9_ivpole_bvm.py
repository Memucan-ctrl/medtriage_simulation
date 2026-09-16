import bpy
import bmesh
import math
import os

print("=================== B9. IV POLE + BVM BUILD ===================")

col_name = "IVPole_BVM_Props"
if col_name in bpy.data.collections:
    col = bpy.data.collections[col_name]
else:
    col = bpy.data.collections.new(col_name)
    bpy.context.scene.collection.children.link(col)

def create_box(name, size, loc, parent=None, origin_mode='CENTER'):
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

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    if parent:
        obj.parent = parent
    return obj

def create_cylinder(name, radius, height, loc, parent=None, origin_mode='CENTER'):
    mesh = bpy.data.meshes.new(name + "_Mesh")
    obj = bpy.data.objects.new(name, mesh)
    col.objects.link(obj)
    
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=False, segments=12, radius1=radius, radius2=radius, depth=height)
    
    if origin_mode == 'BOTTOM':
        for v in bm.verts: v.co.z += height * 0.5

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    if parent:
        obj.parent = parent
    return obj

b9_objs = []

# 1. IV Pole
# IVPole_Base (Floor contact origin at Z = 0)
iv_base = create_box("IVPole_Base", (0.50, 0.50, 0.08), (0, 0, 0), origin_mode='BOTTOM')
b9_objs.append(iv_base)

# IVPole_Shaft (Height 1.90 m)
iv_shaft = create_cylinder("IVPole_Shaft", 0.015, 1.82, (0, 0, 0.08), parent=iv_base, origin_mode='BOTTOM')
b9_objs.append(iv_shaft)

# IVPole_Hook_1 & IVPole_Hook_2
hook_1 = create_box("IVPole_Hook_1", (0.25, 0.02, 0.02), (0, 0, 1.88), parent=iv_shaft)
hook_2 = create_box("IVPole_Hook_2", (0.02, 0.25, 0.02), (0, 0, 1.88), parent=iv_shaft)
b9_objs.extend([hook_1, hook_2])

# IV_Bag (500 ml)
iv_bag = create_box("IV_Bag", (0.12, 0.04, 0.22), (0.10, 0, 1.65), parent=hook_1)
b9_objs.append(iv_bag)

# IV_Drip_Chamber
iv_drip = create_cylinder("IV_Drip_Chamber", 0.015, 0.060, (0.10, 0, 1.51), parent=iv_bag)
b9_objs.append(iv_drip)

# IV_Line (10-segment chain: IV_Line_1..10)
prev_seg = iv_drip
for i in range(1, 11):
    line_name = f"IV_Line_{i}"
    l_z = 1.48 - i * 0.06
    l_obj = create_cylinder(line_name, 0.003, 0.060, (0.10, 0, l_z), parent=prev_seg, origin_mode='CENTER')
    b9_objs.append(l_obj)
    prev_seg = l_obj

# 2. Bag Valve Mask (BVM)
# BVM_Bag (origin at CENTER so it gets squeezed correctly in VR)
bvm_bag = create_cylinder("BVM_Bag", 0.075, 0.22, (0.60, 0, 0.80), origin_mode='CENTER')
b9_objs.append(bvm_bag)

# BVM_Valve
bvm_valve = create_cylinder("BVM_Valve", 0.030, 0.08, (0.60, 0.12, 0.80), parent=bvm_bag)
b9_objs.append(bvm_valve)

# BVM_Mask
bvm_mask = create_cylinder("BVM_Mask", 0.060, 0.09, (0.60, 0.18, 0.80), parent=bvm_valve)
b9_objs.append(bvm_mask)

for obj in b9_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b9_objs)

print(f"\n--- B9 IV POLE + BVM METRICS ---")
print(f"Total Objects Count: {len(b9_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 4000: {'YES' if total_tris < 4000 else 'NO'})")
print(f"BVM_Bag Origin: CENTER (Ready for squeeze animation)")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "IVPole_BVM.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b9_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B9_IVPole_BVM.png")

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

print("B9 BUILD COMPLETE.")
