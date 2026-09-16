import os, sys
sys.path.append(r"C:\Users\Admin\MedTriage-CardiacMVP")
import blender_client

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)

clips = [
    ("Idle_Unresponsive", 75),
    ("Breathing_Shallow", 90),
    ("Shock_Convulsion", 6),
    ("ROSC_Recovery", 120)
]

for act_name, mid_frame in clips:
    # 1. Front head close-up
    code_front = f"""
import bpy
arm = bpy.data.objects['Armature']
arm.animation_data.action = bpy.data.actions['{act_name}']
bpy.context.scene.frame_set({mid_frame})
bpy.context.view_layer.update()

for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        for space in area.spaces:
            if space.type == 'VIEW_3D':
                space.shading.type = 'SOLID'
                space.shading.color_type = 'SINGLE'
                space.overlay.show_overlays = False
                space.region_3d.view_perspective = 'ORTHO'
                space.region_3d.view_distance = 40.0
                space.region_3d.view_location = (0, -162.5, 32.0)
                space.region_3d.view_rotation = (0, 0, 0.7071068, 0.7071068) # Top/Front of face
"""
    blender_client.execute_code(code_front)
    shot_path_front = os.path.join(out_dir, f"{act_name}_frame{mid_frame}_Front.png")
    blender_client.save_screenshot(shot_path_front)
    
    # 2. Side head close-up
    code_side = f"""
import bpy
arm = bpy.data.objects['Armature']
arm.animation_data.action = bpy.data.actions['{act_name}']
bpy.context.scene.frame_set({mid_frame})
bpy.context.view_layer.update()

for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        for space in area.spaces:
            if space.type == 'VIEW_3D':
                space.shading.type = 'SOLID'
                space.shading.color_type = 'SINGLE'
                space.overlay.show_overlays = False
                space.region_3d.view_perspective = 'ORTHO'
                space.region_3d.view_distance = 45.0
                space.region_3d.view_location = (0, -162.5, 32.0)
                space.region_3d.view_rotation = (0.5, 0.5, 0.5, 0.5) # Side of head
"""
    blender_client.execute_code(code_side)
    shot_path_side = os.path.join(out_dir, f"{act_name}_frame{mid_frame}_Side.png")
    blender_client.save_screenshot(shot_path_side)

print("ALL HEAD CLOSE-UP SCREENSHOTS CAPTURED AND SAVED.")
