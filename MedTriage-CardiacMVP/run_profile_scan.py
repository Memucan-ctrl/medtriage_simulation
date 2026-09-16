import bpy
D = bpy.context.evaluated_depsgraph_get()
O = bpy.data.objects["CC_Base_Body"].evaluated_get(D)
P = [O.matrix_world @ v.co for v in O.data.vertices]
ymin = min(p.y for p in P)
ymax = max(p.y for p in P)
print("NONCE=MK-PROFILE-2224")
print("VERTS=%d Y_RANGE=(%.4f, %.4f)" % (len(P), ymin, ymax))
def sl(i): return [p for p in P if ymin + (ymax - ymin) * i / 18.0 <= p.y <= ymin + (ymax - ymin) * (i + 1) / 18.0]
print("--- Z PROFILE, 18 slices along his long axis ---")
print("\n".join(["slice%02d y=%.3f n=%d zmin=%.4f zmax=%.4f xspan=%.4f" % (i, ymin + (ymax - ymin) * i / 18.0, len(sl(i)), min([p.z for p in sl(i)]), max([p.z for p in sl(i)]), max([p.x for p in sl(i)]) - min([p.x for p in sl(i)])) for i in range(18) if sl(i)]))
AR = bpy.data.objects["Armature"]
SEL = [b.name for b in AR.pose.bones if any(k in b.name for k in ["Head_038", "Spine01", "Spine02", "Hip_02", "Foot", "Hand_0", "Forearm_0", "Calf_0", "Thigh_0", "Upperarm_0"]) and "scaleCompensation" not in b.name]
print("--- BONE WORLD POSITIONS (%d) ---" % len(SEL))
print("\n".join(["%s = %s" % (n, tuple(round(c, 4) for c in (AR.matrix_world @ AR.pose.bones[n].head))) for n in sorted(SEL)]))
print("POSE_POSITION=%s" % AR.data.pose_position)
print("ASSIGNED_ACTION=%s" % (AR.animation_data.action.name if AR.animation_data and AR.animation_data.action else "NONE"))
