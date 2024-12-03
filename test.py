import os
import shutil
from common import get_metadata


def set_metadata(file_path, key, value):
    # 使用 NTFS 流保存元数据
    ads_path = f"{file_path}:{key}"
    with open(ads_path, "w", encoding="utf-8") as f:
        f.write(value)


def collect(name: str, text: str, des: str = None):
    """
    收集文本内容并自动处理文件名重复问题。

    :param name: 初始文件名
    :param text: 需要写入的文本内容
    """
    # 检查并生成唯一文件名
    base_path = os.path.join(root_dir, name)
    file_path = base_path
    count = 1
    ext = ".txt"
    # 如果文件已经存在，自动重命名
    while os.path.exists(file_path + ext):
        file_path = f"{base_path}.{count}"
        count += 1

    # 写入文件
    with open(file_path + ext, "w") as f:
        f.write(text)

    if des:
        set_metadata(file_path + ext, "des", des)


def update():
    pass


def test_collect():
    collect("aaa", "0", "des1")
    collect("aaa", "1", "des2")


def walk_dir(root_dir):
    for root, dirs, files in os.walk(root_dir):
        for file in files:
            file_path = os.path.join(root, file)
            des = get_metadata(file_path, "des")
            if des:
                print(file_path, open(file_path).read(), des)
            else:
                print(file_path, open(file_path).read())


if __name__ == "__main__":
    root_dir = "./dataset"
    shutil.rmtree(root_dir, ignore_errors=True)
    os.makedirs(root_dir, exist_ok=True)
    test_collect()
    walk_dir(root_dir)
