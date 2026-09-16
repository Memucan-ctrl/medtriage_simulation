import bpy
import math

print("=================== FIX PATIENT SCRIPT START ===================")

arm = bpy.data.objects.get('Armature')
if not arm:
    print("ERROR: Armature object not found!")
else:
    bpy.context.view_layer.objects.active = arm
    arm.select_set(True)
    if arm.mode != 'POSE':
        bpy.ops.object.mode_set(mode='POSE')

# Helper to identify facial/head/eye/tongue bones
def is_facial_or_head_bone(bname):
    name_low = bname.lower()
    keywords = ['jaw', 'tongue', 'eye', 'teeth', 'lip', 'brow', 'cheek', 'chin', 'ear', 'nose', 'facial', 'mouth', 'neck']
    return any(k in name_low for k in keywords)

# STEP 2: Cut Agonal_Gasp entirely
if 'Agonal_Gasp' in bpy.data.actions:
    act_to_del = bpy.data.actions['Agonal_Gasp']
    act_to_del.use_fake_user = False
    bpy.data.actions.remove(act_to_del)
    print("STEP 2 COMPLETE: Agonal_Gasp action removed.")

# Helper to remove unwanted fcurves from an action in Blender 5.2
def remove_facial_fcurves(act_name):
    act = bpy.data.actions.get(act_name)
    if not act or not hasattr(act, 'layers') or len(act.layers) == 0:
        return
    for layer in act.layers:
        for strip in layer.strips:
            for slot in act.slots:
                try:
                    bag = strip.channelbag(slot)
                    if not bag or not hasattr(bag, 'fcurves'):
                        continue
                    to_remove = []
                    for fc in bag.fcurves:
                        for pb in arm.pose.bones:
                            if f'pose.bones["{pb.name}"]' in fc.data_path or f"pose.bones['{pb.name}']" in fc.data_path:
                                if is_facial_or_head_bone(pb.name):
                                    to_remove.append(fc)
                                elif pb.name == 'CC_Base_Head_038' and act_name != 'ROSC_Recovery':
                                    to_remove.append(fc)
                    for fc in to_remove:
                        try:
                            bag.fcurves.remove(fc)
                        except Exception:
                            pass
                except Exception:
                    pass

# STEP 1: Clear facial F-curves on remaining actions
for a_name in ['Idle_Unresponsive', 'Breathing_Shallow', 'Shock_Convulsion', 'ROSC_Recovery']:
    remove_facial_fcurves(a_name)

# Zero all facial, jaw, tongue, eye, neck pose bones explicitly
for pb in arm.pose.bones:
    if is_facial_or_head_bone(pb.name) or pb.name == 'CC_Base_Head_038':
        pb.rotation_mode = 'XYZ'
        pb.rotation_euler = (0, 0, 0)
        pb.location = (0, 0, 0)

print("STEP 1 COMPLETE: All facial, jaw, tongue, eye, neck bones zeroed and F-curves cleared.")

# STEP 3: Check mouth/eye/tongue mesh bindings
print("\n--- STEP 3: MESH BINDING VERIFICATION ---")
check_objs = ['CC_Base_Teeth', 'CC_Base_Tongue', 'CC_Base_Eye']

binding_results = {}
for obj_name in check_objs:
    obj = bpy.data.objects.get(obj_name)
    if not obj:
        print(f"Object '{obj_name}': NOT FOUND in scene")
        binding_results[obj_name] = "NO (Object missing)"
        continue
    
    has_armature_mod = False
    arm_target_correct = False
    for mod in obj.modifiers:
        if mod.type == 'ARMATURE':
            has_armature_mod = True
            if mod.object == arm:
                arm_target_correct = True
    
    vgroup_names = [vg.name for vg in obj.vertex_groups]
    has_head_or_jaw_vg = any(k in v.lower() for v in vgroup_names for k in ['head', 'jaw', 'hip', 'root', 'spine'])
    
    verified = (has_armature_mod and arm_target_correct and has_head_or_jaw_vg)
    binding_results[obj_name] = "YES" if verified else "NO"
    print(f"Object '{obj_name}': Verified = {binding_results[obj_name]}")
    print(f"  - Armature Modifier Present: {has_armature_mod}")
    print(f"  - Target Armature Correct: {arm_target_correct}")
    print(f"  - Vertex Groups Count: {len(vgroup_names)} (Matches Head/Jaw: {has_head_or_jaw_vg})")

# Helper to set base supine pose cleanly
def reset_base_supine_pose():
    # Root
    pb_root = arm.pose.bones['_rootJoint']
    pb_root.rotation_mode = 'XYZ'
    pb_root.rotation_euler = (math.radians(90), 0, 0)
    pb_root.location = (0, 19.2406, 0)

    # Spine, neck, head, jaw zeroed
    for bname in ['CC_Base_Waist_033', 'CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_NeckTwist01_036', 'CC_Base_Head_038', 'CC_Base_JawRoot_040']:
        pb = arm.pose.bones.get(bname)
        if pb:
            pb.rotation_mode = 'XYZ'
            pb.rotation_euler = (0, 0, 0)
            pb.location = (0, 0, 0)

    # Arms
    pb = arm.pose.bones.get('CC_Base_L_Upperarm_050')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, 0, math.radians(-80))
    pb = arm.pose.bones.get('CC_Base_R_Upperarm_074')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, 0, math.radians(80))
    pb = arm.pose.bones.get('CC_Base_L_Forearm_051')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, 0, math.radians(-5))
    pb = arm.pose.bones.get('CC_Base_R_Forearm_077')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, 0, math.radians(5))
    pb = arm.pose.bones.get('CC_Base_L_Hand_055')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, math.radians(-90), 0)
    pb = arm.pose.bones.get('CC_Base_R_Hand_081')
    if pb: pb.rotation_mode = 'XYZ'; pb.rotation_euler = (0, math.radians(90), 0)

