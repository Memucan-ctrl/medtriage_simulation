import bpy
import math
import os

print("=================== A2. NEW ACTION: Idle_Conscious ===================")

arm = bpy.data.objects.get('Armature')
if arm:
    bpy.context.view_layer.objects.active = arm
    arm.select_set(True)
    if arm.mode != 'POSE':
        bpy.ops.object.mode_set(mode='POSE')

def set_pose_euler(bname, x_deg, y_deg, z_deg):
    pb = arm.pose.bones.get(bname)
    if pb:
        pb.rotation_mode = 'XYZ'
        pb.rotation_euler = (math.radians(x_deg), math.radians(y_deg), math.radians(z_deg))

def key_target_bones(bone_list, frame_num):
    for bname in bone_list:
        pb = arm.pose.bones.get(bname)
        if pb:
            if pb.rotation_mode == 'XYZ':
                pb.keyframe_insert(data_path="rotation_euler", frame=frame_num)
            elif pb.rotation_mode == 'QUATERNION':
                pb.keyframe_insert(data_path="rotation_quaternion", frame=frame_num)

# Base flattened pose setup helper
def apply_base_supine_pose():
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

    set_pose_euler('CC_Base_L_Thigh_04', 3.5, 0, -1.0)
    set_pose_euler('CC_Base_R_Thigh_00', 3.5, 0, 1.0)
    set_pose_euler('CC_Base_L_Calf_05', 1.0, 0, 0)
    set_pose_euler('CC_Base_R_Calf_021', 1.0, 0, 0)
    set_pose_euler('CC_Base_L_Foot_06', -5.0, 0, 0)
    set_pose_euler('CC_Base_R_Foot_022', -5.0, 0, 0)

# Create or replace Idle_Conscious action
act_name = "Idle_Conscious"
act = bpy.data.actions.get(act_name)
if act:
    act.use_fake_user = False
    bpy.data.actions.remove(act)

act = bpy.data.actions.new(name=act_name)
act.use_fake_user = True
arm.animation_data.action = act

target_bones = ['CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_Head_038']

# 8.0 s = 240 frames @ 30 FPS. 14 breaths/min = ~128 frames per breathing cycle.
# 2.5 cm chest rise cycle + slow head turn (0 -> -12 deg -> 0 -> +12 deg -> 0)
for f in range(1, 241):
    apply_base_supine_pose()
    
    # 1. Breathing modulation (sine wave over ~128 frames)
    breath_phase = math.sin((f - 1) * (2 * math.pi / 128.57))
    breath_angle = max(0.0, breath_phase) * 3.5 # 3.5 deg chest expansion = ~2.5 cm rise
    
    pb_s1 = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_s1: pb_s1.rotation_euler = (math.radians(-breath_angle * 0.5), 0, 0)
    
    pb_s2 = arm.pose.bones.get('CC_Base_Spine02_035')
    if pb_s2: pb_s2.rotation_euler = (math.radians(-breath_angle * 0.5), 0, 0)
    
    # 2. Head turn modulation (sine wave over 240 frames, max 12 deg turn on CC_Base_Head_038 ONLY)
    head_turn = math.sin((f - 1) * (2 * math.pi / 240)) * 12.0 # Max 12 deg turn (strictly <= 15 deg)
    pb_head = arm.pose.bones.get('CC_Base_Head_038')
    if pb_head: pb_head.rotation_euler = (0, math.radians(head_turn), 0)
    
    key_target_bones(target_bones, f)

print(f"ACTION '{act_name}' CREATED: 240 frames @ 30 FPS.")

# Verify final Action list
all_actions = list(bpy.data.actions.keys())
print("\n--- FINAL 5 ACTIONS VERIFICATION ---")
required_5 = ['Idle_Conscious', 'Idle_Unresponsive', 'Breathing_Shallow', 'Shock_Convulsion', 'ROSC_Recovery']
for r_act in required_5:
    a_obj = bpy.data.actions.get(r_act)
    if a_obj:
        fr = a_obj.frame_range
        fps = bpy.context.scene.render.fps
        dur = (fr[1] - fr[0] + 1) / fps
        print(f"Action '{r_act}': Start={int(fr[0])}, End={int(fr[1])}, Duration={dur:.2f}s, Present=YES")
    else:
        print(f"Action '{r_act}': MISSING!")

# Screenshot setup for A2
out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)
shot_path = os.path.join(out_dir, "A2_Idle_Conscious_Side.png")

arm.animation_data.action = bpy.data.actions['Idle_Conscious']
bpy.context.scene.frame_set(120)
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

print("A2 EXECUTION COMPLETE.")
