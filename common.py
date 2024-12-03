import os
import win32file


def get_file_id(file_path):
    """
    获取文件的唯一 ID（NTFS 文件系统）
    :param file_path: 文件路径
    :return: 文件 ID（卷序列号 + 文件索引）
    """
    handle = win32file.CreateFile(
        file_path,
        win32file.GENERIC_READ,
        win32file.FILE_SHARE_READ | win32file.FILE_SHARE_WRITE | win32file.FILE_SHARE_DELETE,
        None,
        win32file.OPEN_EXISTING,
        win32file.FILE_ATTRIBUTE_NORMAL,
        None,
    )
    info = win32file.GetFileInformationByHandle(handle)
    handle.Close()
    return (info[0], info[1], info[2])  # 返回卷序列号、文件索引高位、文件索引低位

import win32com.client
import os

def set_metadata(file_path, key, value):
    shell = win32com.client.Dispatch("Shell.Application")
    file_path = os.path.abspath(file_path)
    folder_path, file_name = os.path.split(file_path)
    
    # 检查路径是否存在
    if not os.path.exists(folder_path):
        raise FileNotFoundError(f"Folder does not exist: {folder_path}")
    if not os.path.isfile(file_path):
        raise FileNotFoundError(f"File does not exist: {file_path}")
    
    # 获取文件对象
    folder = shell.Namespace(folder_path)
    if folder is None:
        raise RuntimeError(f"Cannot access folder: {folder_path}")
    
    file = folder.ParseName(file_name)
    if file is None:
        raise RuntimeError(f"Cannot access file: {file_name} in {folder_path}")
    
    # 打印调试信息
    print(f"File: {file.Name}, Folder: {folder_path}")
    
    # 当前无法直接设置扩展属性，只能获取
    raise NotImplementedError("Setting extended properties is not supported with this method.")


def get_metadata(file_path, key):
    shell = win32com.client.Dispatch("Shell.Application")
    file_path = str(os.path.abspath(file_path))
    folder = shell.Namespace(file_path.rsplit("\\", 1)[0])
    file = folder.ParseName(file_path.rsplit("\\", 1)[1])
    return file.ExtendedProperty(key)