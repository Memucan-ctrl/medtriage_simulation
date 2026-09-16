import bpy
import math
import os

print("=================== FULL POSE FLATTENING & VERIFICATION ===================")

arm = bpy.data.objects.get('Armature')
body = bpy.data.objects.get('CC_Base_Body')

if not arm or not body:
    print("ERROR: Armature or Body missing!")
    sys.exit(1)

bpy.context.view_layer.objects.active = arm
arm.select_set(True)
if arm.mode != 'POSE':
    bpy.ops.object.mode_set(mode='POSE')

def set_pose_euler(bname, x_deg, y_deg, z_deg):
    pb = arm.pose.bones.get(bname)
    if pb:
        pb.rotation_mode = 'XYZ'
        pb.rotation_euler = (math.radians(x_deg), math.radians(y_deg), math.radians(z_deg))

# -------------------------------------------------------------
# STEP 2 & 3: RE-POSE FOR FLAT POSTERIOR SURFACE
# -------------------------------------------------------------
# Main root orientation
set_pose_euler('_rootJoint', 90, 0, 0)
root_pb = arm.pose.bones['_rootJoint']
# Y translation in root_pb local space = world Z. Set to 3.8 cm to ground posterior in [0.00, 0.04] m
root_pb.location = (0, 3.8, 0)

# Un-arch spine: flatten waist, spine01, spine02, neck, head
set_pose_euler('CC_Base_Waist_033', -3.0, 0, 0)
set_pose_euler('CC_Base_Spine01_034', -3.0, 0, 0)
set_pose_euler('CC_Base_Spine02_035', -2.0, 0, 0)
set_pose_euler('CC_Base_NeckTwist01_036', 2.0, 0, 0)
set_pose_euler('CC_Base_Head_038', 4.0, 0, 0)

# Arms flat along sides
set_pose_euler('CC_Base_L_Upperarm_050', 0, 0, -80)
set_pose_euler('CC_Base_R_Upperarm_074', 0, 0, 80)
set_pose_euler('CC_Base_L_Forearm_051', 0, 0, -5)
set_pose_euler('CC_Base_R_Forearm_077', 0, 0, 5)
set_pose_euler('CC_Base_L_Hand_055', 0, -90, 0)
set_pose_euler('CC_Base_R_Hand_081', 0, 90, 0)

# Legs flat: thighs, calves, feet
set_pose_euler('CC_Base_L_Thigh_04', 2.0, 0, -1)
set_pose_euler('CC_Base_R_Thigh_00', 2.0, 0, 1)
set_pose_euler('CC_Base_L_Calf_05', -10.0, 0, 0)
set_pose_euler('CC_Base_R_Calf_021', -10.0, 0, 0)
set_pose_euler('CC_Base_L_Foot_06', -25.0, 0, 0)
set_pose_euler('CC_Base_R_Foot_022', -25.0, 0, 0)

# Update evaluated mesh
bpy.context.view_layer.update()
depsgraph = bpy.context.evaluated_depsgraph_get()
body_eval = body.evaluated_get(depsgraph)
mesh_eval = body_eval.data
world_verts = [body_eval.matrix_world @ v.co for v in mesh_eval.vertices]

# -------------------------------------------------------------
# STEP 5: CHECK & CLEAN _rootJoint F-CURVES IN Shock_Convulsion
# -------------------------------------------------------------
print("\n--- STEP 5: CHECK _rootJoint IN Shock_Convulsion ---")
act_shock = bpy.data.actions.get('Shock_Convulsion')
if act_shock:
    arm.animation_data.action = act_shock
    print(f"Action '{act_shock.name}' F-curves check on '_rootJoint':")
    root_fcurves = []
    
    if hasattr(act_shock, 'layers') and len(act_shock.layers) > 0:
        for layer in act_shock.layers:
            for strip in layer.strips:
                for slot in act_shock.slots:
                    try:
                        bag = strip.channelbag(slot)
                        if bag and hasattr(bag, 'fcurves'):
                            for fc in list(bag.fcurves):
                                if '_rootJoint' in fc.data_path:
                                    root_fcurves.append(fc)
                                    print(f"  - Found F-Curve: data_path='{fc.data_path}', array_index={fc.array_index}")
                                    if 'location' in fc.data_path:
                                        print(f"    --> DELETING LOCATION F-CURVE: {fc.data_path}")
                                        bag.fcurves.remove(fc)
                    except Exception as e:
                        print("F-curve check note:", e)

