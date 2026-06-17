#!/usr/bin/env python3
"""
解压并整理 Kenney.nl 素材到 Unity Assets 目录

规则：
- 保留原始 zip，解压到临时目录
- 将 PNG/SVG 按包主题分类到 Sprites / Tilesets / UI / Effects
- 将 OGG/WAV 分类到 Audio/SFX 或 Audio/Music
- 将 FBX/OBJ/GLB 分类到 Models
- 保留 License.txt 和包级 README 到对应目录

用法：
    python tools/asset-crawlers/organize_kenney.py
"""
import json
import shutil
import zipfile
from datetime import datetime, timezone
from pathlib import Path
from collections import defaultdict, Counter

PROJECT_ROOT = Path(__file__).resolve().parent.parent.parent
KENNEY_DIR = PROJECT_ROOT / "tools" / "asset-crawlers" / "downloads" / "kenney"
ASSETS_ROOT = PROJECT_ROOT / "Assets"
TEMP_DIR = PROJECT_ROOT / "tools" / "asset-crawlers" / "_unpack_temp"

# 输出目标
OUT_SPRITES = ASSETS_ROOT / "Art" / "Sprites" / "Kenney"
OUT_TILESETS = ASSETS_ROOT / "Art" / "Tilesets" / "Kenney"
OUT_UI = ASSETS_ROOT / "Art" / "UI" / "Kenney"
OUT_EFFECTS = ASSETS_ROOT / "Art" / "Effects" / "Kenney"
OUT_SFX = ASSETS_ROOT / "Audio" / "SFX" / "Kenney"
OUT_MUSIC = ASSETS_ROOT / "Audio" / "Music" / "Kenney"
OUT_MODELS = ASSETS_ROOT / "Models" / "Kenney"

# 文件扩展名归类
IMAGE_EXTS = {".png", ".jpg", ".jpeg", ".svg", ".gif"}
AUDIO_EXTS = {".ogg", ".wav", ".mp3"}
MODEL_EXTS = {".fbx", ".obj", ".glb", ".gltf", ".dae", ".stl", ".mtl"}
META_EXTS = {".txt", ".md", ".json", ".csv", ".url"}
SKIP_DIRS = {"source", "__MACOSX", ".DS_Store"}


def clean_filename(name: str) -> str:
    """去除空格等不适合路径的字符"""
    return name.replace(" ", "_").replace("(", "").replace(")", "")


def category_for_pack(pack_name: str) -> str:
    """根据包名判断主要分类"""
    lowered = pack_name.lower()
    if any(k in lowered for k in ["audio", "sound", "sfx", "ui audio", "digital audio", "rpg audio", "music", "jingle", "voice"]):
        return "audio"
    if any(k in lowered for k in ["particle", "smoke", "light mask", "effect", "vfx", "flare", "magic", "fire", "spark"]):
        return "effects"
    if any(k in lowered for k in ["ui pack", "ui_", "font", "icon", "emote", "rank", "medal", "crosshair", "input prompt"]):
        return "ui"
    if any(k in lowered for k in ["tile", "tileset", "hexagon", "isometric", "platformer", "platform", "dungeon", "graveyard", "background", "map"]):
        return "tilesets"
    if any(k in lowered for k in ["character", "monster", "animal", "enemy", "robot", "vehicle", "car", "tank", "protagonist", "survivor", "weapon", "item", "kit", "pack"]):
        return "sprites"
    return "sprites"


def audio_subcategory(pack_name: str, rel_path: str) -> str:
    """判断音频是音乐还是音效"""
    lowered = pack_name.lower()
    if any(k in lowered for k in ["music", "jingle", "bgm"]):
        return "music"
    if any(k in lowered for k in ["ui audio", "click", "rollover", "switch"]):
        return "sfx"
    # 根据文件路径里的目录名判断
    parts = Path(rel_path).parts
    if any("music" in p.lower() or "bgm" in p.lower() for p in parts):
        return "music"
    return "sfx"


