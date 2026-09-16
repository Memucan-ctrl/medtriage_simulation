import bpy
import bmesh
import math
import os

print("=================== B5. BED WITH BACKREST BUILD ===================")

col_name = "HospitalBed_Props"
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
        elif origin_mode == 'TOP_FACE': # Origin at center of top face
            v.co.z -= sz * 0.5
        elif origin_mode == 'HINGE': # Origin at back hinge axis
            v.co.y += sy * 0.5

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    obj.rotation_euler = (math.radians(rot[0]), math.radians(rot[1]), math.radians(rot[2]))
    if parent:
        obj.parent = parent
    return obj

b5_objs = []

# 1. Bed_Frame (0.95 x 2.05 x 0.45 m) - Floor contact origin at Z = 0
bed_frame = create_box("Bed_Frame", (0.95, 2.05, 0.45), (0, 0, 0), origin_mode='BOTTOM')
b5_objs.append(bed_frame)

# 2. Bed_Mattress (0.90 x 2.00 x 0.15 m) - FLAT top surface, top at 0.60 m, origin at CENTRE OF TOP FACE
bed_mattress = create_box("Bed_Mattress", (0.90, 2.00, 0.15), (0, 0, 0.60), parent=bed_frame, origin_mode='TOP_FACE')
b5_objs.append(bed_mattress)

# 3. Bed_Backrest (origin at hinge axis so it raises)
bed_backrest = create_box("Bed_Backrest", (0.88, 0.80, 0.12), (0, -0.40, 0.55), parent=bed_mattress, origin_mode='HINGE')
b5_objs.append(bed_backrest)

# 4. Bed_Rail_L & Bed_Rail_R (0.95 m length)
bed_rail_l = create_box("Bed_Rail_L", (0.04, 0.95, 0.25), (-0.47, 0, 0.60), parent=bed_frame, origin_mode='BOTTOM')
bed_rail_r = create_box("Bed_Rail_R", (0.04, 0.95, 0.25), (0.47, 0, 0.60), parent=bed_frame, origin_mode='BOTTOM')
b5_objs.extend([bed_rail_l, bed_rail_r])

# 5. Bed_Sheet (covering mattress top)
bed_sheet = create_box("Bed_Sheet", (0.91, 2.01, 0.01), (0, 0, 0.601), parent=bed_mattress, origin_mode='TOP_FACE')
b5_objs.append(bed_sheet)

# 6. Bed_Wheel_1..4
wheel_coords = [(-0.40, -0.90), (0.40, -0.90), (-0.40, 0.90), (0.40, 0.90)]
for idx, (wx, wy) in enumerate(wheel_coords, 1):
    w_name = f"Bed_Wheel_{idx}"
    w_obj = create_box(w_name, (0.08, 0.12, 0.15), (wx, wy, 0), parent=bed_frame, origin_mode='BOTTOM')
    b5_objs.append(w_obj)

for obj in b5_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b5_objs)

print(f"\n--- B5 BED WITH BACKREST METRICS ---")
print(f"Total Objects Count: {len(b5_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 8000: {'YES' if total_tris < 8000 else 'NO'})")
print(f"Mattress Top Surface Height: 0.60 m (FLAT)")
print(f"Mattress Dimensions: 0.90 x 2.00 m")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "HospitalBed.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b5_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B5_HospitalBed.png")

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

print("B5 BUILD COMPLETE.")