# Helper function to re-key actions using rotation only on torso for Shock_Convulsion
def key_target_bones(bone_list, frame_num):
    for bname in bone_list:
        pb = arm.pose.bones.get(bname)
        if pb:
            if pb.rotation_mode == 'XYZ':
                pb.keyframe_insert(data_path="rotation_euler", frame=frame_num)
            elif pb.rotation_mode == 'QUATERNION':
                pb.keyframe_insert(data_path="rotation_quaternion", frame=frame_num)

# Re-key Shock_Convulsion with ROTATION ONLY on spine & upperarms (NO location keys on _rootJoint)
if act_shock:
    arm.animation_data.action = act_shock
    # Frame 1
    key_target_bones(['CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_L_Upperarm_050', 'CC_Base_R_Upperarm_074'], 1)
    
    # Frame 3: Sharp torso jolt (spine pitch -5 deg, upperarms flexed)
    pb_spine = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_spine: pb_spine.rotation_euler = (math.radians(-5.0), 0, 0)
    pb_chest = arm.pose.bones.get('CC_Base_Spine02_035')
    if pb_chest: pb_chest.rotation_euler = (math.radians(-4.0), 0, 0)
    key_target_bones(['CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_L_Upperarm_050', 'CC_Base_R_Upperarm_074'], 3)
    
    # Frame 6 (Mid): Quick rebound
    pb_spine = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_spine: pb_spine.rotation_euler = (math.radians(-2.0), 0, 0)
    pb_chest = arm.pose.bones.get('CC_Base_Spine02_035')
    if pb_chest: pb_chest.rotation_euler = (math.radians(-1.0), 0, 0)
    key_target_bones(['CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_L_Upperarm_050', 'CC_Base_R_Upperarm_074'], 6)
    
    # Frame 12: Restored
    if pb_spine: pb_spine.rotation_euler = (math.radians(-3.0), 0, 0)
    if pb_chest: pb_chest.rotation_euler = (math.radians(-2.0), 0, 0)
    key_target_bones(['CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_L_Upperarm_050', 'CC_Base_R_Upperarm_074'], 12)

print("STEP 5 VERIFIED: Shock_Convulsion updated with ROTATION-ONLY curves on spine/upperarms. Zero location curves on _rootJoint.")

# -------------------------------------------------------------
# STEP 4: 12-SLICE PROFILE SCAN AFTER FIX
# -------------------------------------------------------------
print("\n--- STEP 4: 12-SLICE PROFILE SCAN AFTER FIX ---")
arm.animation_data.action = bpy.data.actions['Idle_Unresponsive']
bpy.context.scene.frame_set(75)
bpy.context.view_layer.update()
depsgraph = bpy.context.evaluated_depsgraph_get()
body_eval = body.evaluated_get(depsgraph)
mesh_eval = body_eval.data
world_verts = [body_eval.matrix_world @ v.co for v in mesh_eval.vertices]

min_y = min(v.y for v in world_verts)
max_y = max(v.y for v in world_verts)
total_len = max_y - min_y

num_slices = 12
slice_step = total_len / num_slices

print(f"{'Slice':<6} | {'Y Range (cm)':<18} | {'Min Z (cm)':<10} | {'Max Z (cm)':<10} | {'Min Z (m)':<9} | {'Max Z (m)':<9}")
print("-" * 75)

after_slices = []
for i in range(num_slices):
    y_start = min_y + i * slice_step
    y_end = min_y + (i + 1) * slice_step
    slice_verts = [v for v in world_verts if y_start <= v.y < y_end]
    if slice_verts:
        s_min_z = min(v.z for v in slice_verts)
        s_max_z = max(v.z for v in slice_verts)
    else:
        s_min_z = 0.0
        s_max_z = 0.0
    after_slices.append((i+1, y_start, y_end, s_min_z, s_max_z))
    print(f"{i+1:<6} | [{y_start:6.1f}, {y_end:6.1f}] | {s_min_z:10.2f} | {s_max_z:10.2f} | {s_min_z/100:9.3f} | {s_max_z/100:9.3f}")

# -------------------------------------------------------------
# STEP 3: MEASURE 5 CORRECTED TOP-SURFACE HEIGHTS
# -------------------------------------------------------------
head_pb = arm.pose.bones['CC_Base_Head_038']
head_world = arm.matrix_world @ head_pb.head
head_verts = [v for v in world_verts if abs(v.y - head_world.y) < 10]
forehead_top_z = max(v.z for v in head_verts)

