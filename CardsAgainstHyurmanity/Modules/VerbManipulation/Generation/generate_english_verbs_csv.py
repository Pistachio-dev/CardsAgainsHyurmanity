import csv

# Basic seed list (irregulars with variants)
irregular_verbs = [
    ("be", "was/were", "been", "being"),
    ("become", "became", "become", "becoming"),
    ("begin", "began", "begun", "beginning"),
    ("bend", "bent", "bent", "bending"),
    ("bet", "bet", "bet", "betting"),
    ("bind", "bound", "bound", "binding"),
    ("bite", "bit", "bitten", "biting"),
    ("blow", "blew", "blown", "blowing"),
    ("break", "broke", "broken", "breaking"),
    ("bring", "brought", "brought", "bringing"),
    ("build", "built", "built", "building"),
    ("burn", "burned/burnt", "burned/burnt", "burning"),
    ("buy", "bought", "bought", "buying"),
    ("catch", "caught", "caught", "catching"),
    ("choose", "chose", "chosen", "choosing"),
    ("come", "came", "come", "coming"),
    ("cost", "cost", "cost", "costing"),
    ("cut", "cut", "cut", "cutting"),
    ("deal", "dealt", "dealt", "dealing"),
    ("dig", "dug", "dug", "digging"),
    ("do", "did", "done", "doing"),
    ("draw", "drew", "drawn", "drawing"),
    ("dream", "dreamed/dreamt", "dreamed/dreamt", "dreaming"),
    ("drink", "drank", "drunk", "drinking"),
    ("drive", "drove", "driven", "driving"),
    ("eat", "ate", "eaten", "eating"),
    ("fall", "fell", "fallen", "falling"),
    ("feel", "felt", "felt", "feeling"),
    ("fight", "fought", "fought", "fighting"),
    ("find", "found", "found", "finding"),
    ("fly", "flew", "flown", "flying"),
    ("forget", "forgot", "forgotten", "forgetting"),
    ("forgive", "forgave", "forgiven", "forgiving"),
    ("freeze", "froze", "frozen", "freezing"),
    ("get", "got", "got/gotten", "getting"),
    ("give", "gave", "given", "giving"),
    ("go", "went", "gone", "going"),
    ("grow", "grew", "grown", "growing"),
    ("hang", "hung", "hung", "hanging"),
    ("have", "had", "had", "having"),
    ("hear", "heard", "heard", "hearing"),
    ("hide", "hid", "hidden", "hiding"),
    ("hold", "held", "held", "holding"),
    ("keep", "kept", "kept", "keeping"),
    ("know", "knew", "known", "knowing"),
    ("lead", "led", "led", "leading"),
    ("learn", "learned/learnt", "learned/learnt", "learning"),
    ("leave", "left", "left", "leaving"),
    ("lend", "lent", "lent", "lending"),
    ("let", "let", "let", "letting"),
    ("lie", "lay", "lain", "lying"),
    ("lose", "lost", "lost", "losing"),
    ("make", "made", "made", "making"),
    ("mean", "meant", "meant", "meaning"),
    ("meet", "met", "met", "meeting"),
    ("pay", "paid", "paid", "paying"),
    ("put", "put", "put", "putting"),
    ("read", "read", "read", "reading"),
    ("ride", "rode", "ridden", "riding"),
    ("ring", "rang", "rung", "ringing"),
    ("rise", "rose", "risen", "rising"),
    ("run", "ran", "run", "running"),
    ("say", "said", "said", "saying"),
    ("see", "saw", "seen", "seeing"),
    ("sell", "sold", "sold", "selling"),
    ("send", "sent", "sent", "sending"),
    ("set", "set", "set", "setting"),
    ("shake", "shook", "shaken", "shaking"),
    ("shine", "shone", "shone", "shining"),
    ("shoot", "shot", "shot", "shooting"),
    ("show", "showed", "shown", "showing"),
    ("sing", "sang", "sung", "singing"),
    ("sit", "sat", "sat", "sitting"),
    ("sleep", "slept", "slept", "sleeping"),
    ("speak", "spoke", "spoken", "speaking"),
    ("spend", "spent", "spent", "spending"),
    ("stand", "stood", "stood", "standing"),
    ("swim", "swam", "swum", "swimming"),
    ("take", "took", "taken", "taking"),
    ("teach", "taught", "taught", "teaching"),
    ("tell", "told", "told", "telling"),
    ("think", "thought", "thought", "thinking"),
    ("throw", "threw", "thrown", "throwing"),
    ("understand", "understood", "understood", "understanding"),
    ("wear", "wore", "worn", "wearing"),
    ("win", "won", "won", "winning"),
    ("write", "wrote", "written", "writing"),
]

# Add thousands of regular verbs by rule
# (You can replace this list with your own or expand via a text corpus)
regular_bases = [
    "accept", "add", "admire", "allow", "answer", "arrive", "ask", "bake", "believe",
    "call", "care", "change", "clean", "close", "compare", "cook", "dance", "deliver",
    "enjoy", "explain", "help", "imagine", "improve", "invite", "jump", "laugh", "learn",
    "like", "listen", "live", "look", "love", "move", "need", "open", "plan", "play",
    "rain", "reach", "remember", "save", "share", "show", "start", "stay", "talk",
    "travel", "try", "use", "wait", "walk", "want", "watch", "work"
]

regular_verbs = []
for base in regular_bases:
    if base.endswith("e"):
        past = base + "d"
        participle = past
        ing = base[:-1] + "ing"
    elif base.endswith("y") and base[-2] not in "aeiou":
        past = base[:-1] + "ied"
        participle = past
        ing = base + "ing"
    else:
        past = base + "ed"
        participle = past
        ing = base + "ing"
    regular_verbs.append((base, past, participle, ing))

# Combine and write
all_verbs = irregular_verbs + regular_verbs

with open("english_verbs_full.csv", "w", newline="", encoding="utf-8") as f:
    writer = csv.writer(f)
    writer.writerow(["base", "past", "past_participle", "ing_form"])
    writer.writerows(all_verbs)

print(f"✅ CSV created: english_verbs_full.csv ({len(all_verbs)} verbs)")
