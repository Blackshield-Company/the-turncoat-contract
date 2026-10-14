#!/usr/bin/env python3
"""Independent check (no Mutagen) of <Alias=...> text tags in a Skyrim SE plugin.

Fails when a tag in dialogue (INFO responses NAM1, prompts RNAM, DIAL names FULL) or quest
text (QUST name FULL, journal CNAM, objectives NNAM) does not name a filled, non-optional
alias of the quest that owns the text (the topic's QNAM quest for dialogue), or when a note
created by an alias uses tags without Stores Text + Uses Stored Text on that alias.
Usage: alias_tags.py Plugin.esp [...]
"""
import re, struct, sys, zlib

OPTIONAL, STORES, USES_STORED = 0x2, 0x100, 0x4000
FILL = {"ALFR", "ALUA", "ALCO", "ALFA", "ALEQ", "ALNA", "ALFE", "ALFL", "CTDA", "ALCA"}
TAG = re.compile(r"<Alias(?:\.[A-Za-z]+)?=([^>.]+)[^>]*>")


def subs(d):
    i, big = 0, None
    while i < len(d):
        t, s = struct.unpack_from("<4sH", d, i)
        if t == b"XXXX":
            big = struct.unpack_from("<I", d, i + 6)[0]; i += 10; continue
        if big is not None: s, big = big, None
        yield t.decode(), d[i + 6:i + 6 + s]; i += 6 + s


def z(b): return b.split(b"\0")[0].decode("cp1252", "replace")


def records(path):
    buf = open(path, "rb").read(); out = []
    def walk(off, end, topic):
        while off < end:
            if buf[off:off + 4] == b"GRUP":
                size, label, gt = struct.unpack_from("<IIi", buf, off + 4)
                walk(off + 24, off + size, label if gt == 7 else topic); off += size
            else:
                t, size, fl, fid = struct.unpack_from("<4sIII", buf, off); d = buf[off + 24:off + 24 + size]
                if fl & 0x40000: d = zlib.decompress(d[4:])
                out.append((t.decode(), fid, list(subs(d)), topic)); off += 24 + size
    _, size = struct.unpack_from("<4sI", buf, 0); walk(24 + size, len(buf), None)
    return out


def check(path):
    R = records(path); quests, topics, books, errs = {}, {}, {}, []
    edid = lambda ss: next((z(d) for s, d in ss if s == "EDID"), "?")
    for t, fid, ss, _ in R:
        if t == "QUST":
            al, cur = {}, None
            for s, d in ss:
                if s in ("ALST", "ALLS"): cur = {"name": "", "flags": 0, "fill": False, "create": None}; al[struct.unpack("<I", d[:4])[0]] = cur
                elif s == "ALED": cur = None
                elif cur is None: continue
                elif s == "ALID": cur["name"] = z(d)
                elif s == "FNAM": cur["flags"] = struct.unpack("<I", d[:4])[0]
                elif s in FILL:
                    cur["fill"] = True
                    if s == "ALCO": cur["create"] = struct.unpack("<I", d[:4])[0]
            quests[fid] = (edid(ss), al, ss)
        elif t == "DIAL": topics[fid] = (edid(ss), next((struct.unpack("<I", d)[0] for s, d in ss if s == "QNAM"), None), ss)
        elif t == "BOOK": books[fid] = (edid(ss), next((z(d) for s, d in ss if s == "DESC"), ""))

    def tags(where, qid, text):
        q = quests.get(qid)
        for name in TAG.findall(text):
            if q is None: errs.append(f"{where}: <Alias={name}> but no owning quest in this plugin"); continue
            a = next((a for a in q[1].values() if a["name"].lower() == name.lower()), None)
            if a is None:
                other = [e for k, (e, al, _) in quests.items() if k != qid and any(x["name"].lower() == name.lower() for x in al.values())]
                errs.append(f"{where}: <Alias={name}> but quest {q[0]} has no such alias" + (f" (it belongs to {', '.join(other)})" if other else ""))
            elif a["flags"] & OPTIONAL or not a["fill"]:
                errs.append(f"{where}: <Alias={name}> on {q[0]} is optional or not filled at quest start")

    for qid, (qe, al, ss) in quests.items():
        # Journal text only shows for quests with a journal type or objectives. Type None quests
        # without objectives (text holders for message boxes) may use optional aliases on purpose.
        dnam = next((d for s, d in ss if s == "DNAM"), b"")
        qtype = dnam[8] if len(dnam) > 8 else 0
        visible = qtype != 0 or any(s == "QOBJ" for s, _ in ss)
        for s, d in ss:
            if s in ("FULL", "CNAM", "NNAM") and visible: tags(f"QUST {qe} {s}", qid, z(d))
        for a in al.values():
            if a["create"] in books and TAG.search(books[a["create"]][1]):
                if (a["flags"] & (STORES | USES_STORED)) != (STORES | USES_STORED):
                    errs.append(f"QUST {qe}: alias {a['name']} creates {books[a['create']][0]} with <Alias=...> text but lacks Stores Text + Uses Stored Text")
                tags(f"BOOK {books[a['create']][0]} (via {qe}.{a['name']})", qid, books[a["create"]][1])
    for tid, (te, qid, ss) in topics.items():
        for s, d in ss:
            if s == "FULL": tags(f"DIAL {te}", qid, z(d))
    for t, fid, ss, top in R:
        if t == "INFO" and top in topics:
            te, qid, _ = topics[top]
            for s, d in ss:
                if s in ("NAM1", "RNAM"): tags(f"DIAL {te} / INFO {edid(ss)} [{fid & 0xFFFFFF:06X}] {s}", qid, z(d))
    return errs


if __name__ == "__main__":
    bad = 0
    for p in sys.argv[1:]:
        e = check(p); bad += len(e)
        for x in e: print(f"{p.split('/')[-1]}: ERROR {x}")
        if not e: print(f"{p.split('/')[-1]}: alias tags OK")
    sys.exit(1 if bad else 0)
