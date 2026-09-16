import bpy, os
p = r"C:\Users\Admin\MedTriage-CardiacMVP\Art_Source"
os.makedirs(p, exist_ok=True)
bpy.ops.wm.save_as_mainfile(filepath=os.path.join(p, "MedTriage_Assets_20260806_2130.blend"), copy=False)
print("SAVED=" + bpy.data.filepath)
print("BYTES=%d" % os.path.getsize(bpy.data.filepath))
