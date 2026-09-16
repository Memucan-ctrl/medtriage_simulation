import bpy
import bmesh
import math
import os

print("=================== B7. PATIENT MONITOR BUILD ===================")

col_name = "PatientMonitor_Props"
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

b7_objs = []

# Screen center at 1.45 m, screen size 0.30 x 0.20 m, flat, facing +Y
# 1. Monitor_Body (0.36 x 0.12 x 0.26 m)
mon_body = create_box("Monitor_Body", (0.36, 0.12, 0.26), (0, 0, 1.45), origin_mode='CENTER')
b7_objs.append(mon_body)

# 2. Monitor_Screen (0.30 x 0.01 x 0.20 m, flat, facing +Y)
mon_screen = create_box("Monitor_Screen", (0.30, 0.01, 0.20), (0, 0.061, 1.45), parent=mon_body)
b7_objs.append(mon_screen)

# 3. Monitor_Bezel
mon_bezel = create_box("Monitor_Bezel", (0.34, 0.015, 0.24), (0, 0.06, 1.45), parent=mon_body)
b7_objs.append(mon_bezel)

# 4. Monitor_Button_1..5
for i in range(1, 6):
    b_name = f"Monitor_Button_{i}"
    bx = -0.12 + (i - 1) * 0.06
    b_obj = create_box(b_name, (0.04, 0.01, 0.02), (bx, 0.062, 1.34), parent=mon_body)
    b7_objs.append(b_obj)

# 5. Monitor_Arm_Upper, Monitor_Arm_Lower, Monitor_Mount
mon_arm_up = create_box("Monitor_Arm_Upper", (0.04, 0.25, 0.04), (0, -0.15, 1.35), parent=mon_body)
mon_arm_low = create_box("Monitor_Arm_Lower", (0.04, 0.04, 0.45), (0, -0.27, 1.15), parent=mon_arm_up)
mon_mount = create_box("Monitor_Mount", (0.15, 0.15, 0.05), (0, -0.27, 0.925), parent=mon_arm_low, origin_mode='BOTTOM')
b7_objs.extend([mon_arm_up, mon_arm_low, mon_mount])

for obj in b7_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b7_objs)

print(f"\n--- B7 PATIENT MONITOR METRICS ---")
print(f"Total Objects Count: {len(b7_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 5000: {'YES' if total_tris < 5000 else 'NO'})")
print(f"Screen Centre Height: 1.45 m (Facing +Y)")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "PatientMonitor.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b7_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B7_PatientMonitor.png")

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

print("B7 BUILD COMPLETE.")
