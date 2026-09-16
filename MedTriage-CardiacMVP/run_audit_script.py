import bpy, os, datetime, hashlib

NONCE = "MK-AUDIT-20260806-2113-A7Q"
L=[]
def P(s): L.append(str(s))
sc = bpy.context.scene
P("NONCE="+NONCE)
P("BLEND="+(bpy.data.filepath or "UNSAVED"))
try:
    st=os.stat(bpy.data.filepath); P("BLEND_BYTES=%d BLEND_MTIME=%s"%(st.st_size,datetime.datetime.fromtimestamp(st.st_mtime).isoformat()))
except Exception as e: P("BLEND_STAT_ERR="+repr(e))
P("NOW="+datetime.datetime.now().isoformat()+" BLENDER="+bpy.app.version_string)
P("SCENE=%s FRAME=%d UNIT=%s SCALE=%r"%(sc.name,sc.frame_current,sc.unit_settings.system,sc.unit_settings.scale_length))
P("COUNTS objects=%d meshes=%d armatures=%d actions=%d images=%d materials=%d"%(
 len(bpy.data.objects),len([o for o in bpy.data.objects if o.type=='MESH']),
 len([o for o in bpy.data.objects if o.type=='ARMATURE']),len(bpy.data.actions),
 len(bpy.data.images),len(bpy.data.materials)))
dg = bpy.context.evaluated_depsgraph_get()
P("--- OBJECTS name|type|verts|tris|wmin(x,y,z)|wmax(x,y,z)|scale|mods|mats|parent|vgroups ---")
fp={}
for ob in sorted(bpy.data.objects,key=lambda o:o.name):
    nv=nt=0; mn=mx=("-","-","-")
    if ob.type=='MESH':
        try:
            ev=ob.evaluated_get(dg); me=ev.to_mesh()
            nv=len(me.vertices)
            try: me.calc_loop_triangles(); nt=len(me.loop_triangles)
            except Exception: nt=len(me.polygons)
            if nv:
                ws=[ev.matrix_world@v.co for v in me.vertices]
                mn=tuple(round(min(w[i] for w in ws),4) for i in range(3))
                mx=tuple(round(max(w[i] for w in ws),4) for i in range(3))
            ev.to_mesh_clear()
        except Exception as e: P("  MESH_ERR %s %r"%(ob.name,e))
        k=(nv,nt); fp.setdefault(k,[]).append(ob.name)
    P("%s|%s|%d|%d|%s|%s|%s|%s|%s|%s|%d"%(ob.name,ob.type,nv,nt,mn,mx,
      tuple(round(s,4) for s in ob.scale),
      ",".join(m.type for m in ob.modifiers) or "-",
      ",".join(ms.name or "None" for ms in ob.material_slots) or "-",
      ob.parent.name if ob.parent else "-", len(ob.vertex_groups)))
P("--- DUPLICATE GEOMETRY FINGERPRINTS (verts,tris shared by >1 object) ---")
for k,v in fp.items():
    if len(v)>1: P("%s => %s"%(k,v))
P("--- ARMATURES ---")
for ob in [o for o in bpy.data.objects if o.type=='ARMATURE']:
    bn=[b.name for b in ob.data.bones]
    P("%s bones=%d cc_base=%d rootJoint=%s first3=%s"%(ob.name,len(bn),
      len([b for b in bn if b.startswith("CC_Base_")]),
      any(b=="_rootJoint" for b in bn), bn[:3]))
    P("  has_CC_Base_Spine02_035=%s has_CC_Base_Head=%s"%("CC_Base_Spine02_035" in bn,"CC_Base_Head" in bn))
    ad=ob.animation_data
    P("  active_action=%s"%(ad.action.name if ad and ad.action else "NONE"))
