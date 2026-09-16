import bpy, os
AR = bpy.data.objects["Armature"]
AR.scale = (0.01, 0.01, 0.01)
bpy.context.view_layer.update()
D = bpy.context.evaluated_depsgraph_get()
def pts(n): return [bpy.data.objects[n].evaluated_get(D).matrix_world @ v.co for v in bpy.data.objects[n].evaluated_get(D).data.vertices]
def bb(P): return (min(p.x for p in P), min(p.y for p in P), min(p.z for p in P), max(p.x for p in P), max(p.y for p in P), max(p.z for p in P))
B = bb(pts("CC_Base_Body"))
M = bb(pts("Bed_Mattress"))
print("NONCE=MK-SCALE-2218")
print("ARM_SCALE=%.4f" % AR.scale.x)
print("BODY min=(%.4f, %.4f, %.4f) max=(%.4f, %.4f, %.4f)" % B)
print("BODY_LENGTH=%.4f BODY_THICKNESS_Z=%.4f" % (B[4] - B[1], B[5] - B[2]))
print("MATTRESS_TOP=%.4f BED_Y=(%.4f, %.4f)" % (M[5], M[1], M[4]))
print("RATIO_BODY_TO_BED=%.3f" % ((B[4] - B[1]) / (M[4] - M[1])))
bpy.ops.wm.save_mainfile()
print("SAVED BYTES=%d" % os.path.getsize(bpy.data.filepath))
