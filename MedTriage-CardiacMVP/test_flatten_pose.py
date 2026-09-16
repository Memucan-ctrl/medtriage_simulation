import bpy
import math

print("=================== FINE-TUNING FLATTENING POSE ===================")

arm = bpy.data.objects.get('Armature')
body = bpy.data.objects.get('CC_Base_Body')

bpy.context.view_layer.objects.active = arm
arm.select_set(True)
if arm.mode != 'POSE':
    bpy.ops.object.mode_set(mode='POSE')

def set_pose_euler(bname, x_deg, y_deg, z_deg):
    pb = arm.pose.bones.get(bname)
    if pb:
        pb.rotation_mode = 'XYZ'
        pb.rotation_euler = (math.radians(x_deg), math.radians(y_deg), math.radians(z_deg))

# 1. Main root
set_pose_euler('_rootJoint', 90, 0, 0)
root_pb = arm.pose.bones['_rootJoint']
# Fine tune Y translation to set posterior surface Z in [0.00, 0.04] m
root_pb.location = (0, 11.5, 0)

# 2. Spine & neck: un-arch spine so upper back and lower back sit flat
set_pose_euler('CC_Base_Waist_033', -2.0, 0, 0)
set_pose_euler('CC_Base_Spine01_034', -2.0, 0, 0)
set_pose_euler('CC_Base_Spine02_035', -2.0, 0, 0)
set_pose_euler('CC_Base_NeckTwist01_036', 3.0, 0, 0)
set_pose_euler('CC_Base_Head_038', 5.0, 0, 0)       # Lift head slightly so forehead top is ~0.21m

# 3. Arms flat at sides
set_pose_euler('CC_Base_L_Upperarm_050', 0, 0, -80)
set_pose_euler('CC_Base_R_Upperarm_074', 0, 0, 80)
set_pose_euler('CC_Base_L_Forearm_051', 0, 0, -5)
set_pose_euler('CC_Base_R_Forearm_077', 0, 0, 5)
set_pose_euler('CC_Base_L_Hand_055', 0, -90, 0)
set_pose_euler('CC_Base_R_Hand_081', 0, 90, 0)

# 4. Legs: adjust thigh and calf rotation so calves & heels lie flat
set_pose_euler('CC_Base_L_Thigh_04', 3.5, 0, -1)    # Lower thighs
set_pose_euler('CC_Base_R_Thigh_00', 3.5, 0, 1)
set_pose_euler('CC_Base_L_Calf_05', -12.0, 0, 0)    # Straighten calves down to floor
set_pose_euler('CC_Base_R_Calf_021', -12.0, 0, 0)
set_pose_euler('CC_Base_L_Foot_06', -15.0, 0, 0)     # Heel & foot orientation
set_pose_euler('CC_Base_R_Foot_022', -15.0, 0, 0)

bpy.context.view_layer.update()
depsgraph = bpy.context.evaluated_depsgraph_get()
body_eval = body.evaluated_get(depsgraph)
mesh_eval = body_eval.data
world_verts = [body_eval.matrix_world @ v.co for v in mesh_eval.vertices]

# 1. Forehead top Z
head_pb = arm.pose.bones['CC_Base_Head_038']
head_world = arm.matrix_world @ head_pb.head
head_verts = [v for v in world_verts if abs(v.y - head_world.y) < 10]
forehead_top_z = max(v.z for v in head_verts)

# 2. Chest surface Z (within 15 cm of sternum in X/Y)
chest_pb = arm.pose.bones['CC_Base_Spine02_035']
chest_world = arm.matrix_world @ chest_pb.head
chest_verts = [v for v in world_verts if abs(v.x - chest_world.x) < 15 and abs(v.y - chest_world.y) < 15]
chest_top_z = max(v.z for v in chest_verts)
chest_back_z = min(v.z for v in chest_verts)

# 3. Hip surface Z
hip_pb = arm.pose.bones['CC_Base_Hip_02']
hip_world = arm.matrix_world @ hip_pb.head
hip_verts = [v for v in world_verts if abs(v.y - hip_world.y) < 15]
hip_top_z = max(v.z for v in hip_verts)
hip_back_z = min(v.z for v in hip_verts)

# 4. Knee surface Z
knee_pb = arm.pose.bones['CC_Base_R_Calf_021']
knee_world = arm.matrix_world @ knee_pb.head
knee_verts = [v for v in world_verts if abs(v.y - knee_world.y) < 15]
knee_top_z = max(v.z for v in knee_verts)
knee_back_z = min(v.z for v in knee_verts)

# 5. Heel surface Z
heel_pb = arm.pose.bones['CC_Base_R_Foot_022']
heel_world = arm.matrix_world @ heel_pb.head
heel_verts = [v for v in world_verts if abs(v.y - heel_world.y) < 15]
heel_top_z = max(v.z for v in heel_verts)
heel_back_z = min(v.z for v in heel_verts)

min_z_all = min(v.z for v in world_verts)

print("\n--- MEASURED 5 TOP-SURFACE HEIGHTS (Target vs Actual) ---")
print(f"1. Forehead: Target [0.20 - 0.24 m] | Actual: {forehead_top_z/100:.3f} m ({forehead_top_z:.2f} cm)")
print(f"2. Chest:    Target [0.22 - 0.25 m] | Actual: {chest_top_z/100:.3f} m ({chest_top_z:.2f} cm) [Back Z: {chest_back_z/100:.3f} m]")
print(f"3. Hip:      Target [0.18 - 0.22 m] | Actual: {hip_top_z/100:.3f} m ({hip_top_z:.2f} cm) [Back Z: {hip_back_z/100:.3f} m]")
print(f"4. Knee:     Target [0.12 - 0.16 m] | Actual: {knee_top_z/100:.3f} m ({knee_top_z:.2f} cm) [Back Z: {knee_back_z/100:.3f} m]")
print(f"5. Heel:     Target [0.04 - 0.08 m] | Actual: {heel_top_z/100:.3f} m ({heel_top_z:.2f} cm) [Back Z: {heel_back_z/100:.3f} m]")
print(f"OVERALL MIN Z ACROSS MESH: {min_z_all/100:.4f} m ({min_z_all:.2f} cm)")