P("--- ACTIONS ---")
for a in sorted(bpy.data.actions,key=lambda x:x.name):
    bones=set(); kf=0
    all_fcurves = []
    if hasattr(a, 'fcurves'):
        all_fcurves = list(a.fcurves)
    elif hasattr(a, 'layers'):
        for layer in a.layers:
            for strip in layer.strips:
                for slot in a.slots:
                    try:
                        bag = strip.channelbag(slot)
                        if bag and hasattr(bag, 'fcurves'):
                            all_fcurves.extend(list(bag.fcurves))
                    except Exception: pass
    for fc in all_fcurves:
        kf+=len(fc.keyframe_points)
        dp=fc.data_path
        if 'pose.bones["' in dp: bones.add(dp.split('pose.bones["')[1].split('"]')[0])
    P("%s frames=%s fcurves=%d keyframes=%d keyed_bones=%d users=%d"%(a.name,tuple(round(f,2) for f in a.frame_range),len(all_fcurves),kf,len(bones),a.users))
    P("  bones=%s"%sorted(bones)[:30])
P("--- POSE SAMPLING (lying vs standing) ---")
orig=sc.frame_current
targets=["CC_Base_Head","CC_Base_Hip","CC_Base_Pelvis","CC_Base_L_Foot","CC_Base_R_Foot","CC_Base_L_Thigh","CC_Base_Spine02_035"]
for ob in [o for o in bpy.data.objects if o.type=='ARMATURE']:
    for f in [int(sc.frame_start),int((sc.frame_start+sc.frame_end)//2),int(sc.frame_end)]:
        sc.frame_set(f); bpy.context.view_layer.update()
        out=[]
        for t in targets:
            pb=ob.pose.bones.get(t)
            if pb:
                w=(ob.matrix_world@pb.matrix).translation
                out.append("%s=(%.4f,%.4f,%.4f)"%(t,w.x,w.y,w.z))
        P("%s f%d %s"%(ob.name,f," ".join(out)))
sc.frame_set(orig); bpy.context.view_layer.update()
P("--- PATIENT MESH REGION HEIGHTS (evaluated, weight>0.5) ---")
cand=[o for o in bpy.data.objects if o.type=='MESH' and any(m.type=='ARMATURE' for m in o.modifiers)]
cand.sort(key=lambda o:len(o.data.vertices),reverse=True)
for ob in cand[:2]:
    ev=ob.evaluated_get(dg); me=ev.to_mesh(); mw=ev.matrix_world
    gi={vg.index:vg.name for vg in ob.vertex_groups}
    for t in targets+["CC_Base_L_Calf","CC_Base_R_Calf"]:
        idx=[i for i,n in gi.items() if n==t]
        if not idx: continue
        zs=[(mw@v.co).z for v in me.vertices if any(g.group in idx and g.weight>0.5 for g in v.groups)]
        if zs: P("%s %s n=%d zmin=%.4f zmax=%.4f zmean=%.4f"%(ob.name,t,len(zs),min(zs),max(zs),sum(zs)/len(zs)))
    ev.to_mesh_clear()
P("--- IMAGES ---")
for im in sorted(bpy.data.images,key=lambda x:x.name):
    b=-1
    try:
        b=os.path.getsize(bpy.path.abspath(im.filepath)) if im.filepath else -1
    except Exception: pass
    P("%s src=%s packed=%s size=%s bytes=%d users=%d"%(im.name,im.source,bool(im.packed_file),tuple(im.size),b,im.users))
P("--- EXPORT DIR LISTING ---")
EXPORT_DIRS=[os.path.dirname(bpy.data.filepath) or ".", r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Patient", r"C:\Users\Admin\MedTriage-CardiacMVP\Assets\MedTriage\Art\Props"]
for d in EXPORT_DIRS:
    P("DIR="+os.path.abspath(d))
    try:
        for fn in sorted(os.listdir(d)):
            p=os.path.join(d,fn)
            if os.path.isfile(p) and fn.lower().split(".")[-1] in ("fbx","glb","gltf","blend","blend1","png","jpg","exr"):
                s=os.stat(p); P("  %s %d %s"%(fn,s.st_size,datetime.datetime.fromtimestamp(s.st_mtime).isoformat()))
    except Exception as e: P("  DIR_ERR "+repr(e))
txt="\n".join(L)
print(txt)
print("SHA256="+hashlib.sha256((NONCE+txt).encode()).hexdigest())
print("LINES=%d"%len(L))