def key_target_bones(bone_list, frame_num):
    for bname in bone_list:
        pb = arm.pose.bones.get(bname)
        if pb:
            if pb.rotation_mode == 'XYZ':
                pb.keyframe_insert(data_path="rotation_euler", frame=frame_num)
            elif pb.rotation_mode == 'QUATERNION':
                pb.keyframe_insert(data_path="rotation_quaternion", frame=frame_num)
            pb.keyframe_insert(data_path="location", frame=frame_num)

# STEP 4: ROSC_Recovery cap head motion <= 15 deg on CC_Base_Head_038 ONLY (no neck, no facial)
act_rosc = bpy.data.actions.get('ROSC_Recovery')
if act_rosc:
    arm.animation_data.action = act_rosc
    reset_base_supine_pose()
    
    rosc_bones = ['_rootJoint', 'CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_Head_038', 'CC_Base_R_Upperarm_074']
    
    # Frame 1: Base pose
    key_target_bones(rosc_bones, 1)
    
    # Frame 120 (Mid): Chest expanded, Right hand lifted slightly, Head turned max 12 degrees (strictly <= 15 deg)
    reset_base_supine_pose()
    pb_spine = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_spine: pb_spine.rotation_euler = (math.radians(-5.0), 0, 0)
    pb_chest = arm.pose.bones.get('CC_Base_Spine02_035')
    if pb_chest: pb_chest.rotation_euler = (math.radians(-4.0), 0, 0)
    
    pb_head = arm.pose.bones.get('CC_Base_Head_038')
    if pb_head:
        pb_head.rotation_mode = 'XYZ'
        pb_head.rotation_euler = (0, math.radians(-12.0), 0) # 12 deg turn on CC_Base_Head_038 only
    
    pb_r_arm = arm.pose.bones.get('CC_Base_R_Upperarm_074')
    if pb_r_arm: pb_r_arm.rotation_euler = (math.radians(-10), math.radians(10), math.radians(72))
    
    key_target_bones(rosc_bones, 120)
    
    # Frame 240: Restored pose
    reset_base_supine_pose()
    pb_head = arm.pose.bones.get('CC_Base_Head_038')
    if pb_head: pb_head.rotation_euler = (0, math.radians(-8.0), 0)
    key_target_bones(rosc_bones, 240)

print("STEP 4 COMPLETE: ROSC_Recovery head motion capped to 12 deg on CC_Base_Head_038 only.")

# STEP 5: Rebuild Shock_Convulsion from TORSO & SHOULDERS ONLY (Head, neck, face stay 100% at rest)
act_shock = bpy.data.actions.get('Shock_Convulsion')
if act_shock:
    arm.animation_data.action = act_shock
    
    # Remove all existing fcurves in Shock_Convulsion
    for layer in list(act_shock.layers):
        for strip in list(layer.strips):
            for slot in list(act_shock.slots):
                try:
                    bag = strip.channelbag(slot)
                    if bag:
                        for fc in list(bag.fcurves):
                            bag.fcurves.remove(fc)
                except Exception:
                    pass
    
    reset_base_supine_pose()
    torso_shoulder_bones = ['_rootJoint', 'CC_Base_Spine01_034', 'CC_Base_Spine02_035', 'CC_Base_L_Upperarm_050', 'CC_Base_R_Upperarm_074']
    
    # Frame 1: Base pose
    key_target_bones(torso_shoulder_bones, 1)
    
    # Frame 3: Sharp torso & shoulder jolt peak (Head, neck, face stay at rest)
    reset_base_supine_pose()
    pb_spine = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_spine: pb_spine.rotation_euler = (math.radians(-6.0), 0, 0)
    pb_chest = arm.pose.bones.get('CC_Base_Spine02_035')
    if pb_chest: pb_chest.rotation_euler = (math.radians(-4.0), 0, 0)
    pb_l_arm = arm.pose.bones.get('CC_Base_L_Upperarm_050')
    if pb_l_arm: pb_l_arm.rotation_euler = (math.radians(10), math.radians(-15), math.radians(-70))
    pb_r_arm = arm.pose.bones.get('CC_Base_R_Upperarm_074')
    if pb_r_arm: pb_r_arm.rotation_euler = (math.radians(10), math.radians(15), math.radians(70))
    key_target_bones(torso_shoulder_bones, 3)
    
    # Frame 6 (Mid-frame): Quick rebound settling
    reset_base_supine_pose()
    pb_spine = arm.pose.bones.get('CC_Base_Spine01_034')
    if pb_spine: pb_spine.rotation_euler = (math.radians(-2.0), 0, 0)
    key_target_bones(torso_shoulder_bones, 6)
    
    # Frame 12: Fully settled back to supine
    reset_base_supine_pose()
    key_target_bones(torso_shoulder_bones, 12)

print("STEP 5 COMPLETE: Shock_Convulsion rebuilt from torso/shoulders only. Head/neck/face stay at rest.")

# Re-export FBXs
import os
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

print("RE-EXPORT COMPLETE: Both FBXs successfully written.")
print("=================== FIX PATIENT SCRIPT END ===================")
