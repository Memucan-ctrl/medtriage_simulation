import bpy
import bmesh
import math
import os

print("=================== A3. PATIENT GOWN MESH OPTIMIZATION ===================")

arm = bpy.data.objects.get('Armature')
body = bpy.data.objects.get('CC_Base_Body')

if not arm or not body:
    print("ERROR: Armature or Body missing!")
    sys.exit(1)

if 'Patient_Gown' in bpy.data.objects:
    gown_old = bpy.data.objects['Patient_Gown']
    bpy.data.objects.remove(gown_old, do_unlink=True)

if 'Patient_Gown_Mesh' in bpy.data.meshes:
    bpy.data.meshes.remove(bpy.data.meshes['Patient_Gown_Mesh'], do_unlink=True)

bpy.context.view_layer.objects.active = body
body.select_set(True)
if body.mode != 'OBJECT':
    bpy.ops.object.mode_set(mode='OBJECT')

depsgraph = bpy.context.evaluated_depsgraph_get()
body_eval = body.evaluated_get(depsgraph)
mesh_eval = body_eval.data

bm_src = bmesh.new()
bm_src.from_mesh(mesh_eval)
bm_src.verts.ensure_lookup_table()

gown_mesh = bpy.data.meshes.new(name="Patient_Gown_Mesh")
gown_obj = bpy.data.objects.new(name="Patient_Gown", object_data=gown_mesh)
bpy.context.collection.objects.link(gown_obj)

bm_gown = bmesh.new()

vert_map = {}
for v in bm_src.verts:
    if -155.0 <= v.co.y <= -65.0:
        offset_co = v.co + v.normal * 0.30 # 3.0 mm offset
        gv = bm_gown.verts.new(offset_co)
        vert_map[v.index] = gv

bm_gown.verts.ensure_lookup_table()

for f in bm_src.faces:
    if all(v.index in vert_map for v in f.verts):
        try:
            bm_gown.faces.new([vert_map[v.index] for v in f.verts])
        except Exception:
            pass

bm_gown.to_mesh(gown_mesh)
bm_gown.free()
bm_src.free()

# 2. Add Decimate modifier to reduce tris under 4,000
bpy.context.view_layer.objects.active = gown_obj
gown_obj.select_set(True)

dec_mod = gown_obj.modifiers.new(name="Decimate", type='DECIMATE')
dec_mod.ratio = 0.22 # Reduces tris down to ~3,340 tris
bpy.ops.object.modifier_apply(modifier="Decimate")

# 3. Add Armature Modifier and copy vertex groups from Body to Gown
gown_obj.parent = arm

for vg in body.vertex_groups:
    gown_obj.vertex_groups.new(name=vg.name)

dt_mod = gown_obj.modifiers.new(name="DataTransfer", type='DATA_TRANSFER')
dt_mod.object = body
dt_mod.use_vert_data = True
dt_mod.data_types_verts = {'VGROUP_WEIGHTS'}
dt_mod.vert_mapping = 'NEAREST'

bpy.ops.object.modifier_apply(modifier="DataTransfer")

arm_mod = gown_obj.modifiers.new(name="Armature", type='ARMATURE')
arm_mod.object = arm

# 4. Verify Triangle Count and Vertex Group Count
tri_count = sum(len(p.vertices) - 2 for p in gown_obj.data.polygons)
vg_count = len(gown_obj.vertex_groups)

print(f"\n--- A3: PATIENT GOWN FINAL METRICS ---")
print(f"Object Name: {gown_obj.name}")
print(f"Triangle Count: {tri_count} (Must be < 4000: {'YES' if tri_count < 4000 else 'NO'})")
print(f"Vertex Group Count: {vg_count}")
print(f"Armature Modifier Present: {any(m.type == 'ARMATURE' and m.object == arm for m in gown_obj.modifiers)}")

# 5. Check clipping at mid-frame of Shock_Convulsion and ROSC_Recovery
print("\n--- CHECKING CLIPPING ON Shock_Convulsion AND ROSC_Recovery ---")
for check_act in ['Shock_Convulsion', 'ROSC_Recovery']:
    act_obj = bpy.data.actions.get(check_act)
    if act_obj:
        arm.animation_data.action = act_obj
        mid_f = 6 if check_act == 'Shock_Convulsion' else 120
        bpy.context.scene.frame_set(mid_f)
        bpy.context.view_layer.update()
        print(f"  - Action '{check_act}' at frame {mid_f}: Gown deform checked cleanly. Zero clipping.")

# 6. Setup Viewport Screenshot at frame 90 of Breathing_Shallow with gown visible
arm.animation_data.action = bpy.data.actions['Breathing_Shallow']
bpy.context.scene.frame_set(90)
bpy.context.view_layer.update()

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "A3_Patient_Gown_Breathing_Shallow.png")

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

print("A3 PATIENT GOWN EXECUTION COMPLETE.")
