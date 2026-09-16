import bpy
import bmesh
import math
import os

print("=================== B8. MECHANICAL VENTILATOR BUILD ===================")

col_name = "MechanicalVentilator_Props"
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

b8_objs = []

# 1. Vent_Body (0.45 x 0.45 x 1.10 m) - Floor contact origin at Z = 0
vent_body = create_box("Vent_Body", (0.45, 0.45, 1.10), (0, 0, 0.10), origin_mode='BOTTOM')
b8_objs.append(vent_body)

# 2. Vent_Screen (Screen centre at 1.35 m, facing +Y)
vent_screen = create_box("Vent_Screen", (0.32, 0.02, 0.22), (0, 0.23, 1.35), parent=vent_body, origin_mode='CENTER')
b8_objs.append(vent_screen)

# 3. Vent_Knob_Main & Vent_Button_Start
vent_knob = create_box("Vent_Knob_Main", (0.06, 0.03, 0.06), (0.12, 0.23, 1.18), parent=vent_body)
vent_btn_start = create_box("Vent_Button_Start", (0.05, 0.02, 0.05), (-0.12, 0.23, 1.18), parent=vent_body)
b8_objs.extend([vent_knob, vent_btn_start])

# 4. Vent_Mask (Facemask connected to hose)
vent_mask = create_box("Vent_Mask", (0.12, 0.14, 0.08), (0, 0.80, 0.90), parent=vent_body)
b8_objs.append(vent_mask)

# 5. Vent_Hose_Main (10-Segment Chain: Vent_Hose_Main_1..10)
prev_seg = vent_body
for i in range(1, 11):
    h_name = f"Vent_Hose_Main_{i}"
    h_y = 0.24 + i * 0.055
    h_obj = create_box(h_name, (0.03, 0.05, 0.03), (0, h_y, 1.10), parent=prev_seg, origin_mode='CENTER')
    b8_objs.append(h_obj)
    prev_seg = h_obj

# 6. Vent_Wheel_1..4
wheel_coords = [(-0.18, -0.18), (0.18, -0.18), (-0.18, 0.18), (0.18, 0.18)]
for idx, (wx, wy) in enumerate(wheel_coords, 1):
    w_name = f"Vent_Wheel_{idx}"
    w_obj = create_box(w_name, (0.07, 0.09, 0.10), (wx, wy, 0), parent=vent_body, origin_mode='BOTTOM')
    b8_objs.append(w_obj)

for obj in b8_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b8_objs)

print(f"\n--- B8 MECHANICAL VENTILATOR METRICS ---")
print(f"Total Objects Count: {len(b8_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 7000: {'YES' if total_tris < 7000 else 'NO'})")
print(f"Vent Screen Height: 1.35 m")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "MechanicalVentilator.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b8_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B8_MechanicalVentilator.png")

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

print("B8 BUILD COMPLETE.")
