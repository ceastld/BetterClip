import json
import os
import shutil


# avatar_file = "Avatar.json"
# avatar_dir = "D:\文档\Hutao\Metadata\CHS\Avatar"
avatar_dir = "avatar"

for root, dirs, files in os.walk(avatar_dir):
    avatar_list = []
    for file in files:
        file_path = os.path.join(root, file)
        with open(file_path, "r", encoding="utf-8") as f:
            avatar = json.loads(f.read())
            avatar_list.append(avatar)

simply_avatar_list = []
for avatar in avatar_list:
    simply_avatar_list.append(
        {
            "Name": avatar["Name"],
            "Icon": avatar["Icon"],
            "Id": avatar["Id"],
            "Quality": avatar["Quality"],
            "Sort": avatar["Sort"],
        }
    )
    # print(avatar["Name"],avatar["Icon"])

simply_avatar_file = "SimplyAvatar.json"

open(simply_avatar_file, "w").write(json.dumps(simply_avatar_list, indent=4, ensure_ascii=False))

target_avatar_file = "src\BetterClip\Resources\SimplyAvatar.json"
shutil.copyfile(simply_avatar_file, target_avatar_file)
