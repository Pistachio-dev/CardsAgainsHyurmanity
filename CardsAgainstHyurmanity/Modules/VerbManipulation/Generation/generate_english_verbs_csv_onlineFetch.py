import csv
import re
import requests
from io import StringIO

def fetch_list(url):
    resp = requests.get(url)
    resp.raise_for_status()
    return resp.text

def process_irregular_list(text):
    # Example format: base, past, past_participle
    verbs = []
    for line in text.splitlines():
        line = line.strip()
        if not line or line.startswith('#'):
            continue
        parts = re.split(r'\s+|,', line)
        if len(parts) >= 3:
            base = parts[0]
            past = parts[1]
            pp = parts[2]
            ing = base + "ing"  # simple heuristic
            verbs.append((base, past, pp, ing))
    return verbs

def generate_regular(forms_list):
    verbs = []
    for base in forms_list:
        if base.endswith("e") and not base.endswith("ee"):
            past = base + "d"
            pp = past
            ing = base[:-1] + "ing"
        elif base.endswith("y") and base[-2] not in "aeiou":
            past = base[:-1] + "ied"
            pp = past
            ing = base + "ing"
        else:
            past = base + "ed"
            pp = past
            ing = base + "ing"
        verbs.append((base, past, pp, ing))
    return verbs

def main():
    # URLs or file paths you supply
    url_irregular = "https://languageonschools.com/blog/english-irregular-verbs-list/"  # sample
    url_regular_list = "https://example.com/list-of-regular-verbs.txt"  # replace with real list

    irregular_text = fetch_list(url_irregular)
    irregular_verbs = process_irregular_list(irregular_text)
    # For regular list: assume one verb per line
    regular_text = fetch_list(url_regular_list)
    regular_bases = [w.strip() for w in regular_text.splitlines() if w.strip()]
    regular_verbs = generate_regular(regular_bases)

    all_verbs = irregular_verbs + regular_verbs

    # Optionally, deduplicate by base
    seen = set()
    uniq = []
    for v in all_verbs:
        if v[0] not in seen:
            seen.add(v[0])
            uniq.append(v)

    with open("english_verbs_full_auto.csv", "w", newline="", encoding="utf-8") as f:
        writer = csv.writer(f)
        writer.writerow(["base", "past", "past_participle", "ing_form"])
        writer.writerows(uniq)

    print(f"✅ CSV created: english_verbs_full_auto.csv ({len(uniq)} verbs)")

if __name__ == "__main__":
    main()
