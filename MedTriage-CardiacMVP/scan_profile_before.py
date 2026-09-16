import bpy
import math

print("=================== 12-SLICE PROFILE SCAN BEFORE FIX ===================")

arm = bpy.data.objects.get('Armature')
body = bpy.data.objects.get('CC_Base_Body')

if not arm or not body:
    print("ERROR: Armature or Body object missing!")
else:
    bpy.context.view_layer.objects.active = arm
    arm.select_set(True)
    if arm.mode != 'POSE':
        bpy.ops.object.mode_set(mode='POSE')

# Ensure evaluated depsgraph is current
bpy.context.view_layer.update()
depsgraph = bpy.context.evaluated_depsgraph_get()
body_eval = body.evaluated_get(depsgraph)
mesh_eval = body_eval.data
world_verts = [body_eval.matrix_world @ v.co for v in mesh_eval.vertices]

# Long axis is Y. Find crown (min Y) and heel (max Y)
min_y = min(v.y for v in world_verts)
max_y = max(v.y for v in world_verts)
total_len = max_y - min_y

print(f"BODY Y RANGE: [{min_y:.2f}, {max_y:.2f}] (Total Length: {total_len:.2f} cm)")

num_slices = 12
slice_step = total_len / num_slices

print(f"{'Slice':<6} | {'Y Range (cm)':<18} | {'Min Z (cm)':<10} | {'Max Z (cm)':<10} | {'Min Z (m)':<9} | {'Max Z (m)':<9}")
print("-" * 75)

before_slices = []
for i in range(num_slices):
    y_start = min_y + i * slice_step
    y_end = min_y + (i + 1) * slice_step
    
    # Vertices in this slice
    slice_verts = [v for v in world_verts if y_start <= v.y < y_end]
    if slice_verts:
        s_min_z = min(v.z for v in slice_verts)
        s_max_z = max(v.z for v in slice_verts)
    else:
        s_min_z = 0.0
        s_max_z = 0.0
    
    before_slices.append((i+1, y_start, y_end, s_min_z, s_max_z))
    print(f"{i+1:<6} | [{y_start:6.1f}, {y_end:6.1f}] | {s_min_z:10.2f} | {s_max_z:10.2f} | {s_min_z/100:9.3f} | {s_max_z/100:9.3f}")

print("=================== 12-SLICE PROFILE SCAN COMPLETE ===================")
