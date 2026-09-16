import bpy, collections

dg = bpy.context.evaluated_depsgraph_get()
def bb(w): return [tuple(min(p[i] for p in w) for i in range(3)), tuple(max(p[i] for p in w) for i in range(3))]
def hit(b, lo, hi): return all(b[0][i] <= hi[i] and b[1][i] >= lo[i] for i in range(3))
def pts(mw, me): return [tuple(mw @ v.co) for v in me.vertices]
def fmt(o, b, nv, nf): return "%s verts=%d faces=%d wmin=(%.3f,%.3f,%.3f) wmax=(%.3f,%.3f,%.3f) hide_vp=%s coll=%s" % (o.name, nv, nf, b[0][0], b[0][1], b[0][2], b[1][0], b[1][1], b[1][2], o.hide_viewport, [c.name for c in o.users_collection])

M = [o for o in bpy.data.objects if o.type == 'MESH' and len(o.data.vertices) > 0]
DAT = []
for o in M:
    try:
        ev = o.evaluated_get(dg)
        me = ev.to_mesh()
        DAT.append((o, ev.matrix_world, me))
    except Exception as e:
        pass

BOX = [(o, bb(pts(mw, me)), len(me.vertices), len(me.polygons)) for (o, mw, me) in DAT]

print("=== 1. INSIDE THE TORSO ===")
print("\n".join([fmt(o, b, nv, nf) for (o, b, nv, nf) in BOX if hit(b, (-0.35, -1.55, -0.05), (0.35, -0.85, 0.45))]) or "NOTHING IN TORSO BOX")

print("=== 2. BEHIND / UNDER THE BACK ===")
print("\n".join([fmt(o, b, nv, nf) for (o, b, nv, nf) in BOX if hit(b, (-0.60, -1.75, -0.80), (0.60, -0.70, 0.015))]) or "NOTHING BEHIND THE BACK")

print("=== 3. ANYTHING BELOW THE FLOOR PLANE ===")
print("\n".join(["%s dips %.4f m below z=0  coll=%s" % (o.name, -b[0][2], [c.name for c in o.users_collection]) for (o, b, nv, nf) in BOX if b[0][2] < -0.005]) or "NOTHING BELOW Z=0")

print("=== 4. WHICH BONE OWNS THE FLESH UNDER HIM ===")
SK = sorted([o for o in bpy.data.objects if o.type == 'MESH' and any(m.type == 'ARMATURE' for m in o.modifiers) and len(o.data.vertices) > 0], key=lambda o: -len(o.data.vertices))
PB = SK[0] if SK else None
PW = PB.evaluated_get(dg).matrix_world if PB else None
PM = PB.evaluated_get(dg).to_mesh() if PB else None
GN = {vg.index: vg.name for vg in PB.vertex_groups} if PB else {}
def dom(v): return max(v.groups, key=lambda g: g.weight).group if len(v.groups) else -1
LOW = collections.Counter([dom(v) for v in PM.vertices if (PW @ v.co).z < 0.0]) if PM else collections.Counter()
print("patient_mesh=%s total_verts=%s verts_below_z0=%d" % (PB.name if PB else "NONE", len(PM.vertices) if PM else 0, sum(LOW.values())))
print("\n".join(["  %s = %d verts below z=0" % (GN.get(k, "UNWEIGHTED/%d" % k), c) for (k, c) in sorted(LOW.items(), key=lambda kv: -kv[1])[:15]]) or "  NO PATIENT VERTS BELOW Z=0")

print("=== 5. PATIENT EXTREMES ===")
if PM:
    b_pts = bb(pts(PW, PM))
    print("patient wmin=(%.4f,%.4f,%.4f) wmax=(%.4f,%.4f,%.4f)" % (b_pts[0] + b_pts[1]))
else:
    print("no patient mesh")
print("checked=%d meshes of %d objects" % (len(BOX), len(bpy.data.objects)))

# Clean up created evaluated meshes to avoid memory leaks
for (o, mw, me) in DAT:
    o.evaluated_get(dg).to_mesh_clear()
