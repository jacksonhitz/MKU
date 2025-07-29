import json

# List of package names to remove
packages_to_remove = [
    "com.example.seat-licensed",
    "com.some.other-private-package"
]

manifest_path = "Packages/manifest.json"

# Load and modify manifest
with open(manifest_path, "r") as f:
    manifest = json.load(f)

for pkg in packages_to_remove:
    if pkg in manifest["dependencies"]:
        print(f"Removing {pkg} from manifest.json")
        del manifest["dependencies"][pkg]

# Save updated manifest
with open(manifest_path, "w") as f:
    json.dump(manifest, f, indent=2)
    f.write("\n")  # Ensure newline at EOF
