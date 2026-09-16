import bpy
import bmesh
import math
import os

print("=================== B3. AMPOULE + SYRINGE + IV CANNULA BUILD ===================")

col_name = "Syringe_Ampoule_Props"
if col_name in bpy.data.collections:
    col = bpy.data.collections[col_name]
else:
    col = bpy.data.collections.new(col_name)
    bpy.context.scene.collection.children.link(col)

def create_cylinder(name, radius, height, loc, parent=None, origin_mode='CENTER'):
    mesh = bpy.data.meshes.new(name + "_Mesh")
    obj = bpy.data.objects.new(name, mesh)
    col.objects.link(obj)
    
    bm = bmesh.new()
    bmesh.ops.create_cone(bm, cap_ends=True, cap_tris=False, segments=12, radius1=radius, radius2=radius, depth=height)
    
    if origin_mode == 'FLANGE': # Origin at bottom base (finger flange)
        for v in bm.verts: v.co.z += height * 0.5
    elif origin_mode == 'TOP_FACE': # Origin at top face (thumb pad)
        for v in bm.verts: v.co.z -= height * 0.5
    elif origin_mode == 'HUB': # Origin at needle hub base
        for v in bm.verts: v.co.z += height * 0.5
        
    bm.to_mesh(mesh)
    bm.free()
    
    obj.location = loc
    if parent:
        obj.parent = parent
    return obj

b3_objs = []

# 1. Epinephrine Ampoule (65 x 16 mm)
amp_body = create_cylinder("Epi_Ampoule_Body", 0.008, 0.045, (-0.10, 0, 0))
amp_neck = create_cylinder("Epi_Ampoule_Neck", 0.004, 0.015, (-0.10, 0, 0.030), parent=amp_body)
amp_band = create_cylinder("Epi_Ampoule_Band", 0.0045, 0.005, (-0.10, 0, 0.025), parent=amp_body)
b3_objs.extend([amp_body, amp_neck, amp_band])

# 2. 10 ml Syringe
# Syringe_Barrel (origin at finger flange)
syr_barrel = create_cylinder("Syringe_Barrel", 0.009, 0.080, (0, 0, 0), origin_mode='FLANGE')
# Syringe_Plunger (SEPARATE object, origin at CENTRE OF TOP FACE / thumb pad, slides 55mm down Z axis)
syr_plunger = create_cylinder("Syringe_Plunger", 0.007, 0.075, (0, 0, 0.130), parent=syr_barrel, origin_mode='TOP_FACE')
# Syringe_Needle (separate, origin at hub)
syr_needle = create_cylinder("Syringe_Needle", 0.001, 0.035, (0, 0, -0.0175), parent=syr_barrel, origin_mode='HUB')
b3_objs.extend([syr_barrel, syr_plunger, syr_needle])

# 3. IV Cannula
iv_hub = create_cylinder("IV_Cannula_Hub", 0.005, 0.025, (0.10, 0, 0))
iv_port = create_cylinder("IV_Cannula_Port", 0.004, 0.015, (0.10, 0, 0.015), parent=iv_hub, origin_mode='FLANGE')
iv_tape = create_cylinder("IV_Cannula_Tape", 0.012, 0.002, (0.10, 0, -0.005), parent=iv_hub)
b3_objs.extend([iv_hub, iv_port, iv_tape])

for obj in b3_objs:
    bpy.context.view_layer.objects.active = obj
    obj.select_set(True)

bpy.ops.object.transform_apply(location=False, rotation=True, scale=True)

total_tris = sum(sum(len(p.vertices) - 2 for p in obj.data.polygons) for obj in b3_objs)

print(f"\n--- B3 SYRINGE + AMPOULE + CANNULA METRICS ---")
print(f"Total Objects Count: {len(b3_objs)}")
print(f"Total Triangle Count: {total_tris} (Must be < 900 for ampoule, low poly total)")

props_dir = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"
os.makedirs(props_dir, exist_ok=True)
fbx_path = os.path.join(props_dir, "Syringe_Ampoule_Cannula.fbx")

bpy.ops.object.select_all(action='DESELECT')
for obj in b3_objs:
    obj.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=fbx_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH'}, add_leaf_bones=False, use_selection=True
)

sz = os.path.getsize(fbx_path)
print(f"FBX Exported: {fbx_path} ({sz} bytes, {sz/1024:.2f} KB)")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "B3_Syringe_Ampoule_Cannula.png")

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

print("B3 BUILD COMPLETE.")
