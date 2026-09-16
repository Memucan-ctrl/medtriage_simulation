import bpy
import os

print("=================== A4. RE-EXPORT BOTH FBX FILES ===================")

arm = bpy.data.objects.get('Armature')
if not arm:
    print("ERROR: Armature missing!")
    sys.exit(1)

# Count bones in Armature
total_bones = len(arm.data.bones)
print(f"ARMATURE TOTAL BONES: {total_bones} (Must be 192: {'YES' if total_bones == 192 else 'NO'})")

# Select Armature and all mesh objects (Body, Gown, Teeth, Tongue, Eye, etc.)
bpy.ops.object.mode_set(mode='OBJECT')
bpy.ops.object.select_all(action='DESELECT')
arm.select_set(True)

exp_objs = []
for m in bpy.data.objects:
    if m.type in {'MESH', 'ARMATURE'}:
        m.select_set(True)
        exp_objs.append(m.name)

print(f"OBJECTS INCLUDED IN EXPORT: {exp_objs}")

exp_path = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Patient\Patient_Supine_Anims.fbx"
all_bones_path = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Patient\Patient_Supine_Anims_AllBones.fbx"

# Export Patient_Supine_Anims.fbx
bpy.ops.export_scene.fbx(
    filepath=exp_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH', 'ARMATURE'}, add_leaf_bones=False,
    use_armature_deform_only=True, bake_anim=True, bake_anim_use_all_actions=True
)

# Export Patient_Supine_Anims_AllBones.fbx
bpy.ops.export_scene.fbx(
    filepath=all_bones_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH', 'ARMATURE'}, add_leaf_bones=False,
    use_armature_deform_only=False, bake_anim=True, bake_anim_use_all_actions=True
)

sz1 = os.path.getsize(exp_path)
sz2 = os.path.getsize(all_bones_path)

print(f"\n--- A4: FBX EXPORT METRICS ---")
print(f"1. {exp_path} ({sz1} bytes, {sz1/(1024*1024):.2f} MB) - Bone Count: {total_bones}")
print(f"2. {all_bones_path} ({sz2} bytes, {sz2/(1024*1024):.2f} MB) - Bone Count: {total_bones}")

# Save screenshot for A4 confirmation
out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "A4_Patient_Final_Supine.png")

arm.animation_data.action = bpy.data.actions['Idle_Conscious']
bpy.context.scene.frame_set(1)
bpy.context.view_layer.update()

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

print("A4 RE-EXPORT COMPLETE.")
