using System.Windows.Controls;
using BetterClip.ViewModel.Common;
using GongSolutions.Wpf.DragDrop;

namespace BetterClip.View.Controls
{
    public class FavorDropHandler : DefaultDropHandler
    {
        public override void DragOver(IDropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                bool flag = ShouldCopyData(dropInfo);
                dropInfo.Effects = flag ? DragDropEffects.Copy : DragDropEffects.Move;
                bool isTargetItemCenter = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter);
                bool isTreeViewItem = dropInfo.VisualTargetItem is TreeViewItem;
                bool isFolderItem = dropInfo.TargetItem is FolderItemViewModel;

                if (isTreeViewItem && isTargetItemCenter)
                {
                    if (isFolderItem)
                    {
                        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                    }
                    else
                    {
                        dropInfo.Effects = DragDropEffects.None;
                    }
                }
                else
                {
                    dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                }
            }
        }
        public override void Drop(IDropInfo dropInfo)
        {
            base.Drop(dropInfo);
        }
    }
}
