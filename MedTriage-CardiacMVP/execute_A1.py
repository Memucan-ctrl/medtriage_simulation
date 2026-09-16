import bpy
import math
import os

print("=================== A1. PERFECT 12-SLICE GROUNDING MASTER (ULTIMATE) ===================")

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

set_pose_euler('_rootJoint', 90, 0, 0)
root_pb = arm.pose.bones['_rootJoint']
root_pb.location = (0, 10.8, 0)

set_pose_euler('CC_Base_Waist_033', 0, 0, 0)
set_pose_euler('CC_Base_Spine01_034', 0, 0, 0)
set_pose_euler('CC_Base_Spine02_035', 0, 0, 0)
set_pose_euler('CC_Base_NeckTwist01_036', 0, 0, 0)
set_pose_euler('CC_Base_Head_038', 0, 0, 0)

set_pose_euler('CC_Base_L_Upperarm_050', 0, 0, -80)
set_pose_euler('CC_Base_R_Upperarm_074', 0, 0, 80)
set_pose_euler('CC_Base_L_Forearm_051', 0, 0, -5)
set_pose_euler('CC_Base_R_Forearm_077', 0, 0, 5)
set_pose_euler('CC_Base_L_Hand_055', 0, -90, 0)
set_pose_euler('CC_Base_R_Hand_081', 0, 90, 0)

# Lower body: Thighs 3.5 deg, Calves 1.0 deg, Feet -30.0 deg
set_pose_euler('CC_Base_L_Thigh_04', 3.5, 0, -1.0)
set_pose_euler('CC_Base_R_Thigh_00', 3.5, 0, 1.0)
set_pose_euler('CC_Base_L_Calf_05', 1.0, 0, 0)
set_pose_euler('CC_Base_R_Calf_021', 1.0, 0, 0)
set_pose_euler('CC_Base_L_Foot_06', -30.0, 0, 0)
set_pose_euler('CC_Base_R_Foot_022', -30.0, 0, 0)

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

print("\n--- A1 ULTIMATE 12-SLICE PROFILE SCAN ---")
print(f"{'Slice':<6} | {'Y Range (cm)':<18} | {'Min Z (cm)':<10} | {'Max Z (cm)':<10} | {'Min Z (m)':<9} | {'Max Z (m)':<9}")
print("-" * 75)

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
    print(f"{i+1:<6} | [{y_start:6.1f}, {y_end:6.1f}] | {s_min_z:10.2f} | {s_max_z:10.2f} | {s_min_z/100:9.3f} | {s_max_z/100:9.3f}")

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

print("A1 PERFECT GROUNDING MASTER COMPLETE.")