def unpack_and_organize():
    if TEMP_DIR.exists():
        shutil.rmtree(TEMP_DIR)
    TEMP_DIR.mkdir(parents=True, exist_ok=True)

    # 确保输出目录存在
    for d in [OUT_SPRITES, OUT_TILESETS, OUT_UI, OUT_EFFECTS, OUT_SFX, OUT_MUSIC, OUT_MODELS]:
        d.mkdir(parents=True, exist_ok=True)

    stats = defaultdict(lambda: Counter())
    summary = []

    zips = sorted(KENNEY_DIR.glob("*.zip"))
    print(f"[INFO] Found {len(zips)} Kenney packs to organize")

    for zf in zips:
        pack_name = zf.stem
        main_cat = category_for_pack(pack_name)

        # 解压到临时目录
        extract_dir = TEMP_DIR / pack_name
        try:
            with zipfile.ZipFile(zf, "r") as z:
                z.extractall(extract_dir)
        except Exception as e:
            print(f"[WARN] Failed to extract {zf.name}: {e}")
            continue

        # 拷贝文件到目标目录
        copied_files = []
        for src in extract_dir.rglob("*"):
            if src.is_dir():
                continue
            if any(part in SKIP_DIRS for part in src.parts):
                continue

            rel = src.relative_to(extract_dir)
            suffix = src.suffix.lower()

            if suffix in IMAGE_EXTS:
                if main_cat == "effects":
                    dest_dir = OUT_EFFECTS / clean_filename(pack_name)
                elif main_cat == "ui":
                    dest_dir = OUT_UI / clean_filename(pack_name)
                elif main_cat == "tilesets":
                    dest_dir = OUT_TILESETS / clean_filename(pack_name)
                else:
                    dest_dir = OUT_SPRITES / clean_filename(pack_name)
                stats[main_cat]["image"] += 1

            elif suffix in AUDIO_EXTS:
                sub = audio_subcategory(pack_name, str(rel))
                dest_dir = (OUT_MUSIC if sub == "music" else OUT_SFX) / clean_filename(pack_name)
                stats["audio"][sub] += 1

            elif suffix in MODEL_EXTS:
                dest_dir = OUT_MODELS / clean_filename(pack_name)
                stats["models"][suffix] += 1

            elif suffix in META_EXTS or suffix == "":
                # License / README 跟随主分类
                if main_cat == "audio":
                    dest_dir = OUT_SFX / clean_filename(pack_name)
                elif main_cat == "effects":
                    dest_dir = OUT_EFFECTS / clean_filename(pack_name)
                elif main_cat == "ui":
                    dest_dir = OUT_UI / clean_filename(pack_name)
                elif main_cat == "tilesets":
                    dest_dir = OUT_TILESETS / clean_filename(pack_name)
                else:
                    dest_dir = OUT_SPRITES / clean_filename(pack_name)
            else:
                # 其他文件统一放到对应包目录
                if main_cat == "audio":
                    dest_dir = OUT_SFX / clean_filename(pack_name)
                elif main_cat == "effects":
                    dest_dir = OUT_EFFECTS / clean_filename(pack_name)
                elif main_cat == "ui":
                    dest_dir = OUT_UI / clean_filename(pack_name)
                elif main_cat == "tilesets":
                    dest_dir = OUT_TILESETS / clean_filename(pack_name)
                else:
                    dest_dir = OUT_SPRITES / clean_filename(pack_name)

            dest_dir.mkdir(parents=True, exist_ok=True)
            dest_file = dest_dir / rel.name
            try:
                shutil.copy2(src, dest_file)
                copied_files.append(str(dest_file.relative_to(ASSETS_ROOT)))
            except Exception as e:
                print(f"[WARN] Copy failed {src} -> {dest_file}: {e}")

        summary.append({
            "pack": pack_name,
            "category": main_cat,
            "files_copied": len(copied_files),
        })
        print(f"[OK] {pack_name}: {len(copied_files)} files -> {main_cat}")

    # 清理临时目录
    shutil.rmtree(TEMP_DIR)

    # 写入摘要
    report_path = PROJECT_ROOT / "tools" / "asset-crawlers" / "kenney_organization_report.json"
    report = {
        "organized_at": datetime.now(timezone.utc).isoformat(),
        "total_packs": len(zips),
        "stats": {k: dict(v) for k, v in stats.items()},
        "summary": summary,
    }
    report_path.write_text(json.dumps(report, ensure_ascii=False, indent=2), encoding="utf-8")
    print(f"\n[INFO] Report saved to {report_path}")
    print("[STATS]")
    for cat, counts in stats.items():
        print(f"  {cat}: {dict(counts)}")


if __name__ == "__main__":
    unpack_and_organize()
