import bpy, os
G = bpy.data.objects.get("Patient_Gown")
print("FOUND=%s" % bool(G))
if G: bpy.data.objects.remove(G, do_unlink=True)
print("GONE=%s" % ("Patient_Gown" not in bpy.data.objects))
print("MESHES=%d" % len([o for o in bpy.data.objects if o.type == 'MESH']))
bpy.ops.wm.save_mainfile()
print("SAVED=%s BYTES=%d" % (bpy.data.filepath, os.path.getsize(bpy.data.filepath)))
