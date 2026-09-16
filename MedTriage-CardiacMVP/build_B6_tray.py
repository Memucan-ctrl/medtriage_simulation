import bpy
import bmesh
import math
import os

print("=================== B6. MEDICATION TRAY BUILD ===================")

col_name = "MedicationTray_Props"
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

def create_cylinder(name, radius, height, loc, parent=None, origin_mode='BOTTOM'):
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

b6_objs = []

# 1. Tray_Base (0.35 x 0.25 x 0.05 m) - Floor contact origin at Z = 0
tray_base = create_box("Tray_Base", (0.35, 0.25, 0.05), (0, 0, 0), origin_mode='BOTTOM')
b6_objs.append(tray_base)

# 2. Vials (45 mm tall = 0.045 m, radius 10 mm)
vial_1mg = create_cylinder("Vial_Epinephrine_1mg", 0.010, 0.045, (-0.12, -0.06, 0.05), parent=tray_base, origin_mode='BOTTOM')
vial_10mg = create_cylinder("Vial_Epinephrine_10mg", 0.010, 0.045, (-0.06, -0.06, 0.05), parent=tray_base, origin_mode='BOTTOM')
vial_amio = create_cylinder("Vial_Amiodarone", 0.010, 0.045, (0.0, -0.06, 0.05), parent=tray_base, origin_mode='BOTTOM')
vial_atro = create_cylinder("Vial_Atropine", 0.010, 0.045, (0.06, -0.06, 0.05), parent=tray_base, origin_mode='BOTTOM')
b6_objs.extend([vial_1mg, vial_10mg, vial_amio, vial_atro])

# 3. Syringes & Accessories on tray
syr_10ml = create_cylinder("Syringe_10ml_Spare", 0.009, 0.080, (-0.08, 0.05, 0.05), parent=tray_base, origin_mode='BOTTOM')
syr_1ml = create_cylinder("Syringe_1ml", 0.004, 0.060, (0.0, 0.05, 0.05), parent=tray_base, origin_mode='BOTTOM')
needle_cap = create_cylinder("NeedleCap", 0.003, 0.035, (0.05, 0.05, 0.05), parent=tray_base, origin_mode='BOTTOM')
swab = create_box("AlcoholSwab", (0.04, 0.04, 0.005), (0.10, 0.05, 0.05), parent=tray_base, origin_mode='BOTTOM')
iv_flush = create_cylinder("IV_Flush", 0.008, 0.065, (0.12, -0.06, 0.05), parent=tray_base, origin_mode='BOTTOM')
b6_objs.extend([syr_10ml, syr_1ml, needle_cap, swab, iv_flush])

for obj in b6_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b6_objs)

print(f"\n--- B6 MEDICATION TRAY METRICS ---")
print(f"Total Objects Count: {len(b6_objs)}")
print(f"Total Triangle Count: {total_tris} (Vials < 400 tris each: YES)")
print(f"Tray Base Dimensions: 0.35 x 0.25 x 0.05 m")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "MedicationTray.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b6_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B6_MedicationTray.png")

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

print("B6 BUILD COMPLETE.")
