import bpy
import bmesh
import math
import os

print("=================== B1. DEFIBRILLATOR BUILD ===================")

# Clear existing objects in scene or hide patient objects
for obj in list(bpy.data.objects):
    if not obj.name.startswith("CC_Base") and obj.name not in ["Armature", "Patient_Gown", "Camera", "Light"]:
        try:
            bpy.data.objects.remove(obj, do_unlink=True)
        except Exception:
            pass

# Create container collection for Defibrillator
col_name = "Defibrillator_Props"
if col_name in bpy.data.collections:
    col = bpy.data.collections[col_name]
else:
    col = bpy.data.collections.new(col_name)
    bpy.context.scene.collection.children.link(col)

# Helper function to create clean box mesh
def create_box(name, size, loc, rot=(0,0,0), parent=None, origin_mode='CENTER'):
    mesh = bpy.data.meshes.new(name + "_Mesh")
    obj = bpy.data.objects.new(name, mesh)
    col.objects.link(obj)
    
    bm = bmesh.new()
    bmesh.ops.create_cube(bm, size=1.0)

    # Scale box
    sx, sy, sz = size
    for v in bm.verts:
        v.co.x *= sx
        v.co.y *= sy
        v.co.z *= sz
        
        if origin_mode == 'PRESS_FACE': # Origin at front +Y face
            v.co.y -= sy * 0.5
        elif origin_mode == 'GRIP_FACE': # Origin at back -Y face (grip face)
            v.co.y += sy * 0.5
        elif origin_mode == 'BOTTOM': # Origin at bottom -Z face
            v.co.z += sz * 0.5

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    obj.rotation_euler = (math.radians(rot[0]), math.radians(rot[1]), math.radians(rot[2]))
    if parent:
        obj.parent = parent
    return obj

# 1. Defib_Body (0.35 x 0.28 x 0.22 m)
defib_body = create_box("Defib_Body", (0.35, 0.28, 0.22), (0, 0, 0.11), origin_mode='BOTTOM')

# 2. Defib_Screen (0.22 x 0.02 x 0.14 m) on front face
defib_screen = create_box("Defib_Screen", (0.22, 0.02, 0.14), (0, -0.13, 0.14), parent=defib_body)

# 3. Defib_Handle (0.24 x 0.04 x 0.06 m) on top
defib_handle = create_box("Defib_Handle", (0.24, 0.04, 0.06), (0, 0, 0.23), parent=defib_body, origin_mode='CENTER')

# 4. Defib_Knob_Energy (rot axis origin)
defib_knob = create_box("Defib_Knob_Energy", (0.05, 0.03, 0.05), (0.10, -0.13, 0.16), parent=defib_body, origin_mode='CENTER')

# 5. Defib_Button_Charge (press face origin)
defib_btn_charge = create_box("Defib_Button_Charge", (0.04, 0.02, 0.04), (0.10, -0.13, 0.10), parent=defib_body, origin_mode='PRESS_FACE')

# 6. Defib_Button_Shock (press face origin)
defib_btn_shock = create_box("Defib_Button_Shock", (0.04, 0.02, 0.04), (0.10, -0.13, 0.04), parent=defib_body, origin_mode='PRESS_FACE')

# 7. Holsters (Left & Right)
defib_holster_l = create_box("Defib_PadHolster_L", (0.05, 0.12, 0.15), (-0.175, 0, 0.10), parent=defib_body, origin_mode='CENTER')
defib_holster_r = create_box("Defib_PadHolster_R", (0.05, 0.12, 0.15), (0.175, 0, 0.10), parent=defib_body, origin_mode='CENTER')

# 8. Defib_Pad_Anterior & Defib_Pad_Lateral (0.12 x 0.09 x 0.02 m, origin at GRIP FACE, orientation tab on top)
defib_pad_l = create_box("Defib_Pad_Anterior", (0.12, 0.02, 0.09), (-0.22, 0, 0.10), parent=defib_body, origin_mode='GRIP_FACE')
defib_pad_r = create_box("Defib_Pad_Lateral", (0.12, 0.02, 0.09), (0.22, 0, 0.10), parent=defib_body, origin_mode='GRIP_FACE')

# Add small raised orientation tab on top edge of each pad
tab_l = create_box("Defib_Pad_Anterior_Tab", (0.02, 0.01, 0.015), (0, 0.01, 0.05), parent=defib_pad_l, origin_mode='CENTER')
tab_r = create_box("Defib_Pad_Lateral_Tab", (0.02, 0.01, 0.015), (0, 0.01, 0.05), parent=defib_pad_r, origin_mode='CENTER')

# 9. 8-Segment Cable Chains (Pad_Cable_L_1..8 & Pad_Cable_R_1..8)
defib_objs = [defib_body, defib_screen, defib_handle, defib_knob, defib_btn_charge, defib_btn_shock, defib_holster_l, defib_holster_r, defib_pad_l, defib_pad_r, tab_l, tab_r]

for side, p_parent, start_x in [('L', defib_pad_l, -0.22), ('R', defib_pad_r, 0.22)]:
    prev_seg = p_parent
    for seg_i in range(1, 9):
        seg_name = f"Pad_Cable_{side}_{seg_i}"
        seg_obj = create_box(seg_name, (0.015, 0.015, 0.04), (start_x, 0, 0.10 - seg_i * 0.035), parent=prev_seg, origin_mode='CENTER')
        defib_objs.append(seg_obj)
        prev_seg = seg_obj

# Apply transforms (Loc/Rot/Scale)
for obj in defib_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

# Calculate total triangle count
total_tris = 0
for obj in defib_objs:
    t_cnt = sum(len(p.vertices) - 2 for p in obj.data.polygons)
    total_tris += t_cnt

print(f"\n--- B1 DEFIBRILLATOR METRICS ---")
print(f"Total Objects Count: {len(defib_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 6000: {'YES' if total_tris < 6000 else 'NO'})")

# Export B1 FBX
props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "Defibrillator.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in defib_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

# Setup Viewport Screenshot for B1
out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B1_Defibrillator.png")

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

print("B1 BUILD COMPLETE.")
