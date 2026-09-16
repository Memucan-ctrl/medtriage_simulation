import bpy
import bmesh
import math
import os

print("=================== B2. PULSE OXIMETER BUILD ===================")

col_name = "PulseOximeter_Props"
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
        if origin_mode == 'HINGE': # Origin at back -Y face (hinge axis)
            v.co.y += sy * 0.5
        elif origin_mode == 'BOTTOM':
            v.co.z += sz * 0.5

    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    obj.rotation_euler = (math.radians(rot[0]), math.radians(rot[1]), math.radians(rot[2]))
    if parent:
        obj.parent = parent
    return obj

# Hinge axis position at (0, 0, 0)
# Upper clip (0.045 x 0.060 x 0.015 m) above hinge with 25mm finger gap
ox_clip_upper = create_box("Oximeter_Clip_Upper", (0.045, 0.060, 0.015), (0, 0.030, 0.0125), origin_mode='HINGE')

# Lower clip (0.045 x 0.060 x 0.015 m) below hinge
ox_clip_lower = create_box("Oximeter_Clip_Lower", (0.045, 0.060, 0.015), (0, 0.030, -0.0125), parent=ox_clip_upper, origin_mode='HINGE')

# Screen on top face of upper clip
ox_screen = create_box("Oximeter_Screen", (0.030, 0.035, 0.002), (0, 0.035, 0.021), parent=ox_clip_upper, origin_mode='CENTER')

# 6-Segment Cable Chain (Oximeter_Cable_1..6)
ox_objs = [ox_clip_upper, ox_clip_lower, ox_screen]
prev_seg = ox_clip_upper
for i in range(1, 7):
    seg_name = f"Oximeter_Cable_{i}"
    seg_obj = create_box(seg_name, (0.008, 0.008, 0.030), (0, -0.005 - i * 0.025, 0), parent=prev_seg, origin_mode='CENTER')
    ox_objs.append(seg_obj)
    prev_seg = seg_obj

for obj in ox_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in ox_objs)

print(f"\n--- B2 PULSE OXIMETER METRICS ---")
print(f"Total Objects Count: {len(ox_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 1200: {'YES' if total_tris < 1200 else 'NO'})")

# Export B2 FBX
props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "PulseOximeter.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in ox_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B2_PulseOximeter.png")

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

print("B2 BUILD COMPLETE.")
