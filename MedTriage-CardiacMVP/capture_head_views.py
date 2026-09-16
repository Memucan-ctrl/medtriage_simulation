import bpy
import os

print("=================== HEAD CLOSE-UP SCREENSHOT CAPTURE ===================")

arm = bpy.data.objects.get('Armature')
if arm:
    bpy.context.view_layer.objects.active = arm
    arm.select_set(True)
    if arm.mode != 'POSE':
        bpy.ops.object.mode_set(mode='POSE')

# Helper to position 3D viewport close up to head
head_bone = arm.pose.bones.get('CC_Base_Head_038')
head_world = arm.matrix_world @ head_bone.head

# Head position is at (0, -162.5, 30.0) in supine pose
print(f"HEAD WORLD LOCATION: {head_world}")

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)

# Define clips and mid-frames
clips = [
    ("Idle_Unresponsive", 75),
    ("Breathing_Shallow", 90),
    ("Shock_Convulsion", 6),
    ("ROSC_Recovery", 120)
]

def setup_head_view(view_mode):
    for area in bpy.context.screen.areas:
        if area.type == 'VIEW_3D':
            for space in area.spaces:
                if space.type == 'VIEW_3D':
                    space.shading.type = 'SOLID'
                    space.shading.color_type = 'SINGLE'
                    space.overlay.show_overlays = False
                    space.region_3d.view_perspective = 'ORTHO'
                    space.region_3d.view_distance = 45.0 # Close-up view
                    space.region_3d.view_location = (0, -162.5, 32.0) # Centered on head
                    
                    if view_mode == 'FRONT':
                        # Looking down along Y from -Y (facing front of face)
                        space.region_3d.view_rotation = (0, 0, 0.7071068, 0.7071068) # Front view (RX=0, RY=0, RZ=90)
                    elif view_mode == 'SIDE':
                        # Looking from +X side of head
                        space.region_3d.view_rotation = (0.5, 0.5, 0.5, 0.5)

# Render and capture screenshots for each clip
for act_name, frame in clips:
    act = bpy.data.actions.get(act_name)
    if act and arm.animation_data:
        arm.animation_data.action = act
        bpy.context.scene.frame_set(frame)
        bpy.context.view_layer.update()
        print(f"PREPARED {act_name} at frame {frame}")

print("=================== HEAD CLOSE-UP READY ===================")