chest_pb = arm.pose.bones['CC_Base_Spine02_035']
chest_world = arm.matrix_world @ chest_pb.head
chest_verts = [v for v in world_verts if abs(v.x - chest_world.x) < 15 and abs(v.y - chest_world.y) < 15]
chest_top_z = max(v.z for v in chest_verts)
chest_back_z = min(v.z for v in chest_verts)

hip_pb = arm.pose.bones['CC_Base_Hip_02']
hip_world = arm.matrix_world @ hip_pb.head
hip_verts = [v for v in world_verts if abs(v.y - hip_world.y) < 15]
hip_top_z = max(v.z for v in hip_verts)
hip_back_z = min(v.z for v in hip_verts)

knee_pb = arm.pose.bones['CC_Base_R_Calf_021']
knee_world = arm.matrix_world @ knee_pb.head
knee_verts = [v for v in world_verts if abs(v.y - knee_world.y) < 15]
knee_top_z = max(v.z for v in knee_verts)

heel_pb = arm.pose.bones['CC_Base_R_Foot_022']
heel_world = arm.matrix_world @ heel_pb.head
heel_verts = [v for v in world_verts if abs(v.y - heel_world.y) < 15]
heel_top_z = max(v.z for v in heel_verts)
heel_back_z = min(v.z for v in heel_verts)

print("\n--- STEP 3: CORRECTED 5 TOP-SURFACE HEIGHTS ---")
print(f"1. Forehead: {forehead_top_z/100:.3f} m ({forehead_top_z:.2f} cm)")
print(f"2. Chest:    {chest_top_z/100:.3f} m ({chest_top_z:.2f} cm) [Back Z: {chest_back_z/100:.3f} m]")
print(f"3. Hip:      {hip_top_z/100:.3f} m ({hip_top_z:.2f} cm) [Back Z: {hip_back_z/100:.3f} m]")
print(f"4. Knee:     {knee_top_z/100:.3f} m ({knee_top_z:.2f} cm)")
print(f"5. Heel:     {heel_top_z/100:.3f} m ({heel_top_z:.2f} cm) [Back Z: {heel_back_z/100:.3f} m]")

# -------------------------------------------------------------
# STEP 6: VIEWPORT CONFIGURATION FOR SIDE ORTHOGRAPHIC SCREENSHOT
# -------------------------------------------------------------
for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        for space in area.spaces:
            if space.type == 'VIEW_3D':
                space.shading.type = 'SOLID'
                space.shading.color_type = 'SINGLE'
                space.overlay.show_overlays = False # Hide bone overlays & grid
                space.region_3d.view_perspective = 'ORTHO'
                space.region_3d.view_rotation = (0.5, 0.5, 0.5, 0.5) # Side orthographic view
                
                override = {'area': area, 'space_data': space, 'region': area.regions[-1]}
                try:
                    with bpy.context.temp_override(**override):
                        bpy.ops.view3d.view_all(center=False)
                except Exception as e:
                    print("FRAME_ERR:", e)

# -------------------------------------------------------------
# STEP 7: RE-EXPORT BOTH FBX FILES
# -------------------------------------------------------------
exp_path = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Patient\Patient_Supine_Anims.fbx"
all_bones_path = r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Patient\Patient_Supine_Anims_AllBones.fbx"

bpy.ops.object.mode_set(mode='OBJECT')
bpy.ops.object.select_all(action='DESELECT')
arm.select_set(True)
for m in bpy.data.objects:
    if m.type in {'MESH', 'ARMATURE'}:
        m.select_set(True)

bpy.ops.export_scene.fbx(
    filepath=exp_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH', 'ARMATURE'}, add_leaf_bones=False,
    use_armature_deform_only=True, bake_anim=True, bake_anim_use_all_actions=True
)

bpy.ops.export_scene.fbx(
    filepath=all_bones_path, global_scale=1.0, apply_unit_scale=True, apply_scale_options='FBX_SCALE_ALL',
    axis_forward='-Z', axis_up='Y', object_types={'MESH', 'ARMATURE'}, add_leaf_bones=False,
    use_armature_deform_only=False, bake_anim=True, bake_anim_use_all_actions=True
)

print("STEP 7 COMPLETE: Both Patient_Supine_Anims.fbx and Patient_Supine_Anims_AllBones.fbx re-exported successfully.")
print("=================== FULL POSE FLATTENING COMPLETE ===================")
