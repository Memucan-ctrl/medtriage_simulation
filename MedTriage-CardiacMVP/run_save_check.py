import bpy, os, shutil
P = bpy.data.filepath
OLD = r"C:\Users\Admin\.gemini\antigravity-ide\brain\69483519-29f2-4a5a-82af-8c335983377d\Untitled.blend"
print("NONCE=MK-SAVECHECK-2207")
print("CURRENT=" + P)
print("SAVED_BYTES=%d MTIME=%s" % (os.path.getsize(P), os.path.getmtime(P)))
print("OLD_EXISTS=%s OLD_BYTES=%s" % (os.path.exists(OLD), os.path.getsize(OLD) if os.path.exists(OLD) else -1))
print("FREE_GB=%.2f" % (shutil.disk_usage("C:\\").free / 1e9))
print("COUNTS objects=%d meshes=%d armatures=%d actions=%d images=%d materials=%d" % (len(bpy.data.objects), len([o for o in bpy.data.objects if o.type == 'MESH']), len([o for o in bpy.data.objects if o.type == 'ARMATURE']), len(bpy.data.actions), len(bpy.data.images), len(bpy.data.materials)))
def ab(i): return bpy.path.abspath(i.filepath) if i.filepath else ""
def ex(i): return bool(ab(i)) and os.path.exists(ab(i))
def sz(i): return os.path.getsize(ab(i)) if ex(i) else -1
ORPH = [i for i in bpy.data.images if i.users == 0]
print("ORPHAN_IMAGES=%d ORPHAN_PACKED=%d ORPHAN_SOURCE_BYTES=%d" % (len(ORPH), len([i for i in ORPH if i.packed_file]), sum([max(sz(i), 0) for i in ORPH])))
LIVE = [i for i in bpy.data.images if i.users > 0 and i.source == 'FILE']
print("--- IN-USE TEXTURES (%d) ---" % len(LIVE))
print("\n".join(["%s packed=%s exists=%s bytes=%d path=%s" % (i.name, bool(i.packed_file), ex(i), sz(i), ab(i)) for i in LIVE]) or "NONE")
print("PACKED_IN_USE=%d of %d   MISSING_IN_USE=%d" % (len([i for i in LIVE if i.packed_file]), len(LIVE), len([i for i in LIVE if not ex(i)])))
print("ACTIONS(%d)=%s" % (len(bpy.data.actions), sorted([a.name for a in bpy.data.actions])))
print("GOWN_PRESENT=%s  BODY_PRESENT=%s" % ("Patient_Gown" in bpy.data.objects, "CC_Base_Body" in bpy.data.objects))
