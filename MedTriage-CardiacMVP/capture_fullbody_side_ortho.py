import os, sys
sys.path.append(r"C:\Users\Admin\MedTriage-CardiacMVP")
import blender_client

out_dir = r"C:\Users\Admin\.gemini\antigravity-ide\brain\a105a873-bab9-48a2-aba4-633ed18f17a4\patient_head_fixes"
os.makedirs(out_dir, exist_ok=True)

shot_path = os.path.join(out_dir, "FullBody_Side_Orthographic_Flattened.png")

code_side = """
import bpy
arm = bpy.data.objects['Armature']
arm.animation_data.action = bpy.data.actions['Idle_Unresponsive']
bpy.context.scene.frame_set(75)
bpy.context.view_layer.update()

for area in bpy.context.screen.areas:
    if area.type == 'VIEW_3D':
        for space in area.spaces:
            if space.type == 'VIEW_3D':
                space.shading.type = 'SOLID'
                space.shading.color_type = 'SINGLE'
                space.overlay.show_overlays = False
                space.region_3d.view_perspective = 'ORTHO'
                # Right side orthographic view (Numpad 3)
                space.region_3d.view_rotation = (0.5, 0.5, 0.5, 0.5)
                
                override = {'area': area, 'space_data': space, 'region': area.regions[-1]}
                try:
                    with bpy.context.temp_override(**override):
                        bpy.ops.view3d.view_all(center=False)
                except Exception as e:
                    print("FRAME_ERR:", e)
"""

blender_client.execute_code(code_side)
blender_client.save_screenshot(shot_path, max_size=1200)
print(f"FULLBODY SIDE ORTHO SCREENSHOT SAVED TO: {shot_path}")
